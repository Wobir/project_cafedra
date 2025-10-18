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
    }

    [SerializeField] private GameObject? buttonPrefab;
    [SerializeField] private Transform? contentParent;
    [SerializeField] private LevelInfo[] levels = Array.Empty<LevelInfo>();

    private void Start()
    {
        if (buttonPrefab == null || contentParent == null || levels.Length == 0)
        {
            Debug.LogError("LevelSelect: Настройки пусты");
            return;
        }

        foreach (var lvl in levels)
        {
            var btnObj = Instantiate(buttonPrefab, contentParent);
            var text = btnObj.GetComponentInChildren<TMP_Text>();
            if (text != null) text.text = lvl.displayName;
            var btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                string captured = lvl.sceneName;
                btn.onClick.AddListener(() => LoadLevel(captured));
            }
        }
    }

    private static void LoadLevel(string name)
    {
        if (!string.IsNullOrWhiteSpace(name)) SceneManager.LoadScene(name);
    }

    public void SetLevels(LevelInfo[] lvls) => levels = lvls ?? Array.Empty<LevelInfo>();
}
