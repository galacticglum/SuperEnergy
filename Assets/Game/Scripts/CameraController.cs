using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector2 smoothening;
    [SerializeField]
    private float lookaheadDistance = 7;
    [SerializeField]
    private Vector3 velocity;
    [SerializeField]
    private Transform target;

    private void FixedUpdate()
    {
        Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - target.position).normalized;

        float offset = 0;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            offset = lookaheadDistance;
        }

        Vector2 targetOffset = target.position + target.right * offset;
        Vector2 position = Vector3.SmoothDamp(transform.position, targetOffset + direction, ref velocity, smoothening.x);

        transform.position = new Vector3(position.x, position.y, transform.position.z);
        //transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
    }
}
