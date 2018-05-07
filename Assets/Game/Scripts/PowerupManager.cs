using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupManager : MonoBehaviour
{
    public static PowerupManager Current { get; private set; }

    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip powerupAudioClip;

    [SerializeField]
    private GameObject powerupUiRoot;
    [SerializeField]
    private GameObject powerupItemPrefab;

    private HashSet<PowerupType> activePowerups;

    private void Start()
    {
        Current = this;
        activePowerups = new HashSet<PowerupType>();
    }

    public void UsePowerup(PowerupType type, float time, Sprite powerupIcon)
    {
        if (activePowerups.Contains(type)) return;

        if (type == PowerupType.HealthPack)
        {
            playerController.AddHealth(20);
        }

        audioSource.PlayOneShot(powerupAudioClip);

        if (time == 0) return;

        GameObject powerupIconGameObject = Instantiate(powerupItemPrefab);
        powerupIconGameObject.GetComponent<Image>().sprite = powerupIcon;
        powerupIconGameObject.transform.SetParent(powerupUiRoot.transform, false);

        StartCoroutine(StartPowerupCooldown(type, time, powerupIconGameObject));
        activePowerups.Add(type);
    }

    private IEnumerator StartPowerupCooldown(PowerupType type, float time, GameObject powerupIconGameObject)
    {
        Slider slider = powerupIconGameObject.transform.GetChild(0).GetComponent<Slider>();
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            slider.value = Mathf.Lerp(1, 0, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        Destroy(powerupIconGameObject);
        activePowerups.Remove(type);
    }

    public bool IsPowerupActive(PowerupType type) => activePowerups.Contains(type);
}
