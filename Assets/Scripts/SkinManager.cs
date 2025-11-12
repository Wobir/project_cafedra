using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance { get; private set; }

    [SerializeField] private List<BallSkin> skins = new();
    private int selectedSkinIndex;

    public List<BallSkin> Skins => skins;
    public int SelectedSkinIndex => selectedSkinIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        RecheckUnlocks();
        LoadSelectedSkin();
    }

    public void RecheckUnlocks()
    {
        int stars = GameSaveManager.Instance?.data.totalStars ?? 0;
        int bonuses = GameSaveManager.Instance?.data.totalBonuses ?? 0;

        foreach (var skin in skins)
        {
            skin.Unlocked = (stars >= skin.RequiredStars && skin.RequiredStars > 0)
                          || (bonuses >= skin.RequiredBonuses && skin.RequiredBonuses > 0)
                          || (skin.RequiredStars == 0 && skin.RequiredBonuses == 0);
        }
    }

    public void SelectSkin(int index)
    {
        if (index < 0 || index >= skins.Count) return;
        if (!skins[index].Unlocked) return;

        selectedSkinIndex = index;
        GameSaveManager.Instance?.UpdateSelectedSkin(index);
    }

    public Material GetSelectedMaterial()
    {
        if (skins == null || skins.Count == 0) return null;
        return skins[selectedSkinIndex].Material;
    }

    public void LoadSelectedSkin()
    {
        selectedSkinIndex = PlayerPrefs.GetInt("SelectedSkinIndex", 0);
        RecheckUnlocks();
    }
}
