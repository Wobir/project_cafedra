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
    [SerializeField] private Sprite? starSprite; // используется в prefab
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = Color.black;

    private void Start()
    {
        if (buttonPrefab == null || contentParent == null || levels.Length == 0)
            return;

        foreach (var lvl in levels)
        {
            var btnObj = Instantiate(buttonPrefab, contentParent);

            // Название уровня
            var text = btnObj.GetComponentInChildren<TMPro.TMP_Text>();
            if (text != null)
                text.text = lvl.displayName;

            // Звезды
            if (isLevelSelectScene && lvl.showStars)
            {
                int starsEarned = 0;
                if (GameSaveManager.Instance?.data?.levels != null)
                {
                    starsEarned = GameSaveManager.Instance.data.levels
                        .Find(l => l.sceneName == lvl.sceneName)?.stars ?? 0;
                }

                for (int i = 1; i <= 3; i++)
                {
                    Transform? starTransform = btnObj.transform.Find($"Star{i}");
                    if (starTransform != null)
                    {
                        var img = starTransform.GetComponent<Image>();
                        if (img != null)
                        {
                            img.color = i <= starsEarned ? activeColor : inactiveColor;
                        }
                        starTransform.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                // Если ShowStars выключен — полностью скрываем/удаляем объекты звезд
                for (int i = 1; i <= 3; i++)
                {
                    Transform? starTransform = btnObj.transform.Find($"Star{i}");
                    if (starTransform != null)
                    {
                        starTransform.gameObject.SetActive(false);
                    }
                }
            }

            // Кнопка запуска сцены
            var btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                string scene = lvl.sceneName;
                btn.onClick.AddListener(() =>
                {
                    if (!string.IsNullOrWhiteSpace(scene))
                        SceneManager.LoadScene(scene);
                });
            }
        }
    }

    public void SetLevels(LevelInfo[] lvls) => levels = lvls ?? Array.Empty<LevelInfo>();
}
