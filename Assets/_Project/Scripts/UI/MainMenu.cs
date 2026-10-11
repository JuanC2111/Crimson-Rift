using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Menú principal: solo conecta botones con acciones.
// La arquitectura es la misma que en el resto de la UI: el script no tiene lógica de juego.
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private string levelScene = "Level01"; // nombre exacto de la escena

    private void Start()
    {
        Time.timeScale = 1f; // por si venimos de una pausa
        playButton.onClick.AddListener(Play);
        quitButton.onClick.AddListener(Quit);
    }

    private void Play()
    {
        SceneManager.LoadScene(levelScene);
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}