using UnityEngine;

public class ProjectileInstance : MonoBehaviour
{
    private new Rigidbody2D rigidbody2D;
    
    private void Initialize(Vector3 velocity)
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        if (rigidbody2D == null)
        {
            rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
        }

        rigidbody2D.velocity = velocity;
    }

    private void Update()
    {

    }

    public static ProjectileInstance Create(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        GameObject projectileInstanceGameObject = Instantiate(prefab, position, rotation);
        ProjectileInstance instance = projectileInstanceGameObject.AddComponent<ProjectileInstance>();
        instance.Initialize(velocity);

        return instance;
    }
}
