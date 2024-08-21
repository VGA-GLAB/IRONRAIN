using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Boss;

namespace Enemy.Funnel
{
    public class RequiredRef
    {
        public RequiredRef(Transform transform, Transform player, Transform offset, Transform rotate, FunnelParams funnelParams,
            BlackBoard blackBoard, Animator animator, Renderer[] renderers, FunnelEffects effects, Collider[] hitBoxes,
            BossController boss, Transform muzzle)
        {
            Transform = transform;
            Player = player;
            Offset = offset;
            Rotate = rotate;
            FunnelParams = funnelParams;
            BlackBoard = blackBoard;
            Animator = animator;
            Renderers = renderers;
            Effects = effects;
            HitBoxes = hitBoxes;

            States = new Dictionary<StateKey, State<StateKey>>();
            Body = new Body(this);
            BodyAnimation = new BodyAnimation(this);
            AnimationEvent = Animator.GetComponent<AnimationEvent>();
            Effector = new Effector(this);
            AgentScript = transform.GetComponent<AgentScript>();
            Boss = boss;
            BossRotate = boss.FindRotate();
            Muzzle = muzzle;
        }

        public Transform Transform { get; private set; }
        public Transform Player { get; private set; }
        public Transform Offset { get; private set; }
        public Transform Rotate { get; private set; }
        public FunnelParams FunnelParams { get; private set; }
        public BlackBoard BlackBoard { get; private set; }
        public Animator Animator { get; private set; }
        public Renderer[] Renderers { get; private set; }
        public FunnelEffects Effects { get; private set; }
        public Collider[] HitBoxes { get; private set; }

        public Dictionary<StateKey, State<StateKey>> States { get; private set; }
        public Body Body { get; private set; }
        public BodyAnimation BodyAnimation { get; private set; }
        public AnimationEvent AnimationEvent { get; private set; }
        public Effector Effector { get; private set; }
        public AgentScript AgentScript { get; private set; }
        public BossController Boss { get; set; }
        public Transform BossRotate { get; set; }
        public Transform Muzzle { get; private set; }
    }
}
