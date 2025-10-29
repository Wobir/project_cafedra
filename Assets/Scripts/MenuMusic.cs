using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class BGMusic : MonoBehaviour
{
    public static BGMusic Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip levelMusic;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private string[] menuScenes;
    [SerializeField] private string musicSliderTag = "MusicSlider";
    [SerializeField] private string sfxSliderTag = "SFXSlider";
    [SerializeField] private string playerTag = "Player";

    private Slider musicSlider;
    private Slider sfxSlider;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        GameSaveManager.Instance?.LoadProgress();
        ApplySavedVolumes();
        AssignSliders();
        StartCoroutine(AssignSlidersCoroutine());
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isMenu = menuScenes.Contains(scene.name);
        AudioClip targetClip = isMenu ? menuMusic : levelMusic;
        if (musicSource.clip != targetClip)
        {
            StopAllCoroutines();
            StartCoroutine(SwitchMusic(targetClip));
        }
        AssignSliders();
    }

    private IEnumerator AssignSlidersCoroutine()
    {
        yield return new WaitUntil(() => GameObject.FindWithTag(musicSliderTag) != null || GameObject.FindWithTag(sfxSliderTag) != null);
        AssignSliders();
    }

    private void AssignSliders()
    {
        AssignMusicSlider();
        AssignSfxSlider();
    }

    private void AssignMusicSlider()
    {
        GameObject obj = GameObject.FindWithTag(musicSliderTag);
        if (!obj) return;
        musicSlider = obj.GetComponent<Slider>();
        if (musicSlider == null) return;
        musicSlider.onValueChanged.RemoveAllListeners();
        musicSlider.value = musicVolume;
        musicSlider.onValueChanged.AddListener(v =>
        {
            musicVolume = v;
            if (musicSource != null)
                musicSource.volume = v;
            GameSaveManager.Instance?.UpdateSettings(musicVolume, sfxVolume);
        });
    }

    private void AssignSfxSlider()
    {
        GameObject obj = GameObject.FindWithTag(sfxSliderTag);
        if (!obj) return;
        sfxSlider = obj.GetComponent<Slider>();
        if (sfxSlider == null) return;
        sfxSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.value = sfxVolume;
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }

    private void SetSfxVolume(float value)
    {
        sfxVolume = value;
        ApplySfxVolume();
        GameSaveManager.Instance?.UpdateSettings(musicVolume, sfxVolume);
    }

    private void ApplySfxVolume()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag(playerTag))
        {
            foreach (AudioSource src in player.GetComponentsInChildren<AudioSource>())
            {
                src.volume = sfxVolume;
            }
        }
    }

    private void ApplySavedVolumes()
    {
        musicVolume = GameSaveManager.Instance?.GetMusicVolume() ?? 1f;
        sfxVolume = GameSaveManager.Instance?.GetSfxVolume() ?? 1f;
        if (musicSource != null)
            musicSource.volume = musicVolume;
        ApplySfxVolume();
    }

    private IEnumerator SwitchMusic(AudioClip newClip)
    {
        if (musicSource.clip == newClip)
            yield break;
        while (musicSource.volume > 0f)
        {
            musicSource.volume -= fadeSpeed * Time.deltaTime;
            yield return null;
        }
        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();
        while (musicSource.volume < musicVolume)
        {
            musicSource.volume += fadeSpeed * Time.deltaTime;
            yield return null;
        }
        musicSource.volume = musicVolume;
    }
}
