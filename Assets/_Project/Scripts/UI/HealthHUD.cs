using TMPro;
using UnityEngine;

// HUD de vida: escucha el evento Changed del componente Health del jugador.
// No hay lógica de juego aquí, solo presentación.
public class HealthHUD : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private TMP_Text label;

    private void Start()
    {
        playerHealth.Changed += Refresh;
        Refresh(playerHealth.Current, playerHealth.Max);
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.Changed -= Refresh;
    }

    private void Refresh(int current, int max) => label.text = "Vida: " + current + " / " + max;
}