using UnityEngine;

// Trampa que daña al jugador mientras la toca. La protección de 1 segundo
// de Health evita que lo mate de golpe.
[RequireComponent(typeof(Collider2D))]
public class Hazard : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent(out IDamageable target))
            target.TakeDamage(damage, transform.position);
    }
}