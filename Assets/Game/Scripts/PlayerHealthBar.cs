using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private Slider backingHealthBarSlider;
    private Slider healthBarSlider;

    private void Start()
    {
        healthBarSlider = GetComponent<Slider>();
        playerController.PlayerHealthChanged += OnPlayerHealthChanged;
    }

    private void OnPlayerHealthChanged(object sender, PlayerHealthChangedEventArgs args)
    {
        backingHealthBarSlider.value = args.OldHealth / args.PlayerController.MaxHealth;
        StartCoroutine(TransitionHealthBar(args.NewHealth / args.PlayerController.MaxHealth));
    }

    private IEnumerator TransitionHealthBar(float destination)
    {
        const float transitionInTime = 0.2f;
        float elapsedTime = 0;

        while (elapsedTime < transitionInTime)
        {
            healthBarSlider.value = Mathf.Lerp(healthBarSlider.value, destination, elapsedTime / transitionInTime);
            elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }
}
