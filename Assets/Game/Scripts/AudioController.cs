using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Current { get; private set; }

    public AudioSource AudioSource => audioSource;

    [SerializeField]
    private AudioSource audioSource;

    private void Start()
    {
        Current = this;
    }
}