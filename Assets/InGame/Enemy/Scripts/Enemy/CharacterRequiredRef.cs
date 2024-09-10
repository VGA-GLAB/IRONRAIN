using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class CharacterRequiredRef
    {
        public CharacterRequiredRef(Transform transform, Transform player, Transform offset, Transform rotate,
            Animator animator, Renderer[] renderers, Collider[] hitBoxes)
        {
            Transform = transform;
            Player = player;
            Offset = offset;
            Rotate = rotate;
            Animator = animator;
            Renderers = renderers;
            HitBoxes = hitBoxes;
        }

        public Transform Transform { get; }
        public Transform Player { get; }
        public Transform Offset { get; }
        public Transform Rotate { get; }
        public Animator Animator { get; }
        public Renderer[] Renderers { get; }
        public Collider[] HitBoxes { get; }
    }
}
