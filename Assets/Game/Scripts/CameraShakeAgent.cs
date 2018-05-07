using UnityEngine;

/// <summary>
/// Controls camera shake. This is a primitive implementation.
/// </summary>
public class CameraShakeAgent : MonoBehaviour
{
    private float duration;
    private float amount;
    private float degradationFactor;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        if (duration > 0)
        {
            transform.localPosition = startPosition + Random.insideUnitSphere * amount;
            duration -= Time.deltaTime * degradationFactor;
        }
        else
        {
            transform.localPosition = startPosition;
            Destroy(this);
        }
    }

    public static CameraShakeAgent Create(Camera camera, float duration, float amount = 0.5f, float degradationFactor = 1f)
    {
        if (camera == null) return null;

        CameraShakeAgent instance = camera.gameObject.AddComponent<CameraShakeAgent>();
        instance.duration = duration;
        instance.amount = amount;
        instance.degradationFactor = degradationFactor;

        return instance;
    }
}