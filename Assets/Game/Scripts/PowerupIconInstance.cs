using UnityEngine;
using UnityEngine.UI;

public class PowerupIconInstance : MonoBehaviour
{
    private PowerupHandle powerupHandle;
    private Slider timerSlider;
    private float elapsedTime;

    public void Initialize(PowerupHandle handle)
    {
        powerupHandle = handle;
    }

    private void Start()
    {
        timerSlider = transform.Find("Timer").GetComponent<Slider>();
    }

    private void Update()
    {
        if (elapsedTime < powerupHandle.Duration)
        {
            timerSlider.value = Mathf.Lerp(1, 0, elapsedTime / powerupHandle.Duration);
            elapsedTime += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
            PowerupManager.Current.RemovePowerup(powerupHandle.Type);
        }
    }
}
