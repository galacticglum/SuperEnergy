using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private float mouseSensitivity = 0.5f;
    [SerializeField]
    private float rotationSpeed = 30;

    private float bonusX;
    private float bonusY;

    private CursorLockMode currentCursorLockMode;

    private bool doSmooth;
    private Vector3 smoothDesintation;
    private Vector3 smoothVelocity;

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (currentCursorLockMode == CursorLockMode.None)
            {
                currentCursorLockMode = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (currentCursorLockMode == CursorLockMode.Locked)
            {
                currentCursorLockMode = CursorLockMode.None;
                Cursor.visible = true;
            }

            Cursor.lockState = currentCursorLockMode;
        }

        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        //if (doSmooth)
        //{
        //    transform.position = Vector3.SmoothDamp(transform.position, smoothDesintation, ref smoothVelocity, 0.2f);

        //    Vector3 difference = transform.position - smoothDesintation;
        //    if (difference.sqrMagnitude < 0.001f)
        //    {
        //        doSmooth = false;
        //    }

        //    return;
        //}

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        bool previousSmoothState = doSmooth;
        Vector2 offset = new Vector2(0f, 0f);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            offset = playerController.transform.up.normalized * 3f;
            doSmooth = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (Vector2.Distance(playerController.transform.position, transform.position) > 4f)
            {
                offset = playerController.transform.up.normalized * -3f;
                doSmooth = true;
            }
        }

        float screenAspect = Screen.width / (float)Screen.height;
        float cameraHeight = Camera.main.orthographicSize * 2;
        Bounds bounds = new Bounds(Camera.main.transform.position, new Vector3(cameraHeight * screenAspect, cameraHeight, 0));

        bonusX += mouseX + offset.x;
        bonusY += mouseY + offset.y;

        Vector3 position = playerController.transform.position + new Vector3(bonusX, bonusY, 0f);
        position.x = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
        position.y = Mathf.Clamp(position.y, bounds.min.y, bounds.max.y);
        position.z = playerController.transform.position.z;

        //if (!previousSmoothState && doSmooth)
        //{
        //    smoothDesintation = position;
        //    return;
        //}

        transform.position = position;

        Vector3 lookForward = playerController.transform.position - position;
        if (lookForward == Vector3.zero) return;
        Quaternion rotation = Quaternion.LookRotation(lookForward, playerController.transform.forward);
        rotation.x = 0;
        rotation.y = 0;

        playerController.transform.rotation = rotation;
    }
}
