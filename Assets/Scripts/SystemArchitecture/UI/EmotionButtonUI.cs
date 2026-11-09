using UnityEngine;

public class EmotionButtonUI : MonoBehaviour
{
    [SerializeField]
    private EmotionType emotion;

    public EmotionType GetEmotion()
    {
        return emotion;
    }
}
