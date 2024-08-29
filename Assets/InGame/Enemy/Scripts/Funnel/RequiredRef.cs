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
            BulletPool = BulletPool.Find();
        }

        public Transform Transform { get; }
        public Transform Player { get; }
        public Transform Offset { get; }
        public Transform Rotate { get; }
        public FunnelParams FunnelParams { get; }
        public BlackBoard BlackBoard { get; }
        public Animator Animator { get; }
        public Renderer[] Renderers { get; }
        public FunnelEffects Effects { get; }
        public Collider[] HitBoxes { get; }

        public Dictionary<StateKey, State<StateKey>> States { get; }
        public Body Body { get; }
        public BodyAnimation BodyAnimation { get; }
        public AnimationEvent AnimationEvent { get; }
        public Effector Effector { get; }
        public AgentScript AgentScript { get; }
        public BossController Boss { get; }
        public Transform BossRotate { get; }
        public Transform Muzzle { get; }
        public BulletPool BulletPool { get; }
    }
}
