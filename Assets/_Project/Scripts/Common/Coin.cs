using UnityEngine;

// Moneda recogible: al tocarla el jugador, suma y desaparece.
[RequireComponent(typeof(Collider2D))]
public class Coin : MonoBehaviour
{
    [SerializeField] private int value = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null) GameManager.Instance.AddCoin(value);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayCoin();
        Destroy(gameObject);
    }
}