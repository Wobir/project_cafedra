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

        if (keyboard.cKey != null && keyboard.vKey != null && keyboard.bKey != null)
        {
            if (keyboard.cKey.isPressed && keyboard.vKey.isPressed && keyboard.bKey.isPressed)
                ResetProgress();
        }
    }

    // Сохранение вызывается только при важных действиях
    public void SaveProgress()
    {
#if YANDEX_GAMES
        YG2.saves.dataJson = JsonUtility.ToJson(data, true);
        YG2.saves.selectedSkinIndex = selectedSkinIndex;
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
        {
            data = JsonUtility.FromJson<SaveData>(json);
            selectedSkinIndex = YG2.saves.selectedSkinIndex;
        }
#else
        if (PlayerPrefs.HasKey("SaveData"))
            data = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString("SaveData"));
        selectedSkinIndex = PlayerPrefs.GetInt("SelectedSkinIndex", 0);
#endif
    }

    public void UpdateLevelProgress(string sceneName, int stars, float time, int bonuses)
    {
        if (string.IsNullOrEmpty(sceneName)) return;

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

        for (int i = 0; i < data.levels.Count; i++)
        {
            data.totalStars += data.levels[i].stars;
            data.totalBonuses += data.levels[i].bonusesCollected;
        }

        // Сохраняем только после завершения уровня
        SaveProgress();
        SkinManager.Instance?.RecheckUnlocks();
    }

    public void UpdateSelectedSkin(int index)
    {
        selectedSkinIndex = index;
        SaveProgress();
    }

    public void UpdateSettings(float musicVolume, float sfxVolume)
    {
        data.settings.musicVolume = musicVolume;
        data.settings.sfxVolume = sfxVolume;
    }

    public int GetSelectedSkinIndex() => selectedSkinIndex;
    public float GetMusicVolume() => data.settings != null ? data.settings.musicVolume : 0.6f;
    public float GetSfxVolume() => data.settings != null ? data.settings.sfxVolume : 0.6f;

    public void ResetProgress()
    {
        data = new SaveData();
        selectedSkinIndex = 0;

        SaveProgress();
        SkinManager.Instance?.RecheckUnlocks();
        SkinManager.Instance?.LoadSelectedSkin();
    }
}
