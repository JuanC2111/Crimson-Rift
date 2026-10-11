using UnityEngine;

// Todo lo que pueda recibir daño (jugador, enemigos, jefes) cumple este contrato.
// Así una espada o una trampa dañan "cosas dañables" sin saber qué son.
public interface IDamageable
{
    void TakeDamage(int amount, Vector2 hitSource);
}