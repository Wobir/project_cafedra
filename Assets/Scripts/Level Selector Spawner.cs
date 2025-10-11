#nullable enable
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[DisallowMultipleComponent]
public sealed class LevelSelect : MonoBehaviour
{
    [SerializeField] private GameObject? buttonPrefab;
    [SerializeField] private Transform? contentParent;
    [SerializeField] private string[] levels=Array.Empty<string>();

    private void Start()
    {
        if(buttonPrefab==null || contentParent==null || levels.Length==0) { Debug.LogError("LevelSelect: Настройки пусты"); return; }
        foreach(var lvl in levels)
        {
            var btnObj=Instantiate(buttonPrefab,contentParent);
            var text=btnObj.GetComponentInChildren<TMP_Text>();
            if(text!=null) text.text=lvl;
            var btn=btnObj.GetComponent<Button>();
            if(btn!=null) { string captured=lvl; btn.onClick.AddListener(()=>LoadLevel(captured)); }
        }
    }

    private static void LoadLevel(string name){ if(!string.IsNullOrWhiteSpace(name)) SceneManager.LoadScene(name); }
    public void SetLevels(string[] lvls)=>levels=lvls??Array.Empty<string>();
}
