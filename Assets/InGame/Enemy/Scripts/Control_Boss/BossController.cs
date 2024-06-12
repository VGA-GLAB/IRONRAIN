using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Enemy.Control.Boss
{
    /// <summary>
    /// ボスを操作する。
    /// 一時的に画面から消して動作を止めたい場合: xxx
    /// これ以上動かさず、削除する場合: xxx
    /// </summary>
    public class BossController : MonoBehaviour
    {
        [Header("プランナーが弄る値")]
        [SerializeField] private BossParams _params;
        [Header("自身やプレハブへの参照")]
        [SerializeField] private Transform _offset;
        [SerializeField] private Transform _rotate;
        [SerializeField] private Renderer[] _renderers;

        // 注入する依存関係
        private Transform _pointP;
        private Transform _player;
        // 自身の状態や周囲を認識して黒板に書き込むPerception層。
        private Perception _perception;
        private OverrideOrder _overrideOrder;
        // 黒板に書き込まれた内容から次にとるべき行動を決定するBrain層。
        private UtilityEvaluator _utilityEvaluator;
        private BehaviorTree _behaviorTree;
        // 行動をオブジェクトに反映してキャラクターを動かすAction層。
        private BodyController _bodyController;
        // 各層で値を読み書きする用の黒板。
        private BlackBoard _blackBoard;

        private List<FunnelController> _funnels;

        // 非表示にする非同期処理を実行中フラグ。
        // 二重に処理を呼ばないために必要。
        private bool _isCleanupRunning;

        // Transform2つだとVContainerでエラーが出るので、デバッグ用のクラスで注入している。
        [Inject]
        private void Construct(DebugPointP pointP, Transform player)
        {
            _pointP = pointP.transform;
            _player = player;
        }

        private void Awake()
        {
            // 依存関係をチェック
            if (_pointP == null || _player == null)
            {
                Debug.LogWarning($"依存関係の構築に失敗: PointP:{_pointP}, Player:{_player}");
            }

            _funnels = new List<FunnelController>();
            _blackBoard = new BlackBoard();

            // Perception
            _perception = new Perception(transform, _blackBoard, _player, _pointP);
            _overrideOrder = new OverrideOrder(_blackBoard);
            // Brain
            _utilityEvaluator = new UtilityEvaluator(_params, _blackBoard);
            _behaviorTree = new BehaviorTree(transform, _params, _blackBoard);
            // Action
            _bodyController = new BodyController(transform, _blackBoard, _offset, _rotate, _renderers, _funnels);
        }

        private void Start()
        {
            EnemyManager.Register(this);

            // ファンネルと相互に参照させる。
            FunnelController.RegisterOwner(this, _funnels);

            // レーダーに表示する。
            if (TryGetComponent(out AgentScript a)) a.EnemyGenerate();

            // Perception
            _perception.Init();
        }

        private void Update()
        {
            // Perception
            _perception.Update();
            _overrideOrder.Update();
            
            // Brain
            IReadOnlyList<Choice> eval = _utilityEvaluator.Evaluate();
            for (int i = 0; i < eval.Count; i++)
            {
                _behaviorTree.Run(eval[i]);
            }

            // Action
            // オブジェクトに諸々を反映させているので結果をハンドリングする。
            // 完了が返ってきた場合は、続けて後始末処理を呼び出す。
            // OnPreCleanup -> LateUpdate -> 次フレームのUpdate -> 非表示 の順で呼ばれる。
            if (_bodyController.Update() == BodyController.Result.Complete && !_isCleanupRunning)
            {
                _isCleanupRunning = true;
                StartCoroutine(CleanupAsync());
            }
        }

        private void LateUpdate()
        {
            // Perception
            _overrideOrder.ClearOrderedTrigger();

            // Brain
            _behaviorTree.ClearBlackBoardWritedValues();
        }

        // 後始末、Update内から呼び出す。
        private IEnumerator CleanupAsync()
        {
            // レーダーから消す。
            if (TryGetComponent(out AgentScript a)) a.EnemyGenerate();

            // Perception
            _perception.Dispose();

            // Action
            _bodyController.Dispose();

            // 次フレームのUpdateの後まで待つ。
            yield return null;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            // 登録解除は死亡したタイミングではなく、ゲームが終了するタイミングになっている。
            // 死亡した敵かの判定が出来るようにするため。
            EnemyManager.Release(this);

            // Perception
            _perception.Dispose();

            // Action
            _bodyController.Dispose();
        }

        private void OnDrawGizmos()
        {
            _perception?.Draw();
        }

        /// <summary>
        /// 外部から敵の行動を制御する。
        /// /// </summary>
        public void Order(EnemyOrder order)
        {
            _overrideOrder.Buffer(order);
        }
    }
}
