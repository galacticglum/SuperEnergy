using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour
{
    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }

    public bool IsAttacking { get; set; }
    public float AttackWaitTime { get; set; }
    public float Damage => Random.Range(minimumDamage, maximumDamage);
    public AudioClip AttackAudioClip => attackAudioClip;

    [SerializeField]
    private float speed = 2;
    [SerializeField]
    private float maxWaypointDistance = 2;
    [SerializeField]
    private float minimumEngagementDistance = 10;
    [SerializeField]
    private int minimumDamage = 5;
    [SerializeField]
    private int maximumDamage = 10;
    [SerializeField]
    private float maxHealth = 100;
    [SerializeField]
    private EnemyHealthBar healthBar;

    [SerializeField]
    private AudioClip attackAudioClip;

    private static readonly Color HurtColourTint = new Color(1, 0.55f, 0.55f);

    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;
    private float lastTakeDamageTime;

    private AIPath aiPath;
    private Seeker seeker;
    private int currentWaypoint;

    private Vector2 previousPlayerPosition;

    private void Start()
    {
        healthBar.Initialize(this);

        CurrentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Player";
        spriteRenderer.sortingOrder = 1;

        aiPath = gameObject.AddComponent<AIPath>();
        seeker = GetComponent<Seeker>();

        aiPath.whenCloseToDestination = CloseToDestinationMode.Stop;
        aiPath.maxSpeed = speed;
        aiPath.rotationIn2D = true;
        aiPath.gravity = Vector3.zero;
        aiPath.updateRotation = false;

        SetupDestinationPosition();
    }

    private void SetupDestinationPosition()
    {
        float distance = Vector2.Distance(transform.position, playerController.transform.position);
        if (distance <= minimumEngagementDistance) return;

        float targetDistance = distance * Random.Range(0.4f, 0.6f);
        aiPath.destination = RandomPositionWithinDistance(targetDistance);
    }

    private void Update()
    {
        if (playerController.IsGameover)
        {
            Destroy(gameObject, 1);
        }

        if (Time.time > lastTakeDamageTime + 0.05f && spriteRenderer.color != Color.white)
        {
            spriteRenderer.color = Color.white;
        }

        if (aiPath.reachedEndOfPath)
        {
            SetupDestinationPosition();
        }
    }

    private void FixedUpdate()
    {
        HandleRotation();
        HandleMovement();
    }

    private void HandleRotation()
    {
        Path currentPath = seeker.GetCurrentPath();
        if (currentPath == null) return;
        if (currentWaypoint >= currentPath.vectorPath.Count)
        {
            RotateTo(playerController.transform.position);
            return;
        }

        RotateTo(currentPath.vectorPath[currentWaypoint]);
        if (Vector3.Distance(transform.position, currentPath.vectorPath[currentWaypoint]) < maxWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    private void HandleMovement()
    {
        float distance = Vector2.Distance(transform.position, playerController.transform.position);

        if (distance > minimumEngagementDistance) return;
        if (playerController.IsCombatCircleFull && !playerController.IsEnemyInCombatCircle(this))
        {
            if (!aiPath.reachedEndOfPath) return;

            GridGraph graph = (GridGraph)AstarPath.active.graphs[0];
            Bounds graphBounds = new Bounds(AstarPath.active.transform.position, new Vector3(graph.Width, graph.Depth));

            Vector2 position = Vector2.zero;
            for (int i = 0; i < 20; i++)
            {
                float targetDistance = distance * Random.Range(1f, 1.4f);
                position = RandomPositionWithinDistance(targetDistance);
                if (graphBounds.Contains(position)) break;
            }

            aiPath.destination = position;
            return;
        }

        if (!playerController.IsCombatCircleFull && !playerController.IsEnemyInCombatCircle(this))
        {
            playerController.AddEnemyToCombatCircle(this);
            RecalculateCombatCirclePosition();
        }

        if (Vector2.Distance(previousPlayerPosition, playerController.transform.position) > 1)
        {
            RecalculateCombatCirclePosition();
        }
    }

    private static Vector2 RandomPositionWithinDistance(float distance) => Random.onUnitSphere * distance;

    private void RecalculateCombatCirclePosition()
    {
        aiPath.destination = playerController.GetPositionInCombatCircle(this);
        previousPlayerPosition = playerController.transform.position;
    }

    private void RotateTo(Vector3 target)
    {
        Vector2 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            playerController.RemoveEnemyFromCombatCircle(this);
            if (IsAttacking)
            {
                StopCoroutine(playerController.EnemyAttack(this, AttackWaitTime));
            }

            playerController.Points += Random.Range(100, 150);
            Destroy(gameObject);
        }

        spriteRenderer.color = HurtColourTint;
        lastTakeDamageTime = Time.time;
    }

    public static GameObject Create(GameObject prefab, Vector3 position, PlayerController playerController)
    {
        GameObject enemyGameObject = Instantiate(prefab, position, Quaternion.identity);
        Enemy enemyInstance = enemyGameObject.GetComponent<Enemy>();
        enemyInstance.playerController = playerController;

        return enemyGameObject;
    }
}
