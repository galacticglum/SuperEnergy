using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector2 smoothening;
    [SerializeField]
    private Vector2 velocity;
    [SerializeField]
    private Transform target;

    private void FixedUpdate()
    {
        Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - target.position).normalized;
        float offset = 0;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            offset = 7;
        }

        Vector2 targetOffset = target.position + target.right * 0.5f * offset;
        Vector2 position = new Vector2(Mathf.SmoothDamp(transform.position.x, targetOffset.x + direction.x, ref velocity.x, smoothening.x),
                                       Mathf.SmoothDamp(transform.position.y, targetOffset.y + direction.y, ref velocity.y, smoothening.y));

        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }
}
