#nullable enable
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class LevelSelect : MonoBehaviour
{
    [Serializable]
    public sealed class LevelInfo
    {
        public string sceneName = string.Empty;
        public string displayName = string.Empty;
        public bool showStars = true;
    }

    [SerializeField] private bool isLevelSelectScene = true;
    [SerializeField] private GameObject? buttonPrefab;
    [SerializeField] private Transform? contentParent;
    [SerializeField] private LevelInfo[] levels = Array.Empty<LevelInfo>();

    [Header("Stars")]
    [SerializeField] private Sprite? starSprite;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = Color.black;

    private void Start()
    {
        if (buttonPrefab == null || contentParent == null || levels.Length == 0)
            return;

        var savedLevels = GameSaveManager.Instance != null && GameSaveManager.Instance.data != null
            ? GameSaveManager.Instance.data.levels
            : null;

        foreach (var lvl in levels)
        {
            var btnObj = Instantiate(buttonPrefab, contentParent);

            var text = btnObj.GetComponentInChildren<TMPro.TMP_Text>();
            if (text != null)
                text.text = lvl.displayName;

            int starsEarned = 0;
            if (isLevelSelectScene && lvl.showStars && savedLevels != null)
            {
                var levelData = savedLevels.Find(l => l.sceneName == lvl.sceneName);
                starsEarned = levelData != null ? levelData.stars : 0;
            }

            for (int i = 1; i <= 3; i++)
            {
                Transform? starTransform = btnObj.transform.Find($"Star{i}");
                if (starTransform != null)
                {
                    if (lvl.showStars && isLevelSelectScene)
                    {
                        var img = starTransform.GetComponent<Image>();
                        if (img != null)
                            img.color = i <= starsEarned ? activeColor : inactiveColor;
                        starTransform.gameObject.SetActive(true);
                    }
                    else
                    {
                        starTransform.gameObject.SetActive(false);
                    }
                }
            }

            if (btnObj.TryGetComponent<Button>(out var btn))
            {
                string scene = lvl.sceneName;
                if (!string.IsNullOrWhiteSpace(scene))
                    btn.onClick.AddListener(() => SceneManager.LoadScene(scene));
            }
        }
    }

    public void SetLevels(LevelInfo[] lvls) => levels = lvls != null ? lvls : Array.Empty<LevelInfo>();
}
