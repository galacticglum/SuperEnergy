using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour
{
    private const float Speed = 2;
    private static readonly Color HurtColourTint = new Color(1, 0.55f, 0.55f);

    private float health = 100;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;
    private float lastTakeDamageTime;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (Time.time > lastTakeDamageTime + 0.05f && spriteRenderer.color != Color.white)
        {
            spriteRenderer.color = Color.white;
        }

        Vector2 direction = playerController.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.position = Vector2.MoveTowards(transform.position, playerController.transform.position - transform.right, Speed * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }

        spriteRenderer.color = HurtColourTint;
        lastTakeDamageTime = Time.time;
    }

    public static void Create(GameObject prefab, Vector3 position, PlayerController playerController)
    {
        GameObject enemyGameObject = Instantiate(prefab, position, Quaternion.identity);
        Enemy enemyInstance = enemyGameObject.AddComponent<Enemy>();
        enemyInstance.playerController = playerController;
    }

}
