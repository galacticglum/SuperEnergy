using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Shadow))]
public class ShadowText : MonoBehaviour
{
    private Shadow shadow; 

    [SerializeField]
    private float start;
    [SerializeField]
    private float end = 3.0f;

    private void Start()
    {
        shadow = GetComponent<Shadow>();
    }

    private void Update()
    {
        shadow.effectDistance = new Vector2(Mathf.PingPong(Time.time * 2, end - start) + start, -1);
    }
}
