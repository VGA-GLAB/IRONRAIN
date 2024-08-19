﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HM
{
    public class Patrol : MonoBehaviour, IDamageable
    {
        [SerializeField] float _speed = 1.0f;
        [SerializeField] Transform[] _waypoints;

        void Start()
        {
            StartCoroutine(ForeverAsync());
        }

        IEnumerator ForeverAsync()
        {
            while (true) yield return PatrolAsync();
        }

        IEnumerator PatrolAsync()
        {
            for (int i = 0; i < _waypoints.Length; i++)
            {
                yield return PatrolAsync(i, (i + 1) % _waypoints.Length);
            }
        }

        IEnumerator PatrolAsync(int a, int b)
        {
            Vector3 from = _waypoints[a].position;
            Vector3 to = _waypoints[b].position;
            for (float t = 0; t < 1.0f; t += Time.deltaTime * _speed)
            {
                transform.position = Vector3.Lerp(from, to, t);
                yield return null;
            }

            transform.position = _waypoints[b].position;
        }

        private void OnDrawGizmos()
        {
            for (int i = 0; i < _waypoints.Length; i++)
            {
                Vector3 from = _waypoints[i].position;
                Vector3 to = _waypoints[(i + 1) % _waypoints.Length].position;

                Gizmos.color = new Color(1, 1, 1, 0.2f);
                Gizmos.DrawLine(from, to);
            }
        }

        public void Damage(int value, string weapon = "")
        {
            Debug.Log($"{name}: {weapon}で{value}のダメージを受けた。");
        }
    }
}