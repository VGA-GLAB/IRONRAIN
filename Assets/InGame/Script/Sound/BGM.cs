using UnityEngine;

/// <summary>BGMを管理するクラス</summary>
public class BGM : SingletonMonoBehaviour<BGM>
{
    /// <summary>BGMの音量</summary>
    [SerializeField, Range(0, 2f)] private float _volume = 1.0f;

    /// <summary>BGMの列挙型</summary>
    private enum BGMID
    {
        /// <summary>格納庫1(環境音)</summary>
        BGM_Hanger_Ambient_01,

        /// <summary>格納庫1(BGM)</summary>
        BGM_Hanger_01,

        /// <summary>格納庫2(環境音)</summary>
        BGM_Hanger_Ambient_02,

        /// <summary>格納庫2(BGM)</summary>
        BGM_Hanger_02,

        /// <summary>月面フィールド(環境音)</summary>
        BGM_MoonField_Ambient,

        /// <summary>月面フィールド(BGM)</summary>
        BGM_MoonField,

        /// <summary>ボス戦(環境音)</summary>
        BGM_BossBattle_Ambient,

        /// <summary>ボス戦(BGM)</summary>
        BGM_BossBattle,

        /// <summary>ボス戦/近接戦(環境音)</summary>
        BGM_BossBattle_Close_Ambient,

        /// <summary>ボス戦/近接戦(BGM)</summary>
        BGM_BossBattle_Close,

        /// <summary>勝利(環境音)</summary>
        BGM_Win_Ambient,

        /// <summary>勝利(BGM)</summary>
        BGM_Win
    }

    /// <summary>引数で指定したBGMを再生する</summary>
    /// <param name="id">BGMの列挙型のインデックス</param>
    public void PlayBGM(int id)
    {
        CriAudioManager.Instance.BGM.Play("BGM", ((BGMID)id).ToString(), _volume);
    }
}
