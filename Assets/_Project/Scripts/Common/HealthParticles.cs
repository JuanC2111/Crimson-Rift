using UnityEngine;

// Crea un efecto de partículas cuando este objeto recibe daño o muere.
// Observer: solo escucha los eventos de Health. El efecto se instancia en el
// mundo (no como hijo), así sigue visible aunque este objeto sea destruido.
[RequireComponent(typeof(Health))]
public class HealthParticles : MonoBehaviour
{
    [SerializeField] private GameObject hurtEffect;
    [SerializeField] private GameObject deathEffect;

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

    private void OnHurt(Vector2 source) => Spawn(hurtEffect);
    private void OnDied() => Spawn(deathEffect);

    private void Spawn(GameObject prefab)
    {
        if (prefab == null) return; // si no hay efecto asignado, no pasa nada
        Instantiate(prefab, transform.position, Quaternion.identity);
    }
}