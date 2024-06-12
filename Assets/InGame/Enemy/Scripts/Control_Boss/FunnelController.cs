using Cysharp.Threading.Tasks;
using Enemy.Control.Boss;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

public class FunnelController : MonoBehaviour
{
    enum State
    {
        Closed,   // 未展開(デフォルト)
        Open,     // 展開中
        Defeated, // 撃破された
    }

    // ボス本体を登録するメッセージ
    struct Message
    {
        public BossController Boss;
        public List<FunnelController> Funnels;
    }

    private Transform _transform;
    private BossController _boss;
    // ボス本体を基準として展開するので、この値にボス本体の位置を足す。
    private Vector3 _openPoint;
    // 現在の状態
    private State _state = State.Closed;

    private void Awake()
    {
        _transform = transform;
        MessageReceive();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        // 展開中の場合はボス本体に追従する。
        if (_state == State.Open && _boss != null)
        {
            _transform.position = _boss.transform.position + _openPoint;
        }
    }

    /// <summary>
    /// ファンネルを展開するアニメーションを再生
    /// </summary>
    public void OpenAnimation()
    {
        OpenAnimationAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    // ボス本体の円状の周囲に展開するアニメーション。
    // NOTE:値は適当にベタ書き
    private async UniTask OpenAnimationAsync(CancellationToken token)
    {
        if (_boss == null) return;
        
        float sin = Mathf.Sin(2 * Mathf.PI * Random.value);
        float cos = Mathf.Cos(2 * Mathf.PI * Random.value);
        float dist = Random.Range(2.0f, 3.0f);
        float h = Random.Range(1.5f, 2.5f);
        _openPoint = new Vector3(cos * dist, h, sin * dist);
        
        Vector3 start = _transform.position;
        Vector3 target = _boss.transform.position + _openPoint;
        for (float t = 0; t <= 1; t += Time.deltaTime * 3)
        {
            if (token.IsCancellationRequested) return;

            _transform.position = Vector3.Lerp(start, target, t);
            await UniTask.Yield();
        }

        _transform.position = target;
        // 展開中に状態を変更
        _state = State.Open;
    }

    // メッセージの受信でボス本体を自身に登録。
    // 第二引数のリストに自身を追加して相互に参照させる。
    private void MessageReceive()
    {
        MessageBroker.Default.Receive<Message>().Subscribe(msg =>
        {
            _boss = msg.Boss;
            msg.Funnels.Add(this);
        }).AddTo(this);
    }

    /// <summary>
    /// 全ファンネルにボス本体を登録する。
    /// 第二引数のリストに自身を追加することで相互に参照させる。
    /// </summary>
    public static void RegisterOwner(BossController boss, List<FunnelController> funnels)
    {
        MessageBroker.Default.Publish(new Message { Boss = boss, Funnels = funnels });
    }
}
