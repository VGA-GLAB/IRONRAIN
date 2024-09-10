using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class RequiredRef : CharacterRequiredRef
    {
        public RequiredRef(Transform transform, Transform player, Transform offset, Transform rotate,
            EnemyParams enemyParams, BlackBoard blackBoard, Animator animator, Renderer[] renderers,
            EnemyEffects effects, Collider[] hitBoxes, Equipment equipment, Collider qteTrigger)
            : base(transform, player, offset, rotate, animator, renderers, hitBoxes)
        {
            EnemyParams = enemyParams;
            BlackBoard = blackBoard;
            Effects = effects;
            Equipment = equipment;
            QteTrigger = qteTrigger;

            States = new Dictionary<StateKey, State<StateKey>>();
            Body = new Body(this);
            BodyAnimation = new BodyAnimation(this);
            Effector = new Effector(this);
            AgentScript = transform.GetComponent<AgentScript>();
        }

        public EnemyParams EnemyParams { get; }
        public BlackBoard BlackBoard { get; }
        public EnemyEffects Effects { get; }
        public Equipment Equipment { get; }
        public Collider QteTrigger { get; }

        public Dictionary<StateKey, State<StateKey>> States { get; }
        public Body Body { get; }
        public BodyAnimation BodyAnimation { get; }
        public Effector Effector { get; }
        public AgentScript AgentScript { get; }
    }
}
