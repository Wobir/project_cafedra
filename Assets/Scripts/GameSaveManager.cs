using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class LevelProgress
{
    public string sceneName = "";
    public int stars;
    public float bestTime;
    public int bonusesCollected;
}

[Serializable]
public class GameSettings
{
    public float musicVolume = 0.6f;
    public float sfxVolume = 0.6f;
}

[Serializable]
public class SaveData
{
    public List<LevelProgress> levels = new();
    public GameSettings settings = new();
    public int totalStars;
    public int totalBonuses;
}

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance { get; private set; }
    public SaveData data = new();
    public int selectedSkinIndex;

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
        LoadProgress();
        SkinManager.Instance?.RecheckUnlocks();
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.cKey.isPressed && keyboard.vKey.isPressed && keyboard.bKey.isPressed)
            ResetProgress();
    }

    public void SaveProgress()
    {
#if YANDEX_GAMES
        YG2.saves.dataJson = JsonUtility.ToJson(data, true);
        YG2.SaveProgress();
#else
        PlayerPrefs.SetString("SaveData", JsonUtility.ToJson(data, true));
        PlayerPrefs.SetInt("SelectedSkinIndex", selectedSkinIndex);
        PlayerPrefs.Save();
#endif
    }

    public void LoadProgress()
    {
#if YANDEX_GAMES
        string json = YG2.saves.dataJson;
        if (!string.IsNullOrEmpty(json))
            data = JsonUtility.FromJson<SaveData>(json);
#else
        if (PlayerPrefs.HasKey("SaveData"))
            data = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString("SaveData"));
        selectedSkinIndex = PlayerPrefs.GetInt("SelectedSkinIndex", 0);
#endif
    }

    public void UpdateLevelProgress(string sceneName, int stars, float time, int bonuses)
    {
        var level = data.levels.Find(l => l.sceneName == sceneName);
        if (level == null)
        {
            level = new LevelProgress { sceneName = sceneName };
            data.levels.Add(level);
        }

        if (level.bestTime == 0f || time < level.bestTime)
            level.bestTime = time;

        level.stars = Mathf.Max(level.stars, stars);
        level.bonusesCollected = Mathf.Max(level.bonusesCollected, bonuses);

        data.totalStars = 0;
        data.totalBonuses = 0;
        foreach (var lvl in data.levels)
        {
            data.totalStars += lvl.stars;
            data.totalBonuses += lvl.bonusesCollected;
        }

        SaveProgress();
        SkinManager.Instance?.RecheckUnlocks();

        var progressUI = FindAnyObjectByType<ProgressDisplay>();
        progressUI?.UpdateText();
    }

    public void UpdateSettings(float musicVolume, float sfxVolume)
    {
        data.settings.musicVolume = musicVolume;
        data.settings.sfxVolume = sfxVolume;
        SaveProgress();
    }

    public void UpdateSelectedSkin(int index)
    {
        selectedSkinIndex = index;
        SaveProgress();
    }

    public int GetSelectedSkinIndex() => selectedSkinIndex;
    public float GetMusicVolume() => data.settings.musicVolume;
    public float GetSfxVolume() => data.settings.sfxVolume;

    public void ResetProgress()
    {
        data = new SaveData();
        selectedSkinIndex = 0;

#if YANDEX_GAMES
        YG2.saves.dataJson = JsonUtility.ToJson(data, true);
        YG2.SaveProgress();
#else
        PlayerPrefs.DeleteKey("SaveData");
        PlayerPrefs.DeleteKey("SelectedSkinIndex");
        PlayerPrefs.Save();
#endif

        SkinManager.Instance?.RecheckUnlocks();
        SkinManager.Instance?.LoadSelectedSkin();

        var progressUI = FindAnyObjectByType<ProgressDisplay>();
        progressUI?.UpdateText();
    }
}
