using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinSliderUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject skinItemPrefab;

    private void Start()
    {
        if (contentParent == null || skinItemPrefab == null || SkinManager.Instance == null) return;

        PopulateSlider();
        SkinManager.Instance.LoadSelectedSkin();
        SkinManager.Instance.SelectSkin(SkinManager.Instance.SelectedSkinIndex);
    }

    private void PopulateSlider()
    {
        var skins = SkinManager.Instance.Skins;
        if (skins.Count == 0) return;

        foreach (Transform child in contentParent) Destroy(child.gameObject);

        for (int i = 0; i < skins.Count; i++)
        {
            var skin = skins[i];
            var item = Instantiate(skinItemPrefab, contentParent);
            var rt = item.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;
            rt.anchoredPosition = Vector2.zero;

            var preview = item.transform.Find("Preview")?.GetComponent<Image>();
            if (preview != null && skin.Material != null)
            {
                var tex = SkinPreviewRenderer.Instance.GeneratePreview(skin.Material);
                var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                preview.sprite = sprite;
            }
            var lockIcon = item.transform.Find("LockIcon")?.gameObject;
            if (lockIcon != null)
            {
                lockIcon.SetActive(!skin.Unlocked);

                var currencyImage = lockIcon.transform.Find("Currency")?.GetComponent<Image>();
                var currencyAmount = lockIcon.transform.Find("Amount")?.GetComponent<TMP_Text>();

                if (currencyImage != null)
                    currencyImage.sprite = skin.LockRequirementSprite;

                if (currencyAmount != null)
                {
                    int amount = skin.RequiredStars > 0 ? skin.RequiredStars : skin.RequiredBonuses;
                    currencyAmount.text = amount.ToString();
                }
            }

            var button = item.GetComponent<Button>() ?? item.GetComponentInChildren<Button>();
            if (button == null) continue;

            int index = i;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                SkinManager.Instance.RecheckUnlocks();
                var selected = SkinManager.Instance.Skins[index];
                if (!selected.Unlocked) return;
                SkinManager.Instance.SelectSkin(index);
            });
        }
    }
}
