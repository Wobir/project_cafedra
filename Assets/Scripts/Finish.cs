#nullable enable
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public sealed class FinishTrigger : MonoBehaviour
{
    [Header("UI окна победы")]
    [SerializeField] private GameObject finishMenu = default!;
    [SerializeField] private TMP_Text? statsText;

    [Header("Stars")]
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = Color.black;

    [Header("Настройки сцен")]
    [SerializeField] private string nextLevelSceneName = "";
    [SerializeField] private string menuSceneName = "LevelSelect";

    [Header("Игровые данные")]
    [SerializeField] private CollectBonus? playerScore;
    [SerializeField] private float[] targetTimeForStars = new float[] { 60f, 45f };
    private float levelTime;
    private int maxBonus;
    private bool isFinished;

    [Header("Звуки")]
    [SerializeField] private GameObject? audioRoot;
    private AudioSource[] audioSources = Array.Empty<AudioSource>();

    private PauseManager? pauseManager;

    private void Awake()
    {
        if (playerScore == null)
            playerScore = FindFirstObjectByType<CollectBonus>();

        if (audioRoot != null)
            audioSources = audioRoot.GetComponentsInChildren<AudioSource>(true);

        pauseManager = FindFirstObjectByType<PauseManager>();
        maxBonus = GameObject.FindGameObjectsWithTag("Bonus").Length;
    }

    private void Update()
    {
        if (!isFinished)
            levelTime += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFinished) return;
        if (other.CompareTag("Player"))
            ShowFinishMenu();
    }

    private void ShowFinishMenu()
    {
        isFinished = true;

        if (pauseManager != null)
        {
            pauseManager.enabled = false;
        }

        if (finishMenu != null)
            finishMenu.SetActive(true);

        int stars = CalculateStars();
        UpdateStarsUI(stars);

        string timeTarget = targetTimeForStars.Length > 0 && targetTimeForStars[0] > 0
            ? $"Для 2 звезды: {FormatTimeWithUnits(targetTimeForStars[0])}"
            : "";

        if (statsText != null && playerScore != null)
        {
            statsText.text =
                $"Кристаллы: {playerScore.Score}/{maxBonus}\n" +
                $"Время: {FormatTime(levelTime)}\n" +
                $"{timeTarget}".Trim();
        }

        Time.timeScale = 0f;

        foreach (var audioSource in audioSources)
            audioSource?.Pause();

        if (playerScore != null)
            GameSaveManager.Instance.UpdateLevelProgress(SceneManager.GetActiveScene().name, stars, levelTime, playerScore.Score);
    }

    private int CalculateStars()
    {
        int stars = 1;

        if (levelTime <= targetTimeForStars[0])
            stars = 2;

        if (playerScore != null && playerScore.Score >= maxBonus)
            stars = 3;

        return stars;
    }

    private void UpdateStarsUI(int stars)
    {
        for (int i = 1; i <= 3; i++)
        {
            GameObject? starObj = GameObject.Find($"Star{i}");
            if (starObj != null)
            {
                var img = starObj.GetComponent<Image>();
                if (img != null)
                {
                    img.color = i <= stars ? activeColor : inactiveColor;
                }
                starObj.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"Объект Star{i} не найден на сцене!");
            }
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    private string FormatTimeWithUnits(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        string result = "";

        if (minutes > 0)
            result += $"{minutes} мин ";

        if (seconds > 0)
            result += $"{seconds} сек";

        return result.Trim();
    }

    public void LoadNextLevel()
    {
        ResumeAudio();
        Time.timeScale = 1f;

        if (pauseManager != null)
        {
            pauseManager.enabled = true;
        }

        if (!string.IsNullOrWhiteSpace(nextLevelSceneName))
            SceneManager.LoadScene(nextLevelSceneName);
    }

    public void ReturnToMenu()
    {
        ResumeAudio();
        Time.timeScale = 1f;

        if (pauseManager != null)
        {
            pauseManager.enabled = true;
        }

        if (!string.IsNullOrWhiteSpace(menuSceneName))
            SceneManager.LoadScene(menuSceneName);
    }

    private void ResumeAudio()
    {
        foreach (var audioSource in audioSources)
            audioSource?.UnPause();
    }
}
