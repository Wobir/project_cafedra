using UnityEngine;

[System.Serializable]
public class BallSkin
{
    public string Name;
    public Sprite PreviewSprite;
    public Sprite LockRequirementSprite;
    public Material Material;
    public bool Unlocked;
    public int RequiredStars;
    public int RequiredBonuses;
}
