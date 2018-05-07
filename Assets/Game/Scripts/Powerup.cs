using UnityEngine;

public enum PowerupType
{
    BatteryPack,
    RapidFire,
    MegaRange
}

[RequireComponent(typeof(SpriteRenderer))]
public class Powerup : MonoBehaviour
{
    [SerializeField]
    private PowerupType type;
    [SerializeField]
    private float time;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SpawnController.Current.NewWaveStarted += (sender, args) => SetEnabled(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        PowerupManager.Current.UsePowerup(type, time, spriteRenderer.sprite);

        SetEnabled(false);
    }

    private void SetEnabled(bool toggle)
    {
        spriteRenderer.enabled = toggle;
        GetComponent<Collider2D>().enabled = toggle;
    }
}
