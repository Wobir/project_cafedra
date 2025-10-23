using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private string returnScene;
    [SerializeField] private InputActionReference cancelAction;
    [SerializeField] private CollectBonus playerScore;
    [SerializeField] private AudioSource[] audioSourcesToPause;

    private bool isPaused;
    private float levelTime;
    private int maxBonus;

    private void Awake()
    {
        if (playerScore == null)
        {
            playerScore = FindFirstObjectByType<CollectBonus>();
            if (playerScore == null)
                Debug.LogError("CollectBonus not found in the scene!");
        }

        maxBonus = GameObject.FindGameObjectsWithTag("Bonus").Length;
    }

    private void OnEnable()
    {
        if (cancelAction?.action != null)
        {
            cancelAction.action.Enable();
            cancelAction.action.performed += OnPause;
        }
    }

    private void OnDisable()
    {
        if (cancelAction?.action != null)
        {
            cancelAction.action.performed -= OnPause;
            cancelAction.action.Disable();
        }
    }

    private void Update()
    {
        if (!isPaused)
            levelTime += Time.unscaledDeltaTime;
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;

        if (statsText != null && playerScore != null)
            statsText.text = $"Кристалы: {playerScore.Score}/{maxBonus}\nВремя: {FormatTime(levelTime)}";

        if (audioSourcesToPause != null)
        {
            foreach (var audioSource in audioSourcesToPause)
            {
                if (audioSource == null) continue;
                if (isPaused && audioSource.isPlaying)
                    audioSource.Pause();
                else if (!isPaused)
                    audioSource.UnPause();
            }
        }

        Time.timeScale = isPaused ? 0f : 1f;

        pauseMenu?.SetActive(isPaused);
    }

    public void ReturnToScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(returnScene);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100) % 100);
        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }
}
