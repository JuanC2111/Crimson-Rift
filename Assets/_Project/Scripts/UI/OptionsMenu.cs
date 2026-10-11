using UnityEngine;
using UnityEngine.UI;

// Ventana de opciones: dos sliders que controlan el volumen a través del AudioManager.
// El volumen se guarda solo, así que se recuerda al volver a jugar.
public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button openButton;   // El botón "Opciones" del menú de pausa
    [SerializeField] private Button backButton;

    private void Start()
    {
        panel.SetActive(false);

        if (AudioManager.Instance != null)
        {
            musicSlider.value = AudioManager.Instance.MusicVolume;
            sfxSlider.value = AudioManager.Instance.SfxVolume;
        }

        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        openButton.onClick.AddListener(() => panel.SetActive(true));
        backButton.onClick.AddListener(() => panel.SetActive(false));
    }

    private void OnMusicChanged(float value)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnSfxChanged(float value)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetSfxVolume(value);
    }
}