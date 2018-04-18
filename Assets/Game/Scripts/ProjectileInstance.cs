using UnityEngine;

public class ProjectileInstance : MonoBehaviour
{
    private const float Lifetime = 2;

    private float spawnTime;
    private new Rigidbody2D rigidbody2D;
    
    private void Initialize(Vector3 velocity)
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        if (rigidbody2D == null)
        {
            rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
        }

        rigidbody2D.velocity = velocity;
        rigidbody2D.angularVelocity = 0;

        spawnTime = Time.time;
    }

    private void Update()
    {
        if (Time.time > spawnTime + Lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Obstacle")) return;
        rigidbody2D.velocity = Vector2.zero;
        Destroy(gameObject);
    }

    public static ProjectileInstance Create(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        GameObject projectileInstanceGameObject = Instantiate(prefab, position, rotation);
        ProjectileInstance instance = projectileInstanceGameObject.AddComponent<ProjectileInstance>();
        instance.Initialize(velocity);

        return instance;
    }
}
