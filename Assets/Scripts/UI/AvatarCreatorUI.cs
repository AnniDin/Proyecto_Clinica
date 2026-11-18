using UnityEngine;
using UnityEngine.UI;

public class AvatarCreatorUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private AvatarCatalog catalog;

    [Header("Avatar Images")]
    [SerializeField] private Image headImage;
    [SerializeField] private Image hairBackImage;
    [SerializeField] private Image hairFrontImage;
    [SerializeField] private Image outfitBackImage;
    [SerializeField] private Image outfitFrontImage;

    [Header("Buttons")]
    [SerializeField] private Button previousHeadButton;
    [SerializeField] private Button nextHeadButton;
    [SerializeField] private Button previousHairButton;
    [SerializeField] private Button nextHairButton;
    [SerializeField] private Button previousOutfitButton;
    [SerializeField] private Button nextOutfitButton;

    private int headIndex = 0;
    private int hairIndex = 0;
    private int outfitIndex = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SubscribeListeners();
        UpdateAvatar();
    }

    private void SubscribeListeners()
    {
        previousHairButton.onClick.AddListener(() => ChangeHair(-1));
        nextHairButton.onClick.AddListener(() => ChangeHair(1));

        previousHeadButton.onClick.AddListener(() => ChangeFace(-1));
        nextHeadButton.onClick.AddListener(() => ChangeFace(1));

        previousOutfitButton.onClick.AddListener(() => ChangeOutfit(-1));
        nextOutfitButton.onClick.AddListener(() => ChangeOutfit(1));

    }

    private void ChangeHair(int direction)
    {
        hairIndex = (hairIndex + direction + catalog.hairstyles.Count) % catalog.hairstyles.Count;

        UpdateAvatar();
    }

    private void ChangeFace (int direction)
    {
        headIndex = (headIndex + direction + catalog.heads.Count) % catalog.heads.Count;

        UpdateAvatar();

    }

    private void ChangeOutfit(int direction)
    {
        outfitIndex = (outfitIndex + direction + catalog.outfits.Count) % catalog.outfits.Count;

        UpdateAvatar();
    }

    private void UpdateAvatar()
    {
        headImage.sprite = catalog.heads[headIndex].sprite;

        AvatarSpriteOption hair = catalog.hairstyles[hairIndex];
        SetSprite(hairBackImage, hair.backSprite);
        SetSprite(hairFrontImage, hair.frontSprite);

        AvatarSpriteOption outfits = catalog.outfits[outfitIndex];
        SetSprite(outfitBackImage, outfits.backSprite);
        SetSprite(outfitFrontImage, outfits.frontSprite);
    }

    private void SetSprite(Image image, Sprite sprite)
    {
        image.sprite = sprite;
        image.enabled = sprite != null;
    }
}
