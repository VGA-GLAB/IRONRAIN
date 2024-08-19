using System.Collections.Generic;
using UnityEngine;
using Enemy.Boss.FSM;

namespace Enemy.Boss
{
    public class RequiredRef
    {
        public RequiredRef(Transform transform, Transform player, Transform offset, Transform rotate,
            BossParams bossParams, BlackBoard blackBoard, Animator animator, Renderer[] renderers, BossEffects effects, 
            Collider[] hitBoxes, MeleeEquipment meleeEquip, RangeEquipment rangeEquip, List<FunnelController> funnels,
            DebugPointP pointP)
        {
            Transform = transform;
            Player = player;
            Offset = offset;
            Rotate = rotate;
            BossParams = bossParams;
            BlackBoard = blackBoard;
            Animator = animator;
            Renderers = renderers;
            Effects = effects;
            HitBoxes = hitBoxes;
            MeleeEquip = meleeEquip;
            RangeEquip = rangeEquip;
            Funnels = funnels;
            PointP = pointP;

            States = new Dictionary<StateKey, State>();
            Body = new Body(this);
            BodyAnimation = new BodyAnimation(this);
            AnimationEvent = Animator.GetComponent<AnimationEvent>();
            Effector = new Effector(this);
            AgentScript = transform.GetComponent<AgentScript>();
        }

        public Transform Transform { get; private set; }
        public Transform Player { get; private set; }
        public Transform Offset { get; private set; }
        public Transform Rotate { get; private set; }
        public BossParams BossParams { get; private set; }
        public BlackBoard BlackBoard { get; private set; }
        public Animator Animator { get; private set; }
        public Renderer[] Renderers { get; private set; }
        public BossEffects Effects { get; private set; }
        public Collider[] HitBoxes { get; private set; }
        public MeleeEquipment MeleeEquip { get; private set; }
        public RangeEquipment RangeEquip { get; private set; }
        public List<FunnelController> Funnels { get; private set; }
        public DebugPointP PointP { get; private set; }

        public Dictionary<StateKey, State> States { get; private set; }
        public Body Body { get; private set; }
        public BodyAnimation BodyAnimation { get; private set; }
        public AnimationEvent AnimationEvent { get; private set; }
        public Effector Effector { get; private set; }
        public AgentScript AgentScript { get; private set; }
    }
}
