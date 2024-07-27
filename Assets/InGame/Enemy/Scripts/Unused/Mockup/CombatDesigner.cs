#if false

using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Enemy.Unused.GPT;
using System;
using System.Buffers;

namespace Enemy.Unused
{
    /// <summary>
    /// 敵のレベルを調整するパラメータを送信するメッセージの構造体
    /// -1から1の値をとり、基本の値の倍率として扱う想定。
    /// </summary>
    public struct LevelAdjustMessage
    {
        public const float Min = -1.0f;
        public const float Max = 1.0f;

        private float _moveSpeed;
        private float _fireRate;

        /// <summary>
        /// 移動速度の倍率
        /// </summary>
        public float MoveSpeed
        {
            get => _moveSpeed;
            set => _moveSpeed = Mathf.Clamp(value, -1.0f, 1.0f);
        }
        /// <summary>
        /// 攻撃速度の倍率
        /// </summary>
        public float FireRate
        {
            get => _fireRate;
            set => _fireRate = Mathf.Clamp(value, -1.0f, 1.0f);
        }
    }

    /// <summary>
    /// GPT-4を用いてゲームのレベルを調整する。
    /// </summary>
    public class CombatDesigner : MonoBehaviour
    {
        // シングルトン的な運用のため、一応インスタンスの数を数える
        private static int _instanceCount;

        [SerializeField] private CombatDesignAiDebugger _debbuger;
        [Header("APIキー")]
        [SerializeField] private string _key;
        [Header("リクエストする間隔")]
        [Tooltip("AI空のレスポンスが返ってきた瞬間から計測")]
        [SerializeField] private float _interval;
        [Header("デバッグ用: AIを使用するか")]
        [SerializeField] private bool _useAI = true;

        private FireSource _fireSource;
        private ExitSource _exitSource;

        private void Awake()
        {
            // インスタンスの数が1つかチェック
            _instanceCount++;
            if (_instanceCount > 1)
            {
                Debug.LogError($"{nameof(CombatDesigner)}のインスタンスが重複している。");
            }

            _fireSource = new FireSource();
            _exitSource = new ExitSource();

            // 自身にメッセージングする。
            MessageBroker.Default.Receive<FireSource.Message>()
                .Subscribe(_fireSource.Calculate).AddTo(this);
            MessageBroker.Default.Receive<ExitSource.Message>()
                .Subscribe(_exitSource.Calculate).AddTo(this);
        }

        private void Start()
        {
            if (!_useAI) return;
            
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
            
            if (_debbuger != null) _debbuger.UseAI = _useAI;
        }

        private void OnDestroy()
        {
            _instanceCount--;
        }

        // 一定間隔で全個体にメッセージング
        private async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            GptRequest request = new GptRequest(_key, RequestContents.Content());
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await RunAsync(request);
                    await UniTask.WaitForSeconds(_interval, cancellationToken: token);
                }
                catch(System.Exception e)
                {
                    Debug.LogWarning($"AIへのリクエストに失敗:\n {e.Message}");
                }
            }
        }

        // 各個体から報告された値を基にAIへリクエスト、レスポンスを基に各個体にメッセージングを行う。
        private async UniTask RunAsync(GptRequest request)
        {
            // AIに指示をリクエスト
            string t = RequestContents.Text(
                _fireSource.Source.Cum,
                _fireSource.Source.Hit,
                _exitSource.Source.Dead,
                _exitSource.Source.Escape,
                _exitSource.Source.Cum,
                _exitSource.Source.Min,
                _exitSource.Source.Max
                );
            string response = await request.RequestAsync(t);

            if (_debbuger != null) _debbuger.Response = response;

            // レスポンスを倍率の値に変換してメッセージング
            int[] values = ArrayPool<int>.Shared.Rent(RequestContents.QuestionCount);
            if (RequestContents.Convert(response, values))
            {
                LevelAdjustMessage msg = new LevelAdjustMessage
                {
                    // AIの評価値の範囲からメッセージングする構造体の値の範囲にリマップ
                    FireRate = RequestContents.Remap(LevelAdjustMessage.Min, LevelAdjustMessage.Max, values[0]),
                    MoveSpeed = RequestContents.Remap(LevelAdjustMessage.Min, LevelAdjustMessage.Max, values[1]),
                };

                MessageBroker.Default.Publish(msg);
            }
            Array.Clear(values, 0, values.Length);
            ArrayPool<int>.Shared.Return(values);
        }

        /// <summary>
        /// 発射した弾の結果を報告する。
        /// </summary>
        public static void FireReport(bool isHit)
        {
            MessageBroker.Default.Publish(new FireSource.Message
            { 
                IsHit = isHit
            });
        }

        /// <summary>
        /// 退場時に状態を報告する。
        /// </summary>
        public static void ExitReport(float lifeTime, bool isDead)
        {
            MessageBroker.Default.Publish(new ExitSource.Message 
            { 
                LifeTime = lifeTime,
                IsDead = isDead
            });
        }
    }
}

#endif