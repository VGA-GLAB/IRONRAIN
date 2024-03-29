using UnityEngine;
using UniRx;

namespace Enemy.Control
{
    /// <summary>
    /// ���͋@��̕ω��ɑΉ������邽�߁A�v���C���[�̓��͂����̗񋓌^�ɕR�Â��A���M����B
    /// </summary>
    public enum PlayerActionMap
    {
        None,
        Attack,
    }

    /// <summary>
    /// �v���C���[�̓��͂�G�L�����N�^�[�ɋ��L���郁�b�Z�[�W�̍\����
    /// </summary>
    public struct PlayerInputMessage
    {
        public PlayerActionMap Map;
    }

    /// <summary>
    /// �v���C���[�̓��͂��Ď����A�G�L�����N�^�[�ɋ��L����B
    /// </summary>
    public class PlayerInputObserver : MonoBehaviour
    {
        private void Update()
        {
            // �L�[�{�[�h�ł�����ȊO�ł��Ή��\
            if (Input.GetKeyDown(KeyCode.Space)) Publish(PlayerActionMap.Attack);
        }

        // ���b�Z�[�W���O
        private void Publish(PlayerActionMap map)
        {
            MessageBroker.Default.Publish(new PlayerInputMessage { Map = map });
        }
    }
}
