#nullable enable
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[DisallowMultipleComponent]
public sealed class FinishTrigger : MonoBehaviour
{
    [Header("UI окна победы")]
    [SerializeField] private GameObject finishMenu = default!;
    [SerializeField] private TMP_Text? statsText;

    [Header("Настройки сцен")]
    [SerializeField] private string nextLevelSceneName = "";
    [SerializeField] private string menuSceneName = "LevelSelect";

    [Header("Игровые данные")]
    [SerializeField] private CollectBonus? playerScore;
    [SerializeField] private int maxStars = 3;
    [SerializeField] private float[] targetTimeForStars = new float[] { 60f, 45f }; // 1 бонусная звезда за все кристаллы, остальные за время
    private float levelTime;
    private int maxBonus;
    private bool isFinished;

    [Header("Звуки")]
    [SerializeField] private GameObject? audioRoot; 
    private AudioSource[] audioSources = System.Array.Empty<AudioSource>();

    private void Awake()
    {
        if (playerScore == null)
            playerScore = FindFirstObjectByType<CollectBonus>();

        if (audioRoot != null)
            audioSources = audioRoot.GetComponentsInChildren<AudioSource>(true);

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

        if (finishMenu != null)
            finishMenu.SetActive(true);

        int stars = CalculateStars();
        string starsText = $"Звезды: {stars}/{maxStars}";

        if (statsText != null && playerScore != null)
            statsText.text = $"Кристаллы: {playerScore.Score}/{maxBonus}\n" +
                             $"Время: {FormatTime(levelTime)}\n" +
                             $"{starsText}";

        Time.timeScale = 0f;

        foreach (var audioSource in audioSources)
            audioSource?.Pause();

        if (playerScore != null)
            GameSaveManager.Instance.UpdateLevelProgress(SceneManager.GetActiveScene().name, stars, levelTime, playerScore.Score);
    }

    private int CalculateStars()
    {
        int stars = 1; // 1 звезда за прохождение уровня

        // 2 звезда за время
        if (levelTime <= targetTimeForStars[0]) 
            stars = 2;

        // 3 звезда за сбор всех бонусов
        if (playerScore != null && playerScore.Score >= maxBonus)
            stars = 3;

        return stars;
    }


    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt(time * 100 % 100);
        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    public void LoadNextLevel()
    {
        ResumeAudio();
        Time.timeScale = 1f;

        if (!string.IsNullOrWhiteSpace(nextLevelSceneName))
            SceneManager.LoadScene(nextLevelSceneName);
    }

    public void ReturnToMenu()
    {
        ResumeAudio();
        Time.timeScale = 1f;

        if (!string.IsNullOrWhiteSpace(menuSceneName))
            SceneManager.LoadScene(menuSceneName);
    }

    private void ResumeAudio()
    {
        foreach (var audioSource in audioSources)
            audioSource?.UnPause();
    }
}
