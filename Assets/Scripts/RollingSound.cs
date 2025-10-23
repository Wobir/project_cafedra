using UnityEngine;

public class BallRolling : MonoBehaviour
{
    [SerializeField] private AudioSource rollingSource;
    [SerializeField] private AudioClip rollingSound;
    [SerializeField] private float minRollSpeed = 0.1f;
    [SerializeField] private float minRollVolume = 0.1f;
    [SerializeField] private float maxRollVolume = 1f;
    [SerializeField] private float volumeMultiplier = 0.1f;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        rollingSource.clip = rollingSound;
        rollingSource.loop = true;
        rollingSource.volume = 0f;
        rollingSource.Play();
    }

    private void Update()
    {
        float angularSpeed = _rigidbody.angularVelocity.magnitude;

        if (angularSpeed < minRollSpeed)
        {
            rollingSource.volume = 0f;
        }
        else
        {
            rollingSource.volume = Mathf.Clamp(angularSpeed * volumeMultiplier, minRollVolume, maxRollVolume);
        }
    }
}
