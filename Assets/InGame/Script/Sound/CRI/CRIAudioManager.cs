public class CriAudioManager
{
    private static CriAudioManager _instance;
    
    public static CriAudioManager Instance
    {
        get
        {
            _instance ??= new CriAudioManager();
            return _instance;
        }
    }

    private CriAudioManager()
    {
        _masterVolume = new Volume();
        _bgm = new (_masterVolume);
        _bgmAmbient = new(_masterVolume);
        _se = new (_masterVolume);
        _cockpitSE = new(_masterVolume);
        _voice = new (_masterVolume);
    }

    #region チャンネル

    // マスターのボリューム
    private Volume _masterVolume;

    // BGMを流すチャンネル
    private CriSingleChannel _bgm;

    // BGMAmbientを流すチャンネル
    private CriSingleChannel _bgmAmbient;

    // SEを流すチャンネル
    private CriMultiChannel _se;
    
    // コックピットのSEを流すチャンネル
    private CriMultiChannel _cockpitSE;

    // Voiceを流すチャンネル
    private CriSingleChannel _voice;

    #endregion

    #region プロパティ

    /// <summary>AmbientBGMを流すチャンネル</summary>
    public ICustomChannel AmbientBGM => _bgmAmbient;

    /// <summary>SEのチャンネル</summary>
    public ICustomChannel SE => _se;
    
    /// <summary>コックピットのSEのチャンネル</summary>
    public ICustomChannel CockpitSE => _cockpitSE;
    
    /// <summary>Voiceのチャンネル</summary>
    public ICustomChannel Voice => _voice;

    /// <summary>マスターボリューム</summary>
    public IVolume MasterVolume => _masterVolume;
    
    /// <summary>BGMのチャンネル</summary>
    public ICustomChannel BGM => _bgm;

    #endregion
}