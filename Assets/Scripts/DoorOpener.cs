using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    [SerializeField] private DoorController door;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        door.OpenDoor();
        gameObject.SetActive(false);
    }
}
