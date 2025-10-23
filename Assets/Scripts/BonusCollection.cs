using UnityEngine;

public class CollectBonus : MonoBehaviour
{
    [SerializeField] private AudioSource fxSource;
    [SerializeField] private AudioClip bonusSound;

    private int _score = 0;
    public int Score => _score;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bonus")) return;

        _score++;
        fxSource.PlayOneShot(bonusSound);
        Destroy(other.gameObject);
    }


}
