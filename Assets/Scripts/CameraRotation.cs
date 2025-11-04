#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class CameraRotation : MonoBehaviour
{
    [SerializeField] private List<Transform> targets = new();
    [SerializeField, Min(0)] private int startTargetIndex = 0;
    [SerializeField, Min(0f)] private float distance = 5f, rotationStep = 90f, rotationSpeed = 5f;
    [SerializeField, Min(0f)] private float zoomSpeed = 2f, minDistance = 2f, maxDistance = 15f, followSmooth = 5f;
    [SerializeField, Range(-89f,89f)] private float minXAngle = -80f, maxXAngle = 80f;
    [SerializeField, Min(0f)] private float mouseSensitivityX = 200f, mouseSensitivityY = 150f;
    [SerializeField] private InputActionReference? pointerDeltaAction, scrollAction, leftClickAction, rotateLeftAction, rotateRightAction, selectTargetAction;

    public enum CameraSide { Front, Right, Back, Left }
    public CameraSide CurrentSide { get; private set; } = CameraSide.Front;
    public IReadOnlyList<Transform> Targets => targets;
    public int CurrentTargetIndex => currentTargetIndex;

    private int currentTargetIndex;
    private Transform? target;
    private Vector3 smoothedTargetPos;
    private float currentYAngle, targetYAngle, currentXAngle;
    private bool rotatingWithMouse;

    private static readonly Key[] NumericKeys = {Key.Digit1,Key.Digit2,Key.Digit3,Key.Digit4,Key.Digit5,Key.Digit6,Key.Digit7,Key.Digit8,Key.Digit9};

    private void OnEnable() => EnableAction(pointerDeltaAction, scrollAction, leftClickAction, rotateLeftAction, rotateRightAction, selectTargetAction);
    private void OnDisable() => DisableAction(pointerDeltaAction, scrollAction, leftClickAction, rotateLeftAction, rotateRightAction, selectTargetAction);

    private static void EnableAction(params InputActionReference?[] actions) { foreach(var a in actions) if(a?.action != null && !a.action.enabled) a.action.Enable(); }
    private static void DisableAction(params InputActionReference?[] actions) { foreach(var a in actions) if(a?.action != null && a.action.enabled) a.action.Disable(); }

    private void Start()
    {
        if(targets.Count==0) { enabled=false; return; }
        currentTargetIndex = Mathf.Clamp(startTargetIndex,0,targets.Count-1);
        SetTarget(currentTargetIndex);
        var euler = transform.eulerAngles;
        currentYAngle = targetYAngle = euler.y;
        currentXAngle = euler.x;
        smoothedTargetPos = target?.position ?? transform.position;
        UpdateCameraSide();
    }

    private void LateUpdate()
    {
        if(target==null) return;
        HandleTargetSelection();
        smoothedTargetPos = Vector3.Lerp(smoothedTargetPos,target.position,Time.deltaTime*followSmooth);
        HandleStepRotation();
        HandlePointerRotation();
        currentYAngle = Mathf.LerpAngle(currentYAngle,targetYAngle,Time.deltaTime*rotationSpeed);
        HandleZoom();
        ApplyTransform();
        UpdateCameraSide();
    }

    private void HandleTargetSelection()
    {
        if(selectTargetAction?.action!=null)
        {
            var val = selectTargetAction.action.ReadValue<Vector2>();
            if(val!=Vector2.zero) { SetTarget(Mathf.Clamp((int)val.x,0,targets.Count-1)); return; }
        }
        var kb = Keyboard.current;
        if(kb==null) return;
        for(int i=0;i<Mathf.Min(NumericKeys.Length,targets.Count);i++)
            if(kb[NumericKeys[i]].wasPressedThisFrame) { SetTarget(i); break; }
    }

    private void HandleStepRotation()
    {
        bool left = rotateLeftAction?.action?.triggered ?? Keyboard.current?.qKey.wasPressedThisFrame ?? false;
        bool right = rotateRightAction?.action?.triggered ?? Keyboard.current?.eKey.wasPressedThisFrame ?? false;
        if(left) targetYAngle-=rotationStep;
        if(right) targetYAngle+=rotationStep;
    }

    private void HandlePointerRotation()
    {
        Vector2 delta = pointerDeltaAction?.action?.ReadValue<Vector2>() ?? Mouse.current?.delta.ReadValue() ?? Vector2.zero;
        bool pressed = leftClickAction?.action?.ReadValue<float>()>0.5f || Mouse.current?.leftButton.isPressed==true;
        if(pressed)
        {
            rotatingWithMouse=true;
            float dy = delta.x*mouseSensitivityX*Time.deltaTime;
            float dx = delta.y*mouseSensitivityY*Time.deltaTime;
            currentYAngle+=dy; targetYAngle+=dy;
            currentXAngle=Mathf.Clamp(currentXAngle-dx,minXAngle,maxXAngle);
        }
        else if(rotatingWithMouse)
        {
            targetYAngle=Mathf.Round(targetYAngle/rotationStep)*rotationStep;
            rotatingWithMouse=false;
        }
    }

    private void HandleZoom()
    {
        Vector2 scroll = scrollAction?.action?.ReadValue<Vector2>() ?? Mouse.current?.scroll.ReadValue() ?? Vector2.zero;
        if(Mathf.Abs(scroll.y)>Mathf.Epsilon) distance=Mathf.Clamp(distance-scroll.y*zoomSpeed*Time.deltaTime,minDistance,maxDistance);
    }

    private void ApplyTransform()
    {
        Quaternion rot = Quaternion.Euler(currentXAngle,currentYAngle,0f);
        Vector3 pos = smoothedTargetPos-rot*Vector3.forward*distance;
        transform.SetPositionAndRotation(pos,rot);
    }

    private void SetTarget(int idx)
    {
        if(targets.Count==0) return;
        int clamped=Mathf.Clamp(idx,0,targets.Count-1);
        if(clamped==currentTargetIndex && target!=null) return;
        currentTargetIndex=clamped;
        target=targets[currentTargetIndex];
        smoothedTargetPos=target?.position ?? smoothedTargetPos;
    }

    private void UpdateCameraSide()
    {
        float angle=targetYAngle%360f; if(angle<0f) angle+=360f;
        CurrentSide=angle switch
        {
            <45f or >=315f=>CameraSide.Front,
            <135f=>CameraSide.Right,
            <225f=>CameraSide.Back,
            _=>CameraSide.Left
        };
    }
}
