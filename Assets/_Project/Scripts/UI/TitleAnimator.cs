using TMPro;
using UnityEngine;

// Anima un texto: aparece con fade y luego "respira" (escala suave).
// Usa unscaledTime para que funcione aunque el juego esté en pausa (timeScale = 0).
public class TitleAnimator : MonoBehaviour
{
    [SerializeField] private float fadeInTime = 1.2f;   // segundos del fade inicial
    [SerializeField] private float pulseAmount = 0.05f; // 0.05 = crece 5 %
    [SerializeField] private float pulseSpeed = 2f;     // velocidad del pulso

    private TMP_Text label;
    private Vector3 baseScale;
    private float startTime;

    private void Awake()
    {
        label = GetComponent<TMP_Text>();
        baseScale = transform.localScale;
    }

    private void OnEnable()
    {
        startTime = Time.unscaledTime;
        SetAlpha(0f); // empieza invisible
    }

    private void Update()
    {
        float t = Time.unscaledTime - startTime;

        // Fade: de 0 a 1 durante fadeInTime
        SetAlpha(Mathf.Clamp01(t / fadeInTime));

        // Pulso: oscila alrededor de la escala original
        float pulse = 1f + Mathf.Sin(t * pulseSpeed) * pulseAmount;
        transform.localScale = baseScale * pulse;
    }

    private void SetAlpha(float a)
    {
        Color c = label.color;
        c.a = a;
        label.color = c;
    }
}