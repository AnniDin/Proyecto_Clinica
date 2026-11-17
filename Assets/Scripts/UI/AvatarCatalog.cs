using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AvatarCatalog", menuName = "Avatar/Avatar Catalog")]
public class AvatarCatalog : ScriptableObject
{
    public List<AvatarSpriteOption> hairstyles;
    public List<AvatarFaceOption> faces;
    public List<AvatarSpriteOption> outfits;
}
