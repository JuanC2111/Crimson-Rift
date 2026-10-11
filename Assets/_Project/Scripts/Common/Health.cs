using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float invulnerabilityTime = 1f;

    public int Current { get; private set; }
    public int Max => maxHealth;
    public bool IsDead => Current <= 0;

    public event Action<int, int> Changed;
    public event Action<Vector2> Hurt;
    public event Action Died;

    private float invulnerableUntil;

    private void Awake() => Current = maxHealth;

    private void Start() => Changed?.Invoke(Current, maxHealth);

    public void TakeDamage(int amount, Vector2 hitSource)
    {
        if (IsDead || Time.time < invulnerableUntil) return;

        Current = Mathf.Max(0, Current - amount);
        invulnerableUntil = Time.time + invulnerabilityTime;
        Changed?.Invoke(Current, maxHealth);

        if (IsDead) Died?.Invoke();
        else Hurt?.Invoke(hitSource);
    }

    // Muerte instantánea: ignora la protección. Sirve para caídas al vacío.
    public void Kill()
    {
        if (IsDead) return;
        Current = 0;
        Changed?.Invoke(Current, maxHealth);
        Died?.Invoke();
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        Current = Mathf.Min(maxHealth, Current + amount);
        Changed?.Invoke(Current, maxHealth);
    }
}