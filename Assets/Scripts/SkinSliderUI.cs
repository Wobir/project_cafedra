using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinSliderUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject skinItemPrefab;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.75f, 0.75f, 0.75f, 1f); // немного темнее

    private void Start()
    {
        if (contentParent == null || skinItemPrefab == null || SkinManager.Instance == null) return;

        PopulateSlider();
        SkinManager.Instance.LoadSelectedSkin();
        SkinManager.Instance.SelectSkin(SkinManager.Instance.SelectedSkinIndex);
        UpdateButtonStates();
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
            var buttonText = item.GetComponentInChildren<TMP_Text>();

            if (button == null || buttonText == null) continue;

            int index = i;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                SkinManager.Instance.RecheckUnlocks();
                var selected = SkinManager.Instance.Skins[index];
                if (!selected.Unlocked) return;

                SkinManager.Instance.SelectSkin(index);
                UpdateButtonStates();
            });
        }

        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        int selectedIndex = SkinManager.Instance.SelectedSkinIndex;

        for (int i = 0; i < contentParent.childCount; i++)
        {
            var item = contentParent.GetChild(i);
            var button = item.GetComponent<Button>() ?? item.GetComponentInChildren<Button>();
            var buttonText = item.GetComponentInChildren<TMP_Text>();
            var buttonImage = button?.GetComponent<Image>();

            if (button == null || buttonText == null || buttonImage == null) continue;

            bool isSelected = i == selectedIndex;

            buttonText.text = isSelected ? "Выбрано" : "Выбрать";
            buttonImage.color = isSelected ? selectedColor : normalColor;
        }
    }
}
