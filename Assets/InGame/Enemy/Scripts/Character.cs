using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    // 雑魚、ボス、NPC、ファンネルで共通した基底クラス。
    // オブジェクトが以下のような構成になっている前提。
    // root
    //  Offset
    //   Rotate
    //    3Dモデル
    //   NoRotate
    public class Character : MonoBehaviour
    {
        [SerializeField] protected Renderer[] _renderers;

        // ↑これの参照を取得するためのメソッド。
        [ContextMenu("3DモデルのRendererへの参照を取得")]
        private void GetRendererAll()
        {
            List<Renderer> r = new List<Renderer>();
            foreach (Renderer sm in GetComponentsInChildren<SkinnedMeshRenderer>()) r.Add(sm);
            foreach (Renderer m in GetComponentsInChildren<MeshRenderer>()) r.Add(m);
            _renderers = r.ToArray();

            foreach (var v in _renderers) Debug.Log($"{name}: {v}");
        }

        public Transform FindOffset() => transform.FindChildRecursive("Offset");
        public Transform FindRotate() => transform.FindChildRecursive("Rotate");
        protected Transform FindPlayer() => GameObject.FindGameObjectWithTag(Const.PlayerTag).transform;
    }
}
