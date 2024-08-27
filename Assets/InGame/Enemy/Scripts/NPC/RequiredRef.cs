using Enemy.Boss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.NPC
{
    public class RequiredRef
    {
        public RequiredRef(Transform transform, Transform player, Transform offset, Transform rotate,
            BuddyNpcParams npcParams, BlackBoard blackBoard, Animator animator, Renderer[] renderers, 
            NpcEffects effects)
        {
            Transform = transform;
            Player = player;
            Offset = offset;
            Rotate = rotate;
            NpcParams = npcParams;
            BlackBoard = blackBoard;
            Animator = animator;
            Renderers = renderers;
            Effects = effects;
            HitBoxes = null;

            States = new Dictionary<StateKey, State<StateKey>>();
            Body = new Body(this);
            Effector = new Effector(this);
        }

        public Transform Transform { get; }
        public Transform Player { get; }
        public Transform Offset { get; }
        public Transform Rotate { get; }
        public BuddyNpcParams NpcParams { get; }
        public BlackBoard BlackBoard { get; }
        public Animator Animator { get; }
        public Renderer[] Renderers { get; }
        public NpcEffects Effects { get; }
        public Collider[] HitBoxes { get; }

        public Dictionary<StateKey, State<StateKey>> States { get; }
        public Body Body { get; }
        public Effector Effector { get; }
    }
}
