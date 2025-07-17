using UnityEngine;

public class Parralax : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layer;          // The background layer’s transform
        public float parallaxFactor;     // 0 = static, 1 = moves with camera
    }

    public ParallaxLayer[] layers;       // Array of background layers
    public Transform cameraTransform;    // Assign camera here (auto uses Main Camera if left empty)
    private Vector3 previousCameraPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        previousCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 cameraDelta = cameraTransform.position - previousCameraPosition;

        foreach (ParallaxLayer layer in layers)
        {
            Vector3 layerMove = cameraDelta * layer.parallaxFactor;
            layer.layer.position += new Vector3(layerMove.x, layerMove.y, 0);
        }

        previousCameraPosition = cameraTransform.position;
    }
}
