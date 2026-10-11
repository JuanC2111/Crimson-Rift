using System.Collections;
using UnityEngine;

// Hace que algo se vea golpeado: parpadea en rojo y, si muere, desaparece.
[RequireComponent(typeof(Health))]
public class HitFeedback : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyDelay = 0.4f;

    private Health health;
    private Color original;

    private void Awake()
    {
        health = GetComponent<Health>();
        if (sprite == null) sprite = GetComponentInChildren<SpriteRenderer>();
        original = sprite.color;
    }

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
        StopAllCoroutines();
        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = original;
    }

    private void OnDied()
    {
        StopAllCoroutines();
        sprite.color = Color.red;
        if (destroyOnDeath) Destroy(gameObject, destroyDelay);
    }
}