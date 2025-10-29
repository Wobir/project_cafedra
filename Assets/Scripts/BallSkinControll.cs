using UnityEngine;

public class BallController : MonoBehaviour
{
    private Renderer ballRenderer;

    private void Start()
    {
        ballRenderer = GetComponent<Renderer>();
        ApplySkin();
    }

    private void ApplySkin()
    {
        if (SkinManager.Instance == null || ballRenderer == null) return;
        var mat = SkinManager.Instance.GetSelectedMaterial();
        if (mat != null)
            ballRenderer.material = mat;
    }
}
