using Assets.Scripts.Logging;
using UnityEngine;

/// <summary>
/// Code modified from https://unity3d.com/learn/tutorials/projects/2d-roguelike-tutorial/audio-and-sound-manager
/// </summary>
public class SoundManager : MonoBehaviour
{
    static readonly ILog _log = Log.GetLogger(typeof(SoundManager));

    // Include basic UI options in this. Otherwise split up sounds into bite-sized scripts on game objects
    public AudioClip Cancel;
    public AudioClip Confirm;
    public AudioClip MoveUI;

    public static SoundManager Instance = null;
    public AudioSource SfxPlayer;
    public AudioSource MusicPlayer;

    void Awake()
    {
        //Check if there is already an instance of SoundManager
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }

    public static void PlaySingle(AudioClip clip)
    {
        PlaySingle(clip, 1);
    }

    public static void PlaySingle(AudioClip clip, float volume)
    {
        Instance.SfxPlayer.PlayOneShot(clip, volume);
    }

    public static void PlayConfirm()
    {
        PlaySingle(Instance.Confirm);
    }

    public static void PlayCancel()
    {
        PlaySingle(Instance.Cancel);
    }

    public static void PlayMoveUI()
    {
        PlaySingle(Instance.MoveUI);
    }

    internal static void PlayMusic(AudioClip music)
    {
        Instance.MusicPlayer.loop = true;
        Instance.MusicPlayer.clip = music;
        Instance.MusicPlayer.Play();
    }

    internal static void StopMusic()
    {
        if (Instance.MusicPlayer.isPlaying)
        {
            Instance.MusicPlayer.Stop();
        }
    }

    internal static void ResetMusicVolume()
    {
        Instance.MusicPlayer.volume = 1;
    }

    internal static void SetMusicVolume(float level)
    {
        Instance.MusicPlayer.volume = level;
    }

    internal static void PauseMusic()
    {
        Instance.MusicPlayer.Pause();
    }

    internal static void UnpauseMusic()
    {
        Instance.MusicPlayer.UnPause();
    }
}
