using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class PlayerBonus : MonoBehaviour
{
    [SerializeField] private AudioClip bonusSound;
    [SerializeField] private TextMeshProUGUI scoreText;

    private AudioSource _audioSource;
    private int _score;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bonus")) return;

        _score++;
        _audioSource.PlayOneShot(bonusSound);
        Destroy(other.gameObject);
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText == null) return;
        scoreText.text = $"Очки: {_score}";
    }
}
