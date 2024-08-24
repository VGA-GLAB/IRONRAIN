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

        public Transform Transform { get; }
        public Transform Player { get; }
        public Transform Offset { get; }   
        public Transform Rotate { get; }
        public EnemyParams EnemyParams { get; }
        public BlackBoard BlackBoard { get; }
        public Animator Animator { get; }
        public Renderer[] Renderers { get; }
        public EnemyEffects Effects { get; }
        public Collider[] HitBoxes { get; }
        public Equipment Equipment { get; }

        public Dictionary<StateKey, State<StateKey>> States { get; }
        public Body Body { get; }
        public BodyAnimation BodyAnimation { get; }
        public Effector Effector { get; }
        public AgentScript AgentScript { get; }
    }
}
