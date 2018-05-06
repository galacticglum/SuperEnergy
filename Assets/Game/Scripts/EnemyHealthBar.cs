using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider healthBarSlider;
    private Enemy enemy;

    public void Initialize(Enemy enemy)
    {
        this.enemy = enemy;
    }

	private void Update ()
	{
	    healthBarSlider.value = enemy.CurrentHealth / enemy.MaxHealth;
	    transform.position = Vector3.up * 1.5f + transform.parent.position;
	    transform.rotation = Quaternion.Euler(0, 0, transform.parent.rotation.z * -1);
	}
}
