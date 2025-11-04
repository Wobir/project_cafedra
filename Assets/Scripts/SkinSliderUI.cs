using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinSliderUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject skinItemPrefab;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new(0.75f, 0.75f, 0.75f, 1f);

    private void Start()
    {
        if (contentParent == null || skinItemPrefab == null || SkinManager.Instance == null) return;

        PopulateSlider();
        var manager = SkinManager.Instance;
        manager.LoadSelectedSkin();
        manager.SelectSkin(manager.SelectedSkinIndex);
        UpdateButtonStates();
    }

    private void PopulateSlider()
    {
        var manager = SkinManager.Instance;
        var skins = manager.Skins;
        if (skins.Count == 0) return;

        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        for (int i = 0; i < skins.Count; i++)
        {
            var skin = skins[i];
            var item = Instantiate(skinItemPrefab, contentParent);
            if (item.TryGetComponent<RectTransform>(out var rt))
            {
                rt.localScale = Vector3.one;
                rt.anchoredPosition = Vector2.zero;
            }

            var previewTransform = item.transform.Find("Preview");
            if (previewTransform != null)
            {
                var preview = previewTransform.GetComponent<Image>();
                if (preview != null && skin.Material != null && SkinPreviewRenderer.Instance != null)
                {
                    var tex = SkinPreviewRenderer.Instance.GeneratePreview(skin.Material);
                    if (tex != null)
                    {
                        var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                        preview.sprite = sprite;
                    }
                }
            }

            var lockTransform = item.transform.Find("LockIcon");
            if (lockTransform != null)
            {
                var lockObj = lockTransform.gameObject;
                lockObj.SetActive(!skin.Unlocked);

                var currencyImage = lockTransform.Find("Currency")?.GetComponent<Image>();
                if (currencyImage != null && skin.LockRequirementSprite != null)
                    currencyImage.sprite = skin.LockRequirementSprite;

                var currencyAmount = lockTransform.Find("Amount")?.GetComponent<TMP_Text>();
                if (currencyAmount != null)
                {
                    int amount = skin.RequiredStars > 0 ? skin.RequiredStars : skin.RequiredBonuses;
                    currencyAmount.text = amount.ToString();
                }
            }

            var button = item.GetComponent<Button>();
            if (button == null)
                button = item.GetComponentInChildren<Button>();

            var buttonText = item.GetComponentInChildren<TMP_Text>();
            if (button == null || buttonText == null) continue;

            int index = i;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                var mgr = SkinManager.Instance;
                if (mgr == null) return;
                mgr.RecheckUnlocks();
                var selected = mgr.Skins[index];
                if (!selected.Unlocked) return;
                mgr.SelectSkin(index);
                UpdateButtonStates();
            });
        }

        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        var manager = SkinManager.Instance;
        if (manager == null) return;

        int selectedIndex = manager.SelectedSkinIndex;

        for (int i = 0; i < contentParent.childCount; i++)
        {
            var item = contentParent.GetChild(i);
            var button = item.GetComponent<Button>();
            if (button == null)
                button = item.GetComponentInChildren<Button>();

            var buttonText = item.GetComponentInChildren<TMP_Text>();
            var buttonImage = button != null ? button.GetComponent<Image>() : null;

            if (button == null || buttonText == null || buttonImage == null) continue;

            bool isSelected = i == selectedIndex;
            buttonText.text = isSelected ? "Выбрано" : "Выбрать";
            buttonImage.color = isSelected ? selectedColor : normalColor;
        }
    }
}
