using UnityEngine;

public static class AudioHelper
{
    private static GameObject audioObject;
    private static AudioSource audioSource;

    // Audio Clips
    public static AudioClip InitClip { get; private set; }
    public static AudioClip BattleClip { get; private set; }
    public static AudioClip DieClip { get; private set; }
    public static AudioClip GameFinishClip { get; private set; } // 새로운 오디오 클립 추가

    // 현재 재생 중인 클립
    private static AudioClip currentClip;

    /// <summary>
    /// 오디오 재생을 초기화합니다. InitClip, BattleClip, DieClip, GameFinishClip을 설정해야 합니다.
    /// 이 메서드는 씬 당 하나만 호출되어야 합니다.
    /// </summary>
    /// <param name="initClip">Init 오디오 클립</param>
    /// <param name="battleClip">Battle 오디오 클립</param>
    /// <param name="dieClip">Die 오디오 클립</param>
    /// <param name="gameFinishClip">Game Finish 오디오 클립</param>
    public static void Initialize(AudioClip initClip, AudioClip battleClip, AudioClip dieClip, AudioClip gameFinishClip)
    {
        if (audioObject != null)
        {
            Debug.LogWarning("AudioHelper is already initialized.");
            return;
        }

        if (initClip == null || battleClip == null || dieClip == null || gameFinishClip == null)
        {
            Debug.LogError("AudioHelper initialization failed: One or more AudioClips are not assigned.");
            return;
        }

        // 새로운 GameObject 생성 및 AudioSource 추가
        audioObject = new GameObject("AudioHelperObject");
        audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.loop = true; // Init, Battle 클립은 반복 재생

        // DontDestroyOnLoad를 명시적으로 참조
        Object.DontDestroyOnLoad(audioObject);

        // AudioClips 할당
        InitClip = initClip;
        BattleClip = battleClip;
        DieClip = dieClip;
        GameFinishClip = gameFinishClip; // 새로운 오디오 클립 할당

        Debug.Log("AudioHelper initialized successfully.");
    }

    /// <summary>
    /// 'init' 오디오 클립을 재생합니다.
    /// </summary>
    public static void PlayInitAudio()
    {
        if (audioSource == null || InitClip == null)
        {
            Debug.LogError("AudioHelper is not initialized properly.");
            return;
        }

        if (currentClip != InitClip)
        {
            audioSource.Stop();
            audioSource.clip = InitClip;
            audioSource.loop = true;
            audioSource.Play();
            currentClip = InitClip;
            Debug.Log("AudioHelper: Playing InitClip.");
        }
    }

    /// <summary>
    /// 'battle' 오디오 클립을 재생합니다.
    /// </summary>
    public static void PlayBattleAudio()
    {
        if (audioSource == null || BattleClip == null)
        {
            Debug.LogError("AudioHelper is not initialized properly.");
            return;
        }

        if (currentClip != BattleClip)
        {
            audioSource.Stop();
            audioSource.clip = BattleClip;
            audioSource.loop = true;
            audioSource.Play();
            currentClip = BattleClip;
            Debug.Log("AudioHelper: Playing BattleClip.");
        }
    }

    /// <summary>
    /// 'die' 오디오 클립을 재생합니다.
    /// </summary>
    public static void PlayDieAudio()
    {
        if (audioSource == null || DieClip == null)
        {
            Debug.LogError("AudioHelper is not initialized properly.");
            return;
        }

        // 'die' 오디오는 반복 재생하지 않고 한 번 재생
        if (currentClip != DieClip)
        {
            audioSource.Stop();
            audioSource.clip = DieClip;
            audioSource.loop = false;
            audioSource.Play();
            currentClip = DieClip;
            Debug.Log("AudioHelper: Playing DieClip.");
        }
    }

    /// <summary>
    /// 'game_finish' 오디오 클립을 재생합니다.
    /// </summary>
    public static void PlayGameFinishAudio()
    {
        if (audioSource == null || GameFinishClip == null)
        {
            Debug.LogError("AudioHelper is not initialized properly.");
            return;
        }

        // 'game_finish' 오디오는 반복 재생하지 않고 한 번 재생
        if (currentClip != GameFinishClip)
        {
            audioSource.Stop();
            audioSource.clip = GameFinishClip;
            audioSource.loop = false;
            audioSource.Play();
            currentClip = GameFinishClip;
            Debug.Log("AudioHelper: Playing GameFinishClip.");
        }
    }

    /// <summary>
    /// 현재 재생 중인 오디오 클립을 중지합니다.
    /// </summary>
    public static void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            currentClip = null;
            Debug.Log("AudioHelper: Audio stopped.");
        }
    }
}
