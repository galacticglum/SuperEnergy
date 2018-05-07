using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public delegate void PlayerHealthChangedEventHandler(object sender, PlayerHealthChangedEventArgs args);
public class PlayerHealthChangedEventArgs : EventArgs
{
    public PlayerController PlayerController { get; }
    public float OldHealth { get; }
    public float NewHealth { get; }

    public PlayerHealthChangedEventArgs(PlayerController playerController, float oldHealth, float newHealth)
    {
        PlayerController = playerController;
        OldHealth = oldHealth;
        NewHealth = newHealth;
    }
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public const float WeaponDamage = 20;

    private static readonly Color HurtColourTint = new Color(1, 0.55f, 0.55f);
    private static readonly Vector2 UnitCirleNorth = new Vector2(0, 1);
    private static readonly Vector2 UnitCircleNortheast = new Vector2(0.707106781f, 0.707106781f);
    private static readonly Vector2 UnitCircleEast = new Vector2(1, 0);
    private static readonly Vector2 UnitCircleSoutheast = new Vector2(0.707106781f, -0.707106781f);
    private static readonly Vector2 UnitCircleSouth = new Vector2(0, -1);
    private static readonly Vector2 UnitCircleSouthwest = new Vector2(-0.707106781f, -0.707106781f);
    private static readonly Vector2 UnitCircleWest = new Vector2(-1, 0);
    private static readonly Vector2 UnitCircleNorthwest = new Vector2(-0.707106781f, 0.707106781f);

    public float MaxHealth => maxHealth;
    public bool CanShoot { get; set; }

    public event PlayerHealthChangedEventHandler PlayerHealthChanged;
    private void OnPlayerHealthChanged(PlayerHealthChangedEventArgs args) => PlayerHealthChanged?.Invoke(this, args);

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
    private float maxHealth = 100;
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

    private float currentHealth;
    private float timeSinceLastFire;
    private Vector3 velocity;
    private new Rigidbody2D rigidbody2D;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private Enemy[] combatCircleEnemies;
    private float FuzzyCombatCircleRadius => Random.Range(0.9f, 1.6f) * combatCircleRadius;

    private float lastTakeDamageTime;

    private void Start()
    {
        currentHealth = maxHealth;
        combatCircleEnemies = new Enemy[8];
        rigidbody2D = GetComponent<Rigidbody2D>();
        CanShoot = false;

        InvokeRepeating(nameof(HandleEnemyAttack), 0, 1.5f);
    }

    private void Update()
    {
        if (Time.time > lastTakeDamageTime + 0.05f && spriteRenderer.color != Color.white)
        {
            spriteRenderer.color = Color.white;
        }

        if (CanShoot && Input.GetKey(KeyCode.Mouse0))
        {
            FireProjectile();
        }
    }

    private void HandleEnemyAttack()
    {
        if (IsCombatCircleEmpty) return;
        Enemy[] enemies = combatCircleEnemies.OrderBy(x => Random.value).ToArray();
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null || Vector2.Distance(enemy.transform.position, transform.position) > combatCircleRadius * 1.6f) continue;
            float waitTime = Random.Range(0.1f, 0.2f);
            StartCoroutine(EnemyAttack(enemy, waitTime));
        }
    }

    public IEnumerator EnemyAttack(Enemy enemy, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        const float transitionTime = 0.1f;

        if (enemy == null || enemy.CurrentHealth == 0)
        {
            yield break;
        }

        enemy.IsAttacking = true;
        enemy.AttackWaitTime = waitTime;

        Vector2 originalPosition = enemy.transform.position;
        Vector2 newPosition = originalPosition + (Vector2)(transform.position - enemy.transform.position) * 0.5f;
        float elapsedTime = 0;

        while (elapsedTime < transitionTime)
        {
            enemy.transform.position = Vector2.Lerp(originalPosition, newPosition, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        TakeDamage(enemy.Damage);

        elapsedTime = 0;
        while (elapsedTime < transitionTime)
        {
            enemy.transform.position = Vector2.Lerp(newPosition, originalPosition, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        enemy.IsAttacking = false;
    }

    private void TakeDamage(float amount)
    {
        if (amount == 0) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Debug.Log("Game over");
            //Destroy(gameObject);
        }

        OnPlayerHealthChanged(new PlayerHealthChangedEventArgs(this, currentHealth + amount, currentHealth));
        CameraShakeAgent.Create(Camera.main, 0.1f, 0.1f);
        spriteRenderer.color = HurtColourTint;
        lastTakeDamageTime = Time.time;
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

    private float GetDistanceFromSlot(Vector2 position, int slot) => Vector2.Distance(position, 
        (GetSlotUnitCirclePosition(slot) + new Vector2(transform.position.x, transform.position.y)) * 1.6f);

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

        int slotIndex = availableSlots.OrderBy(slot => GetDistanceFromSlot(enemy.transform.position, slot)).First();
        combatCircleEnemies[slotIndex] = enemy;
    }

    public bool IsEnemyInCombatCircle(Enemy enemy) => combatCircleEnemies.Contains(enemy);
    public bool IsCombatCircleFull => combatCircleEnemies.All(enemy => enemy != null);
    public bool IsCombatCircleEmpty => combatCircleEnemies.All(enemy => enemy == null);

    public void RemoveEnemyFromCombatCircle(Enemy enemy)
    {
        for (int i = 0; i < combatCircleEnemies.Length; i++)
        {
            if (combatCircleEnemies[i] != enemy) continue;

            combatCircleEnemies[i] = null;
            return;
        }
    }

    private static Vector2 GetSlotUnitCirclePosition(int slot)
    {
        switch (slot)
        {
            // North
            case 0:
                return UnitCirleNorth;
            // Northeast
            case 1:
                return UnitCircleNortheast;
            // East
            case 2:
                return UnitCircleEast;
            // Southeast
            case 3:
                return UnitCircleSoutheast;
            // South
            case 4:
                return UnitCircleSouth;
            // Southwest
            case 5:
                return UnitCircleSouthwest;
            // West
            case 6:
                return UnitCircleWest;
            // Northwest
            case 7:
                return UnitCircleNorthwest;
            default:
                throw new Exception("Slot does not exist in the combat circle!");
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

        Vector2 relativeCirclePosition = GetSlotUnitCirclePosition(indexOfEnemy);
        relativeCirclePosition *= FuzzyCombatCircleRadius;
        relativeCirclePosition += new Vector2(transform.position.x, transform.position.y);

        return relativeCirclePosition;
    }
}
