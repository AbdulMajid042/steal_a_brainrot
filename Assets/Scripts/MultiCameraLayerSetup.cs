using UnityEngine;

[System.Serializable]
public class CameraLayerSetup
{
    public string layerName;
    public float farClip = 100f;
}

public class MultiCameraLayerSetup : MonoBehaviour
{
    public CameraLayerSetup[] setups;
    public Camera camera;


    void Start()
    {
        foreach (var setup in setups)
        {
            if (camera == null) continue;

            int layer = LayerMask.NameToLayer(setup.layerName);
            if (layer < 0)
            {
                Debug.LogWarning("Layer not found: " + setup.layerName);
                continue;
            }

            camera.cullingMask = 1 << layer;
            camera.farClipPlane = setup.farClip;
        }
    }
}
