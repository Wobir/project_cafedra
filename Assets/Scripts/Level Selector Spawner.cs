#nullable enable
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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

    private void Start()
    {
        if (buttonPrefab == null || contentParent == null || levels.Length == 0)
            return;

        foreach (var lvl in levels)
        {
            if (buttonPrefab == null || contentParent == null)
                continue;

            var btnObj = Instantiate(buttonPrefab, contentParent);
            var text = btnObj.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                text.text = lvl.displayName;
                if (isLevelSelectScene && lvl.showStars)
                {
                    int stars = 0;
                    if (GameSaveManager.Instance?.data?.levels != null)
                    {
                        stars = GameSaveManager.Instance.data.levels.Find(l => l.sceneName == lvl.sceneName)?.stars ?? 0;
                    }
                    text.text += $"\nЗвезды: {stars}/3";
                }
            }

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
