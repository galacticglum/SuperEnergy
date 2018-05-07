using UnityEngine;

public class Socket : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private float defaultRange = 10;
    [SerializeField]
    private GameObject rangeCircle;
    [SerializeField]
    private float rangeCircleRotationSpeed = 80;
    private float range;

    private bool pluggedIn;

    private void Start()
    {
        rangeCircle.SetActive(false);
    }

    private void Update()
    {
        range = PowerupManager.Current.IsPowerupActive(PowerupType.MegaRange) ? defaultRange * 1.5f : defaultRange;
        rangeCircle.transform.localScale = new Vector3(range / transform.localScale.x, range / transform.localScale.y, 0) * 2;

        if (pluggedIn)
        {
            rangeCircle.transform.Rotate(0, 0, rangeCircleRotationSpeed * Time.deltaTime);
        }

        float distance = (playerController.transform.position - transform.position).magnitude;
        if (pluggedIn)
        {
            if (!(distance > range)) return;

            playerController.CanShoot = false;
            pluggedIn = false;
            rangeCircle.SetActive(false);
        }
        else
        {
            if (!(distance <= 2)) return;

            playerController.CanShoot = true;
            pluggedIn = true;
            rangeCircle.SetActive(true);
        }
    }
}
