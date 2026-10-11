using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Ventana de "Nivel superado". Escucha el evento LevelCompleted del GameManager,
// muestra el panel y pausa el juego. Los botones reinician o salen.
public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private int totalCoins = 10;

    private void Start()
    {
        Time.timeScale = 1f;      // Por si venimos de una escena que quedó en pausa
        panel.SetActive(false);   // Empieza oculto

        GameManager.Instance.LevelCompleted += Show;
        restartButton.onClick.AddListener(Restart);
        quitButton.onClick.AddListener(Quit);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.LevelCompleted -= Show;
    }

    private void Show()
    {
        resultText.text = "Monedas: " + GameManager.Instance.Coins + " / " + totalCoins;
        panel.SetActive(true);
        Time.timeScale = 0f;      // Pausa el juego detrás de la ventana
    }

    private void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // En el editor, detiene el Play
#else
        Application.Quit();
#endif
    }
}