using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public bool CanShoot { get; set; }

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

    [Header("Combat")]
    [SerializeField]
    private float fireRate = 0.5f;
    [SerializeField]
    private float projectileSpeed = 10f;
    [SerializeField]
    private Transform nozzleMarker;
    [SerializeField]
    private GameObject projectilePrefab;

    private float timeSinceLastFire;
    private Vector3 velocity;
    private new Rigidbody2D rigidbody2D;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        CanShoot = false;
    }

    private void Update()
    {
        if (CanShoot && Input.GetKey(KeyCode.Mouse0))
        {
            FireProjectile();
        }
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

    private void FireProjectile()
    {
        if (Time.time <= fireRate + timeSinceLastFire) return;

        Vector3 projectileVelocity = nozzleMarker.right * projectileSpeed;

        Vector3 direction = cursorController.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        ProjectileInstance.Create(projectilePrefab, nozzleMarker.position + projectilePrefab.transform.localScale.x / 2f * nozzleMarker.right, rotation, projectileVelocity);

        timeSinceLastFire = Time.time;
        animator.SetTrigger("Shoot");
    }
}
