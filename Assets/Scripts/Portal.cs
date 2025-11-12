#nullable enable
using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public sealed class Portal : MonoBehaviour
{
    [SerializeField] private Portal? linkedPortal;
    [SerializeField, Min(0f)] private float cooldownTime=1f;

    private bool canTeleport=true;

    private void OnTriggerEnter(Collider other)
    {
        if(!canTeleport || linkedPortal==null || !other.CompareTag("Player")) return;
        var player=other.GetComponent<PlayerTeleport>();
        if(player==null || !player.CanUsePortal(this)) return;

        Teleport(other);
        StartCoroutine(linkedPortal.CooldownRoutine());
        player.SetLastPortal(linkedPortal);
    }

    private void Teleport(Collider obj)
    {
        Transform t=obj.transform;
        Rigidbody? rb=obj.attachedRigidbody;
        Vector3 localPos=transform.InverseTransformPoint(t.position);
        Quaternion localRot=Quaternion.Inverse(transform.rotation)*t.rotation;
        t.SetPositionAndRotation(linkedPortal!.transform.TransformPoint(localPos), linkedPortal.transform.rotation*localRot);
        if (rb!=null) rb.linearVelocity=linkedPortal.transform.TransformDirection(transform.InverseTransformDirection(rb.linearVelocity));
    }

    private IEnumerator CooldownRoutine()
    {
        canTeleport=false;
        yield return new WaitForSeconds(cooldownTime);
        canTeleport=true;
    }

    public void StartCooldown()=>StartCoroutine(linkedPortal?.CooldownRoutine() ?? CooldownRoutine());
    public void LinkPortal(Portal portal)=>linkedPortal=portal;
}
