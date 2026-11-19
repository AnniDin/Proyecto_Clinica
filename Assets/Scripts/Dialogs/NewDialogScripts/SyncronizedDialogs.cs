using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PauseInterval
{
    public float start;
    public float duration;
}

[Serializable]
public class EmojiSubtitle
{
    [Min(0f)]
    public float startTime;
    public Sprite[] emojis;
}


[Serializable]
public class ConsultaPhase
{
    public EmojiSubtitle[] subtitles;
    public AudioClip audio;
    public int animatorPhaseIndex;
    public PauseInterval[] pauses;
}


public class SyncronizedDialogs : MonoBehaviour
{
    [Header("Fases")]
    [SerializeField] private ConsultaPhase[] phases;

    [Header("Referencias")]
    [SerializeField] private InGameUIInteraction UIManager;
    [SerializeField] private Animator doctorAnimator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform emojiContainer;
    [SerializeField] private Image emojiPrefab;

    private bool phaseChanged;

    private int currentPhase = 0;

    private void Start()
    {
        float totalDuration = 0f;

        foreach (ConsultaPhase phase in phases)
        {
            float audioDuration = phase.audio != null
                ? phase.audio.length
                : 0f;

            float animationDuration = GetAnimationDuration(phase.animatorPhaseIndex);

            totalDuration += Mathf.Max(audioDuration, animationDuration);
        }

        // Añadir los fundidos entre fases
        totalDuration += Mathf.Max(0, phases.Length - 1) * 1f;

        UIManager.StartConsultationTimer(totalDuration);
        StartCoroutine(RunPhase(currentPhase));
    }

    private float GetAnimationDuration(int phaseIndex)
    {
        string stateName = "Fase" + phaseIndex.ToString("00");

        foreach (AnimationClip clip in doctorAnimator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == stateName)
                return clip.length;
        }

        Debug.LogWarning("No se encontró la animación: " + stateName);
        return 0f;
    }

    private IEnumerator RunPhase(int index)
    {
        if (index >= phases.Length)
            yield break;

        ConsultaPhase phase = phases[index];

        phaseChanged = false;

        ClearEmojis();

        doctorAnimator.SetInteger(
            "phaseIndex",
            phase.animatorPhaseIndex
        );

        yield return null;

        BrightnessOverlay.Instance.FadeFromBlack(1f);

        audioSource.Stop();

        if (phase.audio != null)
        {
            audioSource.clip = phase.audio;
            audioSource.Play();

            yield return StartCoroutine(
                ShowEmojisSyncedToAudio(phase.subtitles)
            );
        }

        yield return StartCoroutine(
            WaitForAnimationComplete()
        );

        ClearEmojis();

        currentPhase++;

        if (currentPhase < phases.Length)
        {
            BrightnessOverlay.Instance.FadeToBlack(
                1f,
                () => phaseChanged = true
            );

            yield return new WaitUntil(
                () => phaseChanged
            );

            yield return StartCoroutine(
                RunPhase(currentPhase)
            );
        }
        else
        {
            UIManager.OnNextButton();
        }
    }
    private IEnumerator ShowEmojisSyncedToAudio(
       EmojiSubtitle[] subtitles)
    {
        ClearEmojis();

        int currentSubtitle = -1;

        while (audioSource.isPlaying)
        {
            float audioTime = audioSource.time;

            int targetSubtitle = -1;

            if (subtitles != null)
            {
                for (int i = 0; i < subtitles.Length; i++)
                {
                    if (audioTime >= subtitles[i].startTime)
                    {
                        if (targetSubtitle == -1 ||
                            subtitles[i].startTime >
                            subtitles[targetSubtitle].startTime)
                        {
                            targetSubtitle = i;
                        }
                    }
                }
            }

            if (targetSubtitle != currentSubtitle)
            {
                currentSubtitle = targetSubtitle;

                if (currentSubtitle >= 0)
                {
                    DisplayEmojis(
                        subtitles[currentSubtitle].emojis
                    );
                }
                else
                {
                    ClearEmojis();
                }
            }

            yield return null;
        }
    }

    private void DisplayEmojis(Sprite[] emojis)
    {
        ClearEmojis();

        if (emojis == null)
            return;

        foreach (Sprite emoji in emojis)
        {
            if (emoji == null)
                continue;

            Image newEmoji = Instantiate(
                emojiPrefab,
                emojiContainer
            );

            newEmoji.sprite = emoji;
            newEmoji.preserveAspect = true;
        }
    }

    private void ClearEmojis()
    {
        if (emojiContainer == null)
            return;

        foreach (Transform child in emojiContainer)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
    }
    private IEnumerator WaitForAnimationComplete()
    {
        yield return null;

        while (doctorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;
    }
}
