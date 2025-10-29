using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallCollision : MonoBehaviour
{
    [SerializeField] private AudioSource fxSource;
    [SerializeField] private AudioClip collisionSound;
    [SerializeField] private float impactThreshold = 2f;
    [SerializeField] private float minTimeBetweenImpacts = 0.2f;
    [SerializeField] private float postImpactDelay = 0.1f;

    private Rigidbody _rigidbody;
    private Vector3 _previousVelocity;
    private float _lastImpactTime;
    private float _impactEndTime;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _previousVelocity = _rigidbody.linearVelocity;
    }

    private void Update()
    {
        Vector3 velocity = _rigidbody.linearVelocity;
        float speed = velocity.magnitude;
        float deltaV = Mathf.Abs(speed - _previousVelocity.magnitude);

        if (deltaV > impactThreshold && Time.time - _lastImpactTime > minTimeBetweenImpacts)
        {
            if (fxSource && collisionSound)
                fxSource.PlayOneShot(collisionSound, GameSaveManager.Instance?.GetSfxVolume() ?? 1f);

            _lastImpactTime = Time.time;
            _impactEndTime = Time.time + postImpactDelay;
        }

        _previousVelocity = velocity;
    }

    public bool IsInImpactDelay() => Time.time < _impactEndTime;
}
