using UnityEngine;

/// <summary>MonoBehaviourを継承したシングルトン</summary>
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>インスタンス</summary>
    private static T _instance;

    /// <summary>インスタンスのプロパティ</summary>
    public static T Instance
    {
        get
        {
            // インスタンスがnullの場合
            if (_instance == null)
            {
                // CriSoundManagerオブジェクトがシーン上に存在しない場合
                if (FindObjectOfType<T>() == null)
                {
                    // 新しいゲームオブジェクトを生成し、CriSoundManagerを付与する
                    GameObject singleton = new GameObject();
                    singleton.name = typeof(T).ToString();
                    _instance = singleton.AddComponent<T>();

                    // シーン変更時に破棄しないようにする
                    DontDestroyOnLoad(singleton);
                }
                // CriSoundManagerオブジェクトがシーン上に存在するがインスタンスがnullの場合
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