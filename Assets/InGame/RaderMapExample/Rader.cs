using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RaderMapExample
{
    public class Enemy
    {
        public GameObject EnemyObject;
        public Transform EnemyTransform;
        public AgentScript AgentScript;
    }

    public class Rader : MonoBehaviour
    {
        [SerializeField] private float _radius = 9.0f;

        private Collider[] _result;
        private Transform _transform;

        // レーダーに捉えられる限界、適当。
        public int Capacity => 30;

        private void Start()
        {
            _result = new Collider[Capacity];
            _transform = transform;
        }

        private void Update()
        {
            _result = Physics.OverlapSphere(_transform.position, _radius);
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            // レーダーの範囲
            Gizmos.color = new Color(1, 0, 0, 0.1f);
            Gizmos.DrawSphere(GetCenter(), _radius);

            // レーダーに捉えた対象
            Gizmos.color = Color.red;
            foreach (Collider c in _result)
            {
                Vector3 p = c.transform.position;
                Gizmos.DrawCube(p, Vector3.one);
            }
        }

        /// <summary>
        /// レーダーの中心位置を返す。
        /// </summary>
        public Vector3 GetCenter()
        {
            return _transform.position;
        }

        /// <summary>
        ///　敵の情報を取ってくる
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Enemy> GetEnemy()
        {
            foreach(Collider c in  _result)
            {
                if (c == null) yield break;

                if(c.TryGetComponent<AgentScript>(out var agent))
                {
                    var enemy = new Enemy();
                    enemy.AgentScript = agent;
                    enemy.EnemyTransform = agent.gameObject.transform;
                }
            }
        }

        /// <summary>
        /// 補足した対象ごとに、レーダーの中心位置からのベクトルを返す。
        /// </summary>
        public IEnumerable<Vector3> GetResultVec3()
        {
            foreach (Collider c in _result)
            {
                if (c == null) yield break;

                Vector3 a = c.transform.position;
                a.y = 0;
                Vector3 b = GetCenter();
                b.y = 0;

                yield return a - b;
            }
        }

        /// <summary>
        /// 補足した対象ごとに、レーダーの中心位置からのベクトルを返す。
        /// UIに表示させるよう、XY平面上のベクトルに加工してある。
        /// </summary>
        public IEnumerable<Vector2> GetResultVec2()
        {
            foreach (Vector3 v in GetResultVec3())
            {
                yield return new Vector2(v.x, v.z);
            }
        }
    }
}
