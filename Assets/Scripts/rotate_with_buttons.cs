#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class RotateWithButtons : MonoBehaviour
{
    [SerializeField, Min(0f)] private float rotationSpeed=100f, damping=5f;
    [SerializeField, Range(0f,90f)] private float maxRotateX=45f, maxRotateZ=45f;
    [SerializeField] private CameraRotation? cameraRotationScript;
    [SerializeField] private string sceneToReturn="MainMenu";

    private Vector2 velocity, rotation;

    private void Update()
    {
        var kb=Keyboard.current;
        if(kb==null || cameraRotationScript==null) return;
        if(kb.escapeKey.wasPressedThisFrame){ SceneManager.LoadScene(sceneToReturn); return; }

        Vector2 input=GetInput(kb,cameraRotationScript.CurrentSide);
        velocity=Vector2.Lerp(velocity+input*rotationSpeed*Time.deltaTime,Vector2.zero,damping*Time.deltaTime);
        rotation=new Vector2(
            Mathf.Clamp(rotation.x+velocity.x*Time.deltaTime,-maxRotateX,maxRotateX),
            Mathf.Clamp(rotation.y+velocity.y*Time.deltaTime,-maxRotateZ,maxRotateZ)
        );
        transform.localRotation=Quaternion.Euler(rotation.x,0f,rotation.y);
    }

    private static Vector2 GetInput(Keyboard kb, CameraRotation.CameraSide side)
    {
        bool w=kb.wKey.isPressed,a=kb.aKey.isPressed,s=kb.sKey.isPressed,d=kb.dKey.isPressed;
        return side switch
        {
            CameraRotation.CameraSide.Front => new Vector2((w?1f:0f)-(s?1f:0f),(a?1f:0f)-(d?1f:0f)),
            CameraRotation.CameraSide.Right => new Vector2((a?1f:0f)-(d?1f:0f),(s?1f:0f)-(w?1f:0f)),
            CameraRotation.CameraSide.Back => new Vector2((s?1f:0f)-(w?1f:0f),(d?1f:0f)-(a?1f:0f)),
            CameraRotation.CameraSide.Left => new Vector2((d?1f:0f)-(a?1f:0f),(w?1f:0f)-(s?1f:0f)),
            _=>Vector2.zero
        };
    }

    public void SetCameraRotation(CameraRotation cam)=>cameraRotationScript=cam;
}
