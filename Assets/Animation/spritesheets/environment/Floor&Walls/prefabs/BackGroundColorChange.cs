using UnityEngine;

public class BackGroundColorChange : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private SpriteRenderer targetRenderer;

    [Header("Hue Animation")]
    [SerializeField] private float hueSpeed = 0.1f; // Hue units per second (0..1 wrap)

    private float baseHue;
    private float cachedSaturation;
    private float cachedValue;
    private float cachedAlpha;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<SpriteRenderer>();

        if (targetRenderer == null)
            return;

        Color.RGBToHSV(targetRenderer.color, out baseHue, out cachedSaturation, out cachedValue);
        cachedAlpha = targetRenderer.color.a;
    }

    private void Update()
    {
        if (targetRenderer == null)
            return;

        float hue = Mathf.Repeat(baseHue + Time.time * hueSpeed, 1f);

        Color c = Color.HSVToRGB(hue, cachedSaturation, cachedValue);
        c.a = cachedAlpha;
        targetRenderer.color = c;
    }
}
