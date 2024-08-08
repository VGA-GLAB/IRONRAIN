using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss
{
    public class RequiredRef
    {
        public RequiredRef(Transform transform, Transform player, Transform offset, Transform rotate,
            BossParams bossParams, BlackBoard blackBoard, Animator animator, Transform[] models, BossEffects effects, 
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
            Models = models;
            Effects = effects;
            HitBoxes = hitBoxes;
            MeleeEquip = meleeEquip;
            RangeEquip = rangeEquip;
            Funnels = funnels;
            PointP = pointP;
        }

        public Transform Transform { get; private set; }
        public Transform Player { get; private set; }
        public Transform Offset { get; private set; }
        public Transform Rotate { get; private set; }
        public BossParams BossParams { get; private set; }
        public BlackBoard BlackBoard { get; private set; }
        public Animator Animator { get; private set; }
        public Transform[] Models { get; private set; }
        public BossEffects Effects { get; private set; }
        public Collider[] HitBoxes { get; private set; }
        public MeleeEquipment MeleeEquip { get; private set; }
        public RangeEquipment RangeEquip { get; private set; }
        public List<FunnelController> Funnels { get; private set; }
        public DebugPointP PointP { get; private set; }
    }
}
