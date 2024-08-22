using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class RequiredRef
    {
        public RequiredRef(Transform transform, Transform player, Transform offset, Transform rotate, 
            EnemyParams enemyParams, BlackBoard blackBoard, Animator animator, Renderer[] renderers, 
            EnemyEffects effects, Collider[] hitBoxes, Equipment equipment)
        {
            Transform = transform;
            Player = player;
            Offset = offset;
            Rotate = rotate;
            EnemyParams = enemyParams;
            BlackBoard = blackBoard;
            Animator = animator;
            Renderers = renderers;
            Effects = effects;
            HitBoxes = hitBoxes;
            Equipment = equipment;

            States = new Dictionary<StateKey, State<StateKey>>();
            Body = new Body(this);
            BodyAnimation = new BodyAnimation(this);
            Effector = new Effector(this);
            AgentScript = transform.GetComponent<AgentScript>();
        }

        public Transform Transform { get; private set; }
        public Transform Player { get; private set; }
        public Transform Offset { get; private set; }   
        public Transform Rotate { get; private set; }
        public EnemyParams EnemyParams { get; private set; }
        public BlackBoard BlackBoard { get; private set; }
        public Animator Animator { get; private set; }
        public Renderer[] Renderers { get; private set; }
        public EnemyEffects Effects { get; private set; }
        public Collider[] HitBoxes { get; private set; }
        public Equipment Equipment { get; private set; }

        public Dictionary<StateKey, State<StateKey>> States { get; private set; }
        public Body Body { get; private set; }
        public BodyAnimation BodyAnimation { get; private set; }
        public Effector Effector { get; private set; }
        public AgentScript AgentScript { get; private set; }
    }
}
