using UnityEngine;

// Todo lo del golpe de espada vive aquí, separado del movimiento.
// El estado de ataque solo dice "ahora golpea"; este componente decide a quién.
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float hitDelay = 0.25f; // Cuándo cae el golpe dentro de la animación (cuadro 3 de 6)
    [SerializeField] private Vector2 hitBoxSize = new Vector2(1.6f, 1.2f);
    [SerializeField] private Vector2 hitBoxOffset = new Vector2(1f, 0.2f); // Delante del héroe
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform visual; // Para saber hacia dónde mira

    public float HitDelay => hitDelay;

    private float Facing => visual != null && visual.localScale.x < 0f ? -1f : 1f;

    private Vector2 HitCenter =>
        (Vector2)transform.position + new Vector2(hitBoxOffset.x * Facing, hitBoxOffset.y);

    public void DoHit()
    {
        Collider2D[] targets = Physics2D.OverlapBoxAll(HitCenter, hitBoxSize, 0f, enemyLayer);
        foreach (Collider2D target in targets)
        {
            // IDamageable: no importa si es un hongo o un jefe, todos reciben daño igual.
            if (target.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(damage, transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(HitCenter, hitBoxSize);
    }
}