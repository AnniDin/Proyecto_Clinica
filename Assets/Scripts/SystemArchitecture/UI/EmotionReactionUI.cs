using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EmotionReactionUI : MonoBehaviour
{
    [SerializeField] private float duration = 2f;
    [SerializeField] private float riseDistance = 200f;
    [SerializeField] private float horizontalMovement = 50f;

    private RectTransform rectTransform;
    private Image image;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Initialize(Sprite sprite)
    {
        image.sprite = sprite;
        image.preserveAspect = true;

        StartCoroutine(AnimateReaction());
    }

    private IEnumerator AnimateReaction()
    {
        Vector2 startPosition = rectTransform.anchoredPosition;

        Vector2 endPosition = startPosition + new Vector2(
            Random.Range(-horizontalMovement, horizontalMovement),
            riseDistance
        );

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

            // Movimiento ascendente
            rectTransform.anchoredPosition = Vector2.Lerp(
                startPosition,
                endPosition,
                progress
            );

            // Desvanecimiento progresivo
            canvasGroup.alpha = 1f - progress;

            yield return null;
        }

        Destroy(gameObject);
    }
}
