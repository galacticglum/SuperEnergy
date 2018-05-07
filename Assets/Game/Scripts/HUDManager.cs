using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    [SerializeField]
    private AudioClip gameoverAudioClip;
    [SerializeField]
    private CanvasGroup hud;
    [SerializeField]
    private CanvasGroup gameover;
    private bool completeTransition;

    private void Start()
    {
        gameover.alpha = 0;
        hud.alpha = 1;
    }

    private void Update()
    {
        if (!playerController.IsGameover) return;
        if (!completeTransition)
        {
            hud.alpha = 0;
            StartCoroutine(TransitionToGameover());
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
            }
            else if(Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    private IEnumerator TransitionToGameover()
    {
        completeTransition = true;

        AudioController.Current.AudioSource.Stop();
        AudioController.Current.AudioSource.PlayOneShot(gameoverAudioClip);

        const float transitionTime = 1f;
        float elapsedTime = 0;

        while (elapsedTime < transitionTime)
        {
            gameover.alpha = Mathf.Lerp(0, 1, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}