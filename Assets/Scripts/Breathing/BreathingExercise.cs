
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BreathingExercise : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RectTransform innerCircle;
    [SerializeField] private RectTransform outerCircle;
    [SerializeField] private Image phaseIcon;
    [SerializeField] private CanvasGroup iconCanvasGroup;
    [SerializeField] private AudioSource audioSource;

    [Header("Apariencia")]
    [SerializeField, Range(0, 2)] private int appearance = 2;
    // 0 = Femenino, 1 = Masculino, 2 = Neutro

    [Header("Sprites (9 iconos)")]
    [SerializeField] private Sprite[] sprites = new Sprite[9];
    // 0-2: Femenino
    // 3-5: Masculino
    // 6-8: Neutro
    // Orden: Inhalar, Exhalar, Descansar

    [Header("Audios")]
    [SerializeField] private AudioClip explanationAudio;
    [SerializeField] private AudioClip inhaleAudio;
    [SerializeField] private AudioClip holdAudio;
    [SerializeField] private AudioClip exhaleAudio;
    [SerializeField] private AudioClip restAudio;

    [Header("Duración de las fases (segundos)")]
    [SerializeField] private float inhaleDuration = 4f;
    [SerializeField] private float holdDuration = 2f;
    [SerializeField] private float exhaleDuration = 6f;
    [SerializeField] private float restDuration = 2f;

    [Header("Transición de iconos")]
    [SerializeField] private float iconFadeDuration = 0.5f;

    [Header("Configuración")]
    [SerializeField] private float coverageMargin = 1.02f;

    private enum BreathingPhase
    {
        Inhale,
        Hold,
        Exhale,
        Rest
    }

    private BreathingPhase currentPhase = BreathingPhase.Inhale;

    private Vector3 restingScale;
    private Vector3 expandedScale;

    private Coroutine iconTransitionCoroutine;

    private void Awake()
    {
        // Calcular escalas de los círculos.
        restingScale = innerCircle.localScale;

        float widthRatio = outerCircle.rect.width / innerCircle.rect.width;
        float heightRatio = outerCircle.rect.height / innerCircle.rect.height;

        float scaleRatio = Mathf.Max(widthRatio, heightRatio);

        expandedScale = restingScale * scaleRatio * coverageMargin;

        innerCircle.localScale = restingScale;

        // Mostrar el primer icono directamente, sin fade inicial.
        int initialSpriteIndex = appearance * 3 + 2;

        if (sprites != null && sprites.Length >= 9 && phaseIcon != null)
        {
            phaseIcon.sprite = sprites[initialSpriteIndex];
        }

        if (iconCanvasGroup != null)
        {
            iconCanvasGroup.alpha = 1f;
        }
    }

    private void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError("Falta asignar el AudioSource.");
            return;
        }

        StartCoroutine(RunBreathingExercise());
    }

    private IEnumerator RunBreathingExercise()
    {
        // 1. Reproducir explicación inicial.
        yield return PlayExplanation();

        // 2. Repetir el ciclo de respiración.
        while (true)
        {
            yield return RunPhase(
                BreathingPhase.Inhale,
                inhaleAudio,
                inhaleDuration
            );

            yield return RunPhase(
                BreathingPhase.Hold,
                holdAudio,
                holdDuration
            );

            yield return RunPhase(
                BreathingPhase.Exhale,
                exhaleAudio,
                exhaleDuration
            );

            yield return RunPhase(
                BreathingPhase.Rest,
                restAudio,
                restDuration
            );
        }
    }

    private IEnumerator PlayExplanation()
    {
        // Mostrar el icono de mantener/descansar.
        currentPhase = BreathingPhase.Rest;
        UpdatePhaseIcon();

        if (explanationAudio == null)
        {
            Debug.LogWarning("No se ha asignado el audio de explicación.");
            yield break;
        }

        audioSource.clip = explanationAudio;
        audioSource.Play();

        // Esperar a que termine la explicación.
        while (audioSource.isPlaying)
        {
            yield return null;
        }
    }

    private IEnumerator RunPhase(
        BreathingPhase phase,
        AudioClip clip,
        float duration)
    {
        currentPhase = phase;

        // Actualizar icono.
        UpdatePhaseIcon();

        // Reproducir audio de la fase.
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No hay audio asignado para la fase: " + phase);
        }

        float elapsed = 0f;
        duration = Mathf.Max(duration, 0.01f);

        // Animar el círculo durante el tiempo configurado.
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

            UpdateCircleAnimation(phase, progress);

            yield return null;
        }

        // Asegurar que el círculo llega a su posición final.
        UpdateCircleAnimation(phase, 1f);

        // Si el audio todavía está reproduciéndose,
        // esperar a que termine antes de pasar a la siguiente fase.
        while (audioSource.isPlaying)
        {
            yield return null;
        }
    }

    private void UpdateCircleAnimation(
        BreathingPhase phase,
        float progress)
    {
        switch (phase)
        {
            case BreathingPhase.Inhale:

                innerCircle.localScale = Vector3.Lerp(
                    restingScale,
                    expandedScale,
                    Mathf.SmoothStep(0f, 1f, progress)
                );

                break;

            case BreathingPhase.Hold:

                innerCircle.localScale = expandedScale;

                break;

            case BreathingPhase.Exhale:

                innerCircle.localScale = Vector3.Lerp(
                    expandedScale,
                    restingScale,
                    Mathf.SmoothStep(0f, 1f, progress)
                );

                break;

            case BreathingPhase.Rest:

                innerCircle.localScale = restingScale;

                break;
        }
    }

    private void UpdatePhaseIcon()
    {
        if (phaseIcon == null || sprites == null || sprites.Length < 9)
            return;

        int phaseIndex = currentPhase switch
        {
            BreathingPhase.Inhale => 0,
            BreathingPhase.Hold => 2,
            BreathingPhase.Exhale => 1,
            BreathingPhase.Rest => 2,
            _ => 0
        };

        int spriteIndex = appearance * 3 + phaseIndex;

        if (iconTransitionCoroutine != null)
        {
            StopCoroutine(iconTransitionCoroutine);
        }

        Sprite newSprite = sprites[spriteIndex];

        // No hacer transición si el icono ya es el mismo.
        if (phaseIcon.sprite == newSprite)
        {
            return;
        }

        if (iconCanvasGroup == null)
        {
            phaseIcon.sprite = newSprite;
            return;
        }

        iconTransitionCoroutine = StartCoroutine(
            TransitionIcon(newSprite)
        );
    }

    private IEnumerator TransitionIcon(Sprite newSprite)
    {
        float halfDuration = Mathf.Max(iconFadeDuration, 0.01f) / 2f;

        // Fade out.
        float elapsed = 0f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;

            iconCanvasGroup.alpha = 1f -
                Mathf.Clamp01(elapsed / halfDuration);

            yield return null;
        }

        iconCanvasGroup.alpha = 0f;

        // Cambiar sprite.
        phaseIcon.sprite = newSprite;

        // Fade in.
        elapsed = 0f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;

            iconCanvasGroup.alpha =
                Mathf.Clamp01(elapsed / halfDuration);

            yield return null;
        }

        iconCanvasGroup.alpha = 1f;
        iconTransitionCoroutine = null;
    }

    public void SetAppearance(int value)
    {
        if (value < 0 || value > 2)
        {
            Debug.LogWarning("Apariencia no válida. Usa 0, 1 o 2.");
            return;
        }

        appearance = value;
        UpdatePhaseIcon();
    }
}
