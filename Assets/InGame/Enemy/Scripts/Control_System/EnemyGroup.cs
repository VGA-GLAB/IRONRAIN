using Enemy.DebugUse;
using Enemy.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Control
{
    [ExecuteAlways]
    public class EnemyGroup : MonoBehaviour
    {
        private GameObject _parent;
        private Dictionary<EnemyManager.Sequence, List<EnemyController>> _enemies;
        private Dictionary<EnemyManager.Sequence, List<INpc>> _npcs;

        // 数フレーム連続で分類する処理が呼ばれないように、ある程度間隔を設ける。
        private float _lastTime;

        private void Update()
        {
            // ゲーム起動中は敵は位置をしない。
            if (Application.isPlaying) return;

            // なるぽ防止
            _enemies ??= new Dictionary<EnemyManager.Sequence, List<EnemyController>>();
            _npcs ??= new Dictionary<EnemyManager.Sequence, List<INpc>>();

            if (_parent == null)
            {
                // 名前でシーン上を検索。
                const string Name = "Enemy";

                _parent = GameObject.Find(Name);
                if (_parent == null)
                {
                    Debug.LogError($"シーン上の敵の親オブジェクトの名前が違う:{Name}");
                }
            }
            else
            {
                // 分類処理を連続で呼ばないために適当に間隔を測る。
                if (_lastTime + .5f < Time.time)
                {
                    _lastTime = Time.time;
                }
                else return;

                // 重複して追加しないように一度クリアしておく。
                foreach(List<EnemyController> e in _enemies.Values) e.Clear();
                foreach (List<INpc> n in _npcs.Values) n.Clear();

                // 子を分類して追加
                foreach (Transform child in _parent.transform)
                {
                    AddEnemy(child);
                    AddNpc(child);
                }
            }
        }

        private void OnDrawGizmos()
        {
            DrawEnemyGroup();
            DrawNpcGroup();
        }

        // 敵を分類して追加
        private void AddEnemy(Transform child)
        {
            if (!child.TryGetComponent(out EnemyController e)) return;

            EnemyManager.Sequence seq = e.Params.Sequence;

            if (!_enemies.ContainsKey(seq))
            {
                _enemies.Add(seq, new List<EnemyController>());
            }

            _enemies[seq].Add(e);
        }

        // NPCを分類して追加
        private void AddNpc(Transform child)
        {
            if (!child.TryGetComponent(out INpc n)) return;

            if (!_npcs.ContainsKey(n.Sequence))
            {
                _npcs.Add(n.Sequence, new List<INpc>());
            }

            _npcs[n.Sequence].Add(n);
        }

        // 同じシーケンスの敵同士を線で結ぶ
        private void DrawEnemyGroup()
        {
            if (_enemies == null) return;

            foreach (List<EnemyController> enemies in _enemies.Values)
            {
                if (enemies == null) continue;

                for (int i = 0; i < enemies.Count - 1; i++)
                {
                    GizmosUtils.Line(
                        enemies[i].transform.position,
                        enemies[i + 1].transform.position,
                        ColorExtensions.ThinCyan
                        );
                }

                GizmosUtils.Line(
                    enemies[0].transform.position, 
                    enemies[^1].transform.position, 
                    ColorExtensions.ThinCyan
                    );
            }
        }

        // 同じシーケンスのNPC同士を線で結ぶ
        private void DrawNpcGroup()
        {
            if(_npcs == null) return;

            foreach (List<INpc> npcs in _npcs.Values)
            {
                if (npcs == null) continue;

                for (int i = 0; i < npcs.Count - 1; i++)
                {
                    GizmosUtils.Line(
                        npcs[i].GameObject.transform.position,
                        npcs[i + 1].GameObject.transform.position,
                        ColorExtensions.ThinCyan
                        );
                }

                GizmosUtils.Line(
                    npcs[0].GameObject.transform.position,
                    npcs[^1].GameObject.transform.position,
                    ColorExtensions.ThinCyan
                    );
            }
        }
    }
}