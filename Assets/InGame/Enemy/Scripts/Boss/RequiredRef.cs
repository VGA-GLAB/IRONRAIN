using System.Collections.Generic;
using UnityEngine;
using Enemy.Funnel;
using IronRain.Player;

namespace Enemy.Boss
{
    public class RequiredRef
    {
        public RequiredRef(Transform transform, Transform player, Transform offset, Transform rotate, Transform pointP,
            BossParams bossParams, BlackBoard blackBoard, Animator animator, Renderer[] renderers, BossEffects effects, 
            Collider[] hitBoxes, MeleeEquipment meleeEquip, RangeEquipment rangeEquip, List<FunnelController> funnels)
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

            States = new Dictionary<StateKey, State<StateKey>>();
            Body = new Body(this);
            BodyAnimation = new BodyAnimation(this);
            AnimationEvent = Animator.GetComponent<AnimationEvent>();
            Effector = new Effector(this);
            AgentScript = transform.GetComponent<AgentScript>();
            Field = new Field(this, pointP);

            // ↓これをCallbackくらすで囲って外出し。
            AnimationEvent.OnBreakLeftArm += () => Debug.Log("左腕破壊コールバック呼び出し");
        }

        public Transform Transform { get; }
        public Transform Player { get; }
        public Transform Offset { get; }
        public Transform Rotate { get; }
        public BossParams BossParams { get; }
        public BlackBoard BlackBoard { get; }
        public Animator Animator { get; }
        public Renderer[] Renderers { get; }
        public BossEffects Effects { get; }
        public Collider[] HitBoxes { get; }
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
