using System;
using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour
{
    private const float Speed = 2;
    private const float MaxWaypointDistance = 2;

    private static readonly Color HurtColourTint = new Color(1, 0.55f, 0.55f);

    private float health = 100;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;
    private float lastTakeDamageTime;

    private AIPath aiPath;
    private Seeker seeker;
    private int currentWaypoint;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Player";
        spriteRenderer.sortingOrder = 1;

        aiPath = gameObject.AddComponent<AIPath>();
        seeker = GetComponent<Seeker>();

        aiPath.whenCloseToDestination = CloseToDestinationMode.Stop;
        aiPath.maxSpeed = Speed;
        aiPath.rotationIn2D = true;
        aiPath.gravity = Vector3.zero;
        aiPath.updateRotation = false;
    }

    private void Update()
    {
        if (Time.time > lastTakeDamageTime + 0.05f && spriteRenderer.color != Color.white)
        {
            spriteRenderer.color = Color.white;
        }
    }

    private void FixedUpdate()
    {
        if (playerController.IsCombatCircleFull && !playerController.IsEnemyInCombatCircle(this))
        {
            return;
        }

        if (!playerController.IsCombatCircleFull && !playerController.IsEnemyInCombatCircle(this))
        {
            playerController.AddEnemyToCombatCircle(this);
        }

        aiPath.destination = playerController.GetPositionInCombatCircle(this);

        Path currentPath = seeker.GetCurrentPath();
        if (currentPath == null) return;
        if (currentWaypoint >= currentPath.vectorPath.Count)
        {
            RotateTo(playerController.transform.position);
            return;
        }

        RotateTo(currentPath.vectorPath[currentWaypoint]);

        if (Vector3.Distance(transform.position, currentPath.vectorPath[currentWaypoint]) < MaxWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    private void RotateTo(Vector3 target)
    {
        Vector2 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            playerController.RemoveEnemyFromCombatCircle(this);
            Destroy(gameObject);
        }

        spriteRenderer.color = HurtColourTint;
        lastTakeDamageTime = Time.time;
    }

    public static GameObject Create(GameObject prefab, Vector3 position, PlayerController playerController)
    {
        GameObject enemyGameObject = Instantiate(prefab, position, Quaternion.identity);
        Enemy enemyInstance = enemyGameObject.AddComponent<Enemy>();
        enemyInstance.playerController = playerController;

        return enemyGameObject;
    }

}
