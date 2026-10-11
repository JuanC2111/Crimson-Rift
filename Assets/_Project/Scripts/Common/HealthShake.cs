using Unity.Cinemachine;
using UnityEngine;

// Hace temblar la cámara cuando este objeto recibe daño o muere.
// Patrón Observer: solo ESCUCHA los eventos de Health; Health no sabe que existe.
// Así se puede quitar o cambiar el efecto sin tocar la lógica de vida.
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(CinemachineImpulseSource))]
public class HealthShake : MonoBehaviour
{
    [SerializeField] private float hurtForce = 0.6f;   // fuerza del temblor al recibir daño
    [SerializeField] private float deathForce = 1.2f;  // fuerza al morir

    private Health health;
    private CinemachineImpulseSource impulse;

    private void Awake()
    {
        health = GetComponent<Health>();
        impulse = GetComponent<CinemachineImpulseSource>();
    }

    private void OnEnable()
    {
        health.Hurt += OnHurt;
        health.Died += OnDied;
    }

    // Siempre nos desuscribimos para evitar errores al destruir el objeto.
    private void OnDisable()
    {
        health.Hurt -= OnHurt;
        health.Died -= OnDied;
    }

    private void OnHurt(Vector2 hitDirection) => impulse.GenerateImpulse(hurtForce);
    private void OnDied() => impulse.GenerateImpulse(deathForce);
}