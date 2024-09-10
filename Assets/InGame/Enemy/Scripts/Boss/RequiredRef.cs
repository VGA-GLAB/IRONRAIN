using System.Collections.Generic;
using UnityEngine;
using Enemy.Funnel;

namespace Enemy.Boss
{
    public class RequiredRef : CharacterRequiredRef
    {
        public RequiredRef(Transform transform, Transform player, Transform offset, Transform rotate, Transform pointP,
            BossParams bossParams, BlackBoard blackBoard, Animator animator, Renderer[] renderers, BossEffects effects,
            Collider[] hitBoxes, MeleeEquipment meleeEquip, RangeEquipment rangeEquip, List<FunnelController> funnels)
            : base(transform, player, offset, rotate, animator, renderers, hitBoxes)
        {
            BossParams = bossParams;
            BlackBoard = blackBoard;
            Effects = effects;
            MeleeEquip = meleeEquip;
            RangeEquip = rangeEquip;
            Funnels = funnels;

            States = new Dictionary<StateKey, State<StateKey>>();
            Body = new Body(this);
            BodyAnimation = new BodyAnimation(this);
            AnimationEvent = Animator.GetComponent<AnimationEvent>();
            Effector = new Effector(this);
            AgentScript = transform.GetComponent<AgentScript>();
            Field = new Field(this, pointP);
        }

        public BossParams BossParams { get; }
        public BlackBoard BlackBoard { get; }
        public BossEffects Effects { get; }
        public MeleeEquipment MeleeEquip { get; }
        public RangeEquipment RangeEquip { get; }
        public List<FunnelController> Funnels { get; }

        public Dictionary<StateKey, State<StateKey>> States { get; }
        public Body Body { get; }
        public BodyAnimation BodyAnimation { get; }
        public AnimationEvent AnimationEvent { get; }
        public Effector Effector { get; }
        public AgentScript AgentScript { get; }
        public Field Field { get; }
    }
}
