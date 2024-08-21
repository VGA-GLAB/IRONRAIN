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
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    GameObject singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = typeof(T).ToString() + " (Singleton)";

                    // シーン変更時に破棄しないようにする
                    DontDestroyOnLoad(singleton);
                }    
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;  
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}