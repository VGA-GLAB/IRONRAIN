using UnityEngine;

namespace Enemy
{
    public class RequiredRef
    {
        public RequiredRef(Transform transform, Transform player, Transform offset, Transform rotate, 
            EnemyParams enemyParams, BlackBoard blackBoard, Animator animator, Transform[] models, 
            EnemyEffects effects, Collider[] hitBoxes, Equipment equipment)
        {
            Transform = transform;
            Player = player;
            Offset = offset;
            Rotate = rotate;
            EnemyParams = enemyParams;
            BlackBoard = blackBoard;
            Animator = animator;
            Models = models;
            Effects = effects;
            HitBoxes = hitBoxes;
            Equipment = equipment;
        }

        public Transform Transform { get; private set; }
        public Transform Player { get; private set; }
        public Transform Offset { get; private set; }   
        public Transform Rotate { get; private set; }
        public EnemyParams EnemyParams { get; private set; }
        public BlackBoard BlackBoard { get; private set; }
        public Animator Animator { get; private set; }
        public Transform[] Models { get; private set; }
        public EnemyEffects Effects { get; private set; }
        public Collider[] HitBoxes { get; private set; }
        public Equipment Equipment { get; private set; }
    }
}
