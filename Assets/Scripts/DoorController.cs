using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Transform door;
    [SerializeField] private float openHeight = 3f;
    [SerializeField] private float openSpeed = 2f;

    private Vector3 _closedLocalPosition;
    private Vector3 _openLocalPosition;
    private bool _shouldOpen;

    private void Start()
    {
        _closedLocalPosition = door.localPosition;
        _openLocalPosition = _closedLocalPosition + Vector3.up * openHeight;
    }

    private void Update()
    {
        if (_shouldOpen)
        {
            door.localPosition = Vector3.Lerp(door.localPosition, _openLocalPosition, Time.deltaTime * openSpeed);
            if (Vector3.Distance(door.localPosition, _openLocalPosition) < 0.01f)
            {
                door.localPosition = _openLocalPosition;
                _shouldOpen = false;
            }
        }
    }

    public void OpenDoor()
    {
        _shouldOpen = true;
    }
}
