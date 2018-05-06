using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public const float WeaponDamage = 20;

    private static readonly Vector2 UnitCirleNorth = new Vector2(0, 1);
    private static readonly Vector2 UnitCircleNortheast = new Vector2(0.707106781f, 0.707106781f);
    private static readonly Vector2 UnitCircleEast = new Vector2(1, 0);
    private static readonly Vector2 UnitCircleSoutheast = new Vector2(0.707106781f, -0.707106781f);
    private static readonly Vector2 UnitCircleSouth = new Vector2(0, -1);
    private static readonly Vector2 UnitCircleSouthwest = new Vector2(-0.707106781f, -0.707106781f);
    private static readonly Vector2 UnitCircleWest = new Vector2(-1, 0);
    private static readonly Vector2 UnitCircleNorthwest = new Vector2(-0.707106781f, 0.707106781f);

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
    [SerializeField]
    private float combatCircleRadius = 2;

    private float timeSinceLastFire;
    private Vector3 velocity;
    private new Rigidbody2D rigidbody2D;

    private Enemy[] combatCircleEnemies;

    private float FuzzyCombatCircleRadius => Random.Range(0.9f, 1.6f) * combatCircleRadius;

    private void Start()
    {
        combatCircleEnemies = new Enemy[8];
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
            rigidbody2D.angularVelocity = 0;
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

    public void AddEnemyToCombatCircle(Enemy enemy)
    {
        List<int> availableSlots = new List<int>();
        for (int i = 0; i < combatCircleEnemies.Length; i++)
        {
            if (combatCircleEnemies[i] == null)
            {
                availableSlots.Add(i);
            }
        }

        int slotIndex = availableSlots[Random.Range(0, availableSlots.Count)];
        combatCircleEnemies[slotIndex] = enemy;
    }

    public bool IsEnemyInCombatCircle(Enemy enemy) => combatCircleEnemies.Contains(enemy);
    public bool IsCombatCircleFull => combatCircleEnemies.All(enemy => enemy != null);

    public void RemoveEnemyFromCombatCircle(Enemy enemy)
    {
        for (int i = 0; i < combatCircleEnemies.Length; i++)
        {
            if (combatCircleEnemies[i] != enemy) continue;

            combatCircleEnemies[i] = null;
            return;
        }
    }

    public Vector2 GetPositionInCombatCircle(Enemy enemy)
    {
        int indexOfEnemy = -1;
        for (int i = 0; i < combatCircleEnemies.Length; i++)
        {
            if (combatCircleEnemies[i] != enemy) continue;

            indexOfEnemy = i;
            break;
        }

        Vector2 relativeCirclePosition;
        switch (indexOfEnemy)
        {
            // North
            case 0:
                relativeCirclePosition = UnitCirleNorth;
                break;
            // Northeast
            case 1:
                relativeCirclePosition = UnitCircleNortheast;
                break;
            // East
            case 2:
                relativeCirclePosition = UnitCircleEast;
                break;
            // Southeast
            case 3:
                relativeCirclePosition = UnitCircleSoutheast;
                break;
            // South
            case 4:
                relativeCirclePosition = UnitCircleSouth;
                break;
            // Southwest
            case 5:
                relativeCirclePosition = UnitCircleSouthwest;
                break;
            // West
            case 6:
                relativeCirclePosition = UnitCircleWest;
                break;
            // Northwest
            case 7:
                relativeCirclePosition = UnitCircleNorthwest;
                break;
            default:
                throw new Exception("Enemy does not exist in the combat circle!");
        }

        relativeCirclePosition *= FuzzyCombatCircleRadius;
        relativeCirclePosition += new Vector2(transform.position.x, transform.position.y);

        return relativeCirclePosition;
    }
}
