using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Locomotion Settings")]
    [SerializeField]
    private float speed = 7;

    [Header("Animation")]
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Animator legsAnimator;

    [Header("Cursor")]
    [SerializeField]
    private CursorController cursorController;

    private Vector3 velocity;
    private new Rigidbody2D rigidbody2D;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        float verticalAxis = Input.GetAxisRaw("Vertical");

        float moveValue;
        if (horizontalAxis == 0 && verticalAxis == 0)
        {
            moveValue = 0;
            velocity = Vector3.zero;
        }
        else
        {
            moveValue = 1;
            velocity = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized * speed;
        }

        animator.SetFloat("Move", moveValue);
        legsAnimator.SetFloat("Move", moveValue);
        rigidbody2D.velocity = velocity;
    }
}
