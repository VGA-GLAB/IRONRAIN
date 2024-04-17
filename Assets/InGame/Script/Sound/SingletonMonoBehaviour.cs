using UnityEngine;

/// <summary>MonoBehaviour���p�������V���O���g��</summary>
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>�C���X�^���X</summary>
    private static T _instance;

    /// <summary>�C���X�^���X�̃v���p�e�B</summary>
    public static T Instance
    {
        get
        {
            // �C���X�^���X��null�̏ꍇ
            if (_instance == null)
            {
                // CriSoundManager�I�u�W�F�N�g���V�[����ɑ��݂��Ȃ��ꍇ
                if (FindObjectOfType<T>() == null)
                {
                    // �V�����Q�[���I�u�W�F�N�g�𐶐����ACriSoundManager��t�^����
                    GameObject singleton = new GameObject();
                    singleton.name = typeof(T).ToString();
                    _instance = singleton.AddComponent<T>();

                    // �V�[���ύX���ɔj�����Ȃ��悤�ɂ���
                    DontDestroyOnLoad(singleton);
                }
                // CriSoundManager�I�u�W�F�N�g���V�[����ɑ��݂��邪�C���X�^���X��null�̏ꍇ
                _instance = FindObjectOfType<T>();
            }

            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }
}