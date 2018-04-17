using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;

    [Range(0,1)]
    [SerializeField]
    private float mouseSensitivity = 0.5f;
    [SerializeField]
    private float rotationSpeed = 30;

    private float bonusX;
    private float bonusY;

    private CursorLockMode currentCursorLockMode;

    private void Start()
    {
        Cursor.visible = false;
        LockCursor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            LockCursor();
        }

        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;


        float screenAspect = Screen.width / (float)Screen.height;
        float cameraHeight = Camera.main.orthographicSize * 2;
        Bounds bounds = new Bounds(Camera.main.transform.position, new Vector3(cameraHeight * screenAspect, cameraHeight, 0));

        bonusX += mouseX;
        bonusY += mouseY;

        Vector3 position = playerController.transform.position + new Vector3(bonusX, bonusY, 0f);
        position.x = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
        position.y = Mathf.Clamp(position.y, bounds.min.y, bounds.max.y);
        position.z = playerController.transform.position.z;

        transform.position = position;

        Vector3 lookForward = playerController.transform.position - position;
        if (lookForward == Vector3.zero) return;
        Quaternion rotation = Quaternion.LookRotation(lookForward, playerController.transform.forward);
        rotation.x = 0;
        rotation.y = 0;

        playerController.transform.rotation = rotation;
    }

    private void LockCursor()
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
}
