using UnityEngine;

// La meta: al tocarla el jugador, el nivel se completa.
[RequireComponent(typeof(Collider2D))]
public class LevelGoal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null) GameManager.Instance.CompleteLevel();
    }
}