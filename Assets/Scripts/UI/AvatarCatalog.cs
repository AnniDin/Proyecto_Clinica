using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AvatarCatalog", menuName = "Avatar/Avatar Catalog")]
public class AvatarCatalog : ScriptableObject
{
    public List<AvatarSpriteOption> hairstyles;
    public List<AvatarHeadOption> heads;
    public List<AvatarSpriteOption> outfits;
}
