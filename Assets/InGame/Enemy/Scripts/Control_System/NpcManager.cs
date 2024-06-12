using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Enemy.Control
{
    public class NpcManager : MonoBehaviour
    {
        /// <summary>
        /// シーケンス単位での制御をする際に使用。
        /// 命名はChaseSequenceControllerクラスにシリアライズされているそれに準じている。
        /// </summary>
        public enum Sequence
        {
            None,
            MultiBattle, // 追跡:乱戦中に味方機が登場。
        }

        // シーン上のNPCを登録する用のメッセージ。
        private struct RegisterMessage
        {
            public MultiBattleSequenceNpc NPC;
        }

        // 現状、NPC登場のイベントが1つしかないため、それ用にしている。
        private List<MultiBattleSequenceNpc> _multiBattleSequenceNPCs;

        void Awake()
        {
            _multiBattleSequenceNPCs = new List<MultiBattleSequenceNpc>();

            // メッセージングで敵とNPCを登録/登録解除する。
            MessageBroker.Default.Receive<RegisterMessage>()
                .Subscribe(msg => 
                {
                    _multiBattleSequenceNPCs.Add(msg.NPC);
                }).AddTo(this);
        }

        /// <summary>
        /// シーケンスを指定してイベントを実行
        /// </summary>
        public void PlayEvent(Sequence sequence)
        {
            // 乱戦中に味方機が登場イベント
            if (sequence == Sequence.MultiBattle)
            {
                foreach (MultiBattleSequenceNpc npc in _multiBattleSequenceNPCs)
                {
                    npc.gameObject.SetActive(true);
                    npc.Play();
                }
            }
        }

        /// <summary>
        /// 敵を登録する。
        /// </summary>
        public static void Register(MultiBattleSequenceNpc npc)
        {
            MessageBroker.Default.Publish(new RegisterMessage { NPC = npc });
        }
    }
}
