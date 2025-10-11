#nullable enable
using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlayerTeleport : MonoBehaviour
{
    private Portal? lastPortal;
    private float lastTime;
    [SerializeField, Min(0f)] private float portalCooldown=1f;

    public bool CanUsePortal(Portal portal)=>portal!=null && (lastPortal!=portal || Time.time-lastTime>portalCooldown);
    public void SetLastPortal(Portal portal){ lastPortal=portal; lastTime=Time.time; }
    public void SetPortalCooldown(float cooldown)=>portalCooldown=Mathf.Max(0f,cooldown);
}
