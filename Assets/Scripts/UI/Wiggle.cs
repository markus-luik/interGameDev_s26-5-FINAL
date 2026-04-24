using UnityEngine;

public class Wiggle : MonoBehaviour
{
    public float speed = 2f;
    public float amount = 15f;

    private Vector3 startRotation;

    void Start()
    {
        startRotation = transform.eulerAngles;
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * speed) * amount;
        float y = Mathf.Cos(Time.time * speed) * amount;

        transform.eulerAngles = startRotation + new Vector3(x, y, 0);
    }
}