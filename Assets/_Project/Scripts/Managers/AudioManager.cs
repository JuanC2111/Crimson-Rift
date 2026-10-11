using UnityEngine;

// Gestor de audio: un solo objeto que sobrevive entre escenas para que la música
// no se corte al reiniciar. Guarda el volumen en PlayerPrefs para recordarlo.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Fuentes")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Música")]
    [SerializeField] private AudioClip music;

    [Header("Efectos")]
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip coin;
    [SerializeField] private AudioClip hurt;
    [SerializeField] private AudioClip death;

    private const string MusicKey = "MusicVolume";
    private const string SfxKey = "SfxVolume";

    public float MusicVolume => musicSource.volume;
    public float SfxVolume => sfxSource.volume;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ya existe uno: este sobra
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource.volume = PlayerPrefs.GetFloat(MusicKey, 0.5f);
        sfxSource.volume = PlayerPrefs.GetFloat(SfxKey, 0.8f);

        musicSource.clip = music;
        musicSource.loop = true;
        if (music != null) musicSource.Play();
    }

    public void SetMusicVolume(float value)
    {
        musicSource.volume = value;
        PlayerPrefs.SetFloat(MusicKey, value);
    }

    public void SetSfxVolume(float value)
    {
        sfxSource.volume = value;
        PlayerPrefs.SetFloat(SfxKey, value);
    }

    private void Play(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip);
    }

    public void PlayJump() => Play(jump);
    public void PlayCoin() => Play(coin);
    public void PlayHurt() => Play(hurt);
    public void PlayDeath() => Play(death);
}