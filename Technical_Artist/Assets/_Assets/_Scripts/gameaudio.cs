using UnityEngine;

public class gameaudio : MonoBehaviour
{
    public static gameaudio instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip laneSwitchClip;
    [SerializeField] private AudioClip crashClip;
    [SerializeField] private AudioClip buttonClickClip;

    [Header("Volume")]
    [Range(0f, 1f)][SerializeField] private float musicVolume = 0.45f;
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 0.85f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        SetupSources();
        PlayMusic();
    }

    private void SetupSources()
    {
        if (musicSource != null)
        {
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = musicVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.volume = sfxVolume;
        }
    }

    public void PlayMusic()
    {
        if (musicSource == null || backgroundMusic == null)
        {
            return;
        }

        if (musicSource.clip != backgroundMusic)
        {
            musicSource.clip = backgroundMusic;
        }

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource == null)
        {
            return;
        }

        musicSource.Stop();
    }

    public void PlayLaneSwitch()
    {
        PlaySfx(laneSwitchClip);
    }

    public void PlayCrash()
    {
        PlaySfx(crashClip);
    }

    public void PlayButtonClick()
    {
        PlaySfx(buttonClickClip);
    }

    public void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);

        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }
}