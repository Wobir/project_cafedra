using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMusic : MonoBehaviour
{
    private static BGMusic _instance;

    [Header("Настройки аудио")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip levelMusic;
    [SerializeField] private float fadeSpeed = 1f;

    [Header("Сцены меню")]
    [SerializeField] private string[] menuScenes;

    private bool _isFading;
    private float _targetVolume;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayClip(menuMusic);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isMenu = System.Array.Exists(menuScenes, n => n == scene.name);
        var newClip = isMenu ? menuMusic : levelMusic;

        if (audioSource.clip == newClip) return;
        StartCoroutine(SwitchMusic(newClip));
    }

    private System.Collections.IEnumerator SwitchMusic(AudioClip newClip)
    {
        _isFading = true;
        while (audioSource.volume > 0f)
        {
            audioSource.volume -= fadeSpeed * Time.deltaTime;
            yield return null;
        }

        PlayClip(newClip);
        while (audioSource.volume < _targetVolume)
        {
            audioSource.volume += fadeSpeed * Time.deltaTime;
            yield return null;
        }

        _isFading = false;
    }

    private void PlayClip(AudioClip clip)
    {
        audioSource.clip = clip;
        _targetVolume = 1f;
        audioSource.volume = 0f;
        audioSource.loop = true;
        audioSource.Play();
    }
}
