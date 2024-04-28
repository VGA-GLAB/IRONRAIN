using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// �ڋߔ͈͓��Ƀv���C���[�������Ă��邩�𒲂ׂ�B
    /// </summary>
    public class ApproachSensor
    {
        private Transform _transform;
        private EnemyParams _params;

        public ApproachSensor(Transform transform, EnemyParams enemyParams)
        {
            _transform = transform;
            _params = enemyParams;
        }

        /// <summary>
        /// �l�̍X�V
        /// </summary>
        public void Update(BlackBoard blackBoard)
        {
            // ���g�ƃv���C���[�̋������ڋߊJ�n������
            if (blackBoard.TransformToPlayerDistance < _params.Advance.Distance)
            {
                blackBoard.IsPlayerDetected = true;
            }
        }

        /// <summary>
        /// �͈͂�`��
        /// </summary>
        public void DrawRange()
        {
            GizmosUtils.WireCircle(_transform.position, _params.Advance.Distance, ColorExtensions.ThinGreen);
        }
    }
}