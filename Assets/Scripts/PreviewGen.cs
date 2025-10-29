using UnityEngine;

public class SkinPreviewRenderer : MonoBehaviour
{
    public static SkinPreviewRenderer Instance { get; private set; }

    [SerializeField] private Mesh previewMesh;
    [SerializeField] private int textureSize = 256;
    [SerializeField] private float cameraDistance = 2.5f;
    [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0);

    private Camera previewCamera;
    private GameObject camGO;
    private GameObject lightGO;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializePreviewScene();
    }

    private void InitializePreviewScene()
    {
        camGO = new GameObject("PreviewCameraTemp") { hideFlags = HideFlags.HideAndDontSave };
        previewCamera = camGO.AddComponent<Camera>();
        previewCamera.clearFlags = CameraClearFlags.SolidColor;
        previewCamera.backgroundColor = backgroundColor;
        previewCamera.orthographic = false;
        previewCamera.fieldOfView = 25;
        previewCamera.nearClipPlane = 0.1f;
        previewCamera.farClipPlane = 10f;
        previewCamera.transform.position = new Vector3(0, 0, -cameraDistance);
        previewCamera.transform.LookAt(Vector3.zero);

        lightGO = new GameObject("PreviewLightTemp") { hideFlags = HideFlags.HideAndDontSave };
        Light light = lightGO.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = Color.white;
        light.intensity = 1.8f;
        light.transform.rotation = Quaternion.Euler(50, -30, 0);
    }

    public Texture2D GeneratePreview(Material material)
    {
        if (material == null)
            return Texture2D.blackTexture;

        // создаём временный объект под этот конкретный рендер
        GameObject tempGO = new GameObject("SkinPreviewTemp") { hideFlags = HideFlags.HideAndDontSave };
        MeshFilter meshFilter = tempGO.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = tempGO.AddComponent<MeshRenderer>();

        meshFilter.sharedMesh = previewMesh ?? Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
        meshRenderer.sharedMaterial = material;
        tempGO.transform.position = Vector3.zero;
        tempGO.transform.rotation = Quaternion.identity;

        // создаём текстуру для рендера
        RenderTexture rt = new RenderTexture(textureSize, textureSize, 24, RenderTextureFormat.ARGB32);
        RenderTexture currentRT = RenderTexture.active;

        previewCamera.targetTexture = rt;
        RenderTexture.active = rt;
        GL.Clear(true, true, backgroundColor);

        // рендерим превью
        previewCamera.Render();

        // создаём финальную Texture2D
        Texture2D previewTex = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
        previewTex.ReadPixels(new Rect(0, 0, textureSize, textureSize), 0, 0);
        previewTex.Apply();

        // сброс
        RenderTexture.active = currentRT;
        previewCamera.targetTexture = null;
        rt.Release();
        DestroyImmediate(rt);
        DestroyImmediate(tempGO);

        return previewTex;
    }

    private void OnDestroy()
    {
        if (camGO != null) DestroyImmediate(camGO);
        if (lightGO != null) DestroyImmediate(lightGO);
    }
}
