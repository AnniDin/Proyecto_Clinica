using System.Collections;
using UnityEngine;

public class EmotionReactionManager : MonoBehaviour
{
    [SerializeField] private EmotionReactionUI reactionPrefab;
    [SerializeField] private RectTransform reactionsContainer;

    [SerializeField] private int reactionCount = 8;
    [SerializeField] private float spawnInterval = 0.12f;

    public void ShowReactions(Sprite sprite)
    {
        StartCoroutine(SpawnReactions(sprite));
    }

    private IEnumerator SpawnReactions(Sprite sprite)
    {
        for (int i = 0; i < reactionCount; i++)
        {
            EmotionReactionUI reaction = Instantiate(
                reactionPrefab,
                reactionsContainer
            );

            RectTransform rect = reaction.GetComponent<RectTransform>();

            // Posición aleatoria en la zona inferior
            float x = Random.Range(
                -reactionsContainer.rect.width * 0.35f,
                 reactionsContainer.rect.width * 0.35f
            );

            float y = -reactionsContainer.rect.height * 0.3f;

            rect.anchoredPosition = new Vector2(x, y);

            reaction.Initialize(sprite);

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
