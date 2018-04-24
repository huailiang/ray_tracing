using UnityEngine;

public class RayTracingMaster : MonoBehaviour
{
    public ComputeShader RayTracingShader;
    public Texture SkyboxTexture;
    public Light DirectionalLight;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void SetShaderParameters()
    {
        RayTracingShader.SetTexture(0, "_SkyboxTexture", SkyboxTexture);
        RayTracingShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        RayTracingShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);

        Vector3 l = DirectionalLight.transform.forward * -1.0f;
        RayTracingShader.SetVector("_DirectionalLight", new Vector4(l.x, l.y, l.z, DirectionalLight.intensity));
    }

    private void Render(RenderTexture destination)
    {
        // Get a temporary render target for Ray Tracing
        RenderTextureDescriptor desc = new RenderTextureDescriptor(Screen.width, Screen.height,
            RenderTextureFormat.ARGBFloat);
        desc.enableRandomWrite = true;
        RenderTexture result = RenderTexture.GetTemporary(desc);
        result.Create();

        // Set the target and dispatch the compute shader
        RayTracingShader.SetTexture(0, "Result", result);
        RayTracingShader.Dispatch(0, (Screen.width - 1) / 8 + 1, (Screen.height - 1) / 8 + 1, 1);

        // Blit the result texture to the screen and release it
        Graphics.Blit(result, destination);
        RenderTexture.ReleaseTemporary(result);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetShaderParameters();
        Render(destination);
    }
}
