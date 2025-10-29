using UnityEngine;

public class CollectBonus : MonoBehaviour
{
    [SerializeField] private AudioSource fxSource;
    [SerializeField] private AudioClip bonusSound;
    private int _score = 0;
    public int Score => _score;

    private void OnEnable()
    {
        ApplyGlobalVolume();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bonus")) return;
        _score++;
        if (fxSource && bonusSound)
            fxSource.PlayOneShot(bonusSound, GameSaveManager.Instance?.GetSfxVolume() ?? 1f);
        Destroy(other.gameObject);
    }

    private void ApplyGlobalVolume()
    {
        if (fxSource)
            fxSource.volume = GameSaveManager.Instance?.GetSfxVolume() ?? 1f;
    }
}
