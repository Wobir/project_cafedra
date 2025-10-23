using UnityEngine;
using TMPro;

public class CollectBonus : MonoBehaviour
{
    [SerializeField] private AudioSource fxSource;
    [SerializeField] private AudioClip bonusSound;
    [SerializeField] private TextMeshProUGUI scoreText;

    private int _score;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bonus")) return;

        _score++;
        fxSource.PlayOneShot(bonusSound);
        Destroy(other.gameObject);
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText == null) return;
        scoreText.text = $"Очки: {_score}";
    }
}
