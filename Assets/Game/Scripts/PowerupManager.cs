using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct PowerupHandle
{
    public PowerupType Type { get; }
    public float Duration { get; }
    public GameObject IconGameObject { get; }

    public PowerupHandle(PowerupType type, float duration, GameObject iconGameObject)
    {
        Type = type;
        Duration = duration;
        IconGameObject = iconGameObject;
    }
}

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

    private Dictionary<PowerupType, Queue<PowerupHandle>> powerupQueue;
    private Dictionary<PowerupType, bool> activePowerups;

    private void Start()
    {
        Current = this;
        powerupQueue = new Dictionary<PowerupType, Queue<PowerupHandle>>();
        activePowerups = new Dictionary<PowerupType, bool>();
    }

    private void Update()
    {
        foreach (PowerupType type in powerupQueue.Keys)
        {
            if (activePowerups.ContainsKey(type) && (!activePowerups.ContainsKey(type) || activePowerups[type]) ||
                powerupQueue[type].Count <= 0) continue;

            PowerupHandle activePowerupHandle = powerupQueue[type].Dequeue();
            PowerupIconInstance powerupIconInstance = activePowerupHandle.IconGameObject.AddComponent<PowerupIconInstance>();
            powerupIconInstance.Initialize(activePowerupHandle);
            activePowerups[type] = true;
        }
    }

    public void AddPowerup(PowerupType type, float time, Sprite powerupIcon)
    {
        if (type == PowerupType.HealthPack)
        {
            playerController.AddHealth(20);
        }

        audioSource.PlayOneShot(powerupAudioClip);
        if (time == 0) return;

        GameObject powerupIconGameObject = Instantiate(powerupItemPrefab);
        powerupIconGameObject.GetComponent<Image>().sprite = powerupIcon;
        powerupIconGameObject.transform.SetParent(powerupUiRoot.transform, false);

        if (!powerupQueue.ContainsKey(type))
        {
            powerupQueue[type] = new Queue<PowerupHandle>();
        }

        powerupQueue[type].Enqueue(new PowerupHandle(type, time, powerupIconGameObject));
    }

    public void RemovePowerup(PowerupType type)
    {
        activePowerups[type] = false;
    }

    public bool IsPowerupActive(PowerupType type) => activePowerups.ContainsKey(type) && activePowerups[type];
}
