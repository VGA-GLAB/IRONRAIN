using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.NPC
{
    public class RequiredRef : CharacterRequiredRef
    {
        public RequiredRef(Transform transform, Transform player, Transform offset, Transform rotate,
            BuddyNpcParams npcParams, BlackBoard blackBoard, Animator animator, Renderer[] renderers, 
            NpcEffects effects, Callback callback)
            : base(transform, player, offset, rotate, animator, renderers, null)
        {
            NpcParams = npcParams;
            BlackBoard = blackBoard;
            Effects = effects;
            Callback = callback;

            States = new Dictionary<StateKey, State<StateKey>>();
            Body = new Body(this);
            BodyAnimation = new BodyAnimation(this);
            Effector = new Effector(this);
        }

        public BuddyNpcParams NpcParams { get; }
        public BlackBoard BlackBoard { get; }
        public NpcEffects Effects { get; }
        public Callback Callback { get; }

        public Dictionary<StateKey, State<StateKey>> States { get; }
        public Body Body { get; }
        public BodyAnimation BodyAnimation { get; }
        public Effector Effector { get; }
    }
}
