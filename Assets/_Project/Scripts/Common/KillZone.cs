using UnityEngine;

// Zona que mata a cualquiera con Health que entre (héroe o enemigos que caen al vacío).
[RequireComponent(typeof(Collider2D))]
public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Health health)) health.Kill();
    }
}