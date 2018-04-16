using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private CursorController cursorController;
    [SerializeField]
    private float smoothTime = 0.2f;
    [SerializeField]
    private float lookaheadDistance = 7;
    [SerializeField]
    private Vector3 velocity;
    [SerializeField]
    private Transform target;

    private void FixedUpdate()
    {
        Vector2 direction = (cursorController.transform.position - target.position).normalized * 0.8f;

        float offset = 0;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            offset = lookaheadDistance;
        }

        Vector2 targetOffset = target.position + target.up * 0.5f * offset;
        Vector2 position = Vector3.SmoothDamp(transform.position, targetOffset + direction, ref velocity, smoothTime);

        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }
}
