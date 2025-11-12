using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class BGMusic : MonoBehaviour
{
    public static BGMusic Instance { get; private set; }

    [Header("Music Settings")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip levelMusic;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private string[] menuScenes;

    [Header("UI Tags")]
    [SerializeField] private string musicSliderTag = "MusicSlider";
    [SerializeField] private string sfxSliderTag = "SFXSlider";
    [SerializeField] private string playerTag = "Player";

    private Slider musicSlider;
    private Slider sfxSlider;
    private float musicVolume;
    private float sfxVolume;

    private readonly float saveDelay = 1.5f;
    private float lastChangeTime = -10f;      
    private bool isSavingScheduled;       

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
        StartCoroutine(InitializeMusic());
    }

    private IEnumerator InitializeMusic()
    {
        while (GameSaveManager.Instance == null)
            yield return null;

        GameSaveManager.Instance.LoadProgress();
        musicVolume = GameSaveManager.Instance.GetMusicVolume();
        sfxVolume = GameSaveManager.Instance.GetSfxVolume();

        if (musicSource != null)
            musicSource.volume = musicVolume;

        ApplySfxVolume();

        AssignSliders();
        StartCoroutine(AssignSlidersCoroutine());
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isMenu = menuScenes.Contains(scene.name);
        AudioClip targetClip = isMenu ? menuMusic : levelMusic;

        if (musicSource != null && musicSource.clip != targetClip)
        {
            StopAllCoroutines();
            StartCoroutine(SwitchMusic(targetClip));
        }

        AssignSliders();
    }

    private IEnumerator AssignSlidersCoroutine()
    {
        yield return new WaitUntil(() =>
            GameObject.FindWithTag(musicSliderTag) != null || GameObject.FindWithTag(sfxSliderTag) != null);
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
        if (obj == null) return;

        musicSlider = obj.GetComponent<Slider>();
        if (musicSlider == null) return;

        musicSlider.onValueChanged.RemoveAllListeners();
        musicSlider.value = musicVolume;
        musicSlider.onValueChanged.AddListener(v =>
        {
            musicVolume = v;
            if (musicSource != null)
                musicSource.volume = v;
            ScheduleSave();
        });
    }

    private void AssignSfxSlider()
    {
        GameObject obj = GameObject.FindWithTag(sfxSliderTag);
        if (obj == null) return;

        sfxSlider = obj.GetComponent<Slider>();
        if (sfxSlider == null) return;

        sfxSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.value = sfxVolume;
        sfxSlider.onValueChanged.AddListener(v =>
        {
            sfxVolume = v;
            ApplySfxVolume();
            ScheduleSave();
        });
    }

    private void ScheduleSave()
    {
        lastChangeTime = Time.time;

        if (!isSavingScheduled)
            StartCoroutine(DelayedSave());
    }

    private IEnumerator DelayedSave()
    {
        isSavingScheduled = true;
        while (Time.time - lastChangeTime < saveDelay)
            yield return null;

        isSavingScheduled = false;

        if (GameSaveManager.Instance != null)
            GameSaveManager.Instance.UpdateSettings(musicVolume, sfxVolume);
    }

    private void ApplySfxVolume()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        if (players == null || players.Length == 0) return;

        foreach (GameObject player in players)
        {
            if (player == null) continue;

            AudioSource[] sources = player.GetComponentsInChildren<AudioSource>();
            if (sources == null || sources.Length == 0) continue;

            foreach (AudioSource src in sources)
            {
                if (src != null)
                    src.volume = sfxVolume;
            }
        }
    }

    private IEnumerator SwitchMusic(AudioClip newClip)
    {
        if (musicSource == null || musicSource.clip == newClip)
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
