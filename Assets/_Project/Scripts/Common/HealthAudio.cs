using UnityEngine;

// Reproduce el sonido de golpe y de muerte de cualquier objeto con Health.
// Sirve igual para el jugador y para los enemigos.
[RequireComponent(typeof(Health))]
public class HealthAudio : MonoBehaviour
{
    private Health health;

    private void Awake() => health = GetComponent<Health>();

    private void OnEnable()
    {
        health.Hurt += OnHurt;
        health.Died += OnDied;
    }

    private void OnDisable()
    {
        health.Hurt -= OnHurt;
        health.Died -= OnDied;
    }

    private void OnHurt(Vector2 source)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayHurt();
    }

    private void OnDied()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayDeath();
    }
}