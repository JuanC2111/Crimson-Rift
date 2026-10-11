using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Pantalla de Game Over: escucha la muerte del jugador, espera un momento
// para que se vea la animación y muestra el panel.
public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private GameObject panel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private float showDelay = 1.5f;

    private void Start()
    {
        panel.SetActive(false);
        playerHealth.Died += OnPlayerDied;
        restartButton.onClick.AddListener(Restart);
        quitButton.onClick.AddListener(Quit);
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.Died -= OnPlayerDied;
    }

    private void OnPlayerDied() => StartCoroutine(ShowAfterDelay());

    private IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSeconds(showDelay);
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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