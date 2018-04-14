using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Locomotion Settings")]
    [SerializeField]
    private float speed = 7;

    [Header("Animation")]
    [SerializeField]
    private Animator legsAnimator;

    private Vector3 velocity;
    private new Rigidbody2D rigidbody2D;
    private Animator animator;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void FixedUpdate()
    {
        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        float verticalAxis = Input.GetAxisRaw("Vertical");

        if (horizontalAxis == 0 && verticalAxis == 0)
        {
            animator.SetFloat("Move", 0);
            legsAnimator.SetFloat("Move", 0);

            velocity = Vector3.zero;
        }
        else
        {
            animator.SetFloat("Move", 1);
            legsAnimator.SetFloat("Move", 1);

            velocity = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized * speed;
        }

        rigidbody2D.velocity = velocity;
    }
}
