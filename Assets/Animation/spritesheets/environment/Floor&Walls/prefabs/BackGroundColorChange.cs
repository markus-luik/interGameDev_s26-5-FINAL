using UnityEngine;

public class BackGroundColorChange : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private SpriteRenderer targetRenderer;

    [Header("Hue Animation")]
    [SerializeField, Range(0f, 1f)] private float centerHue = 0.12f;
    [SerializeField, Range(0f, 0.5f)] private float hueRange = 0.06f;
    [SerializeField] private float cycleSeconds = 4f;

    private float cachedSaturation;
    private float cachedValue;
    private float cachedAlpha;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<SpriteRenderer>();

        if (targetRenderer == null)
            return;

        Color.RGBToHSV(targetRenderer.color, out _, out cachedSaturation, out cachedValue);
        cachedAlpha = targetRenderer.color.a;
    }

    private void Update()
    {
        if (targetRenderer == null)
            return;

        float duration = Mathf.Max(0.01f, cycleSeconds);
        float t = Mathf.PingPong(Time.time / duration, 1f);
        float hue = Mathf.Repeat(centerHue + Mathf.Lerp(-hueRange, hueRange, t), 1f);

        Color c = Color.HSVToRGB(hue, cachedSaturation, cachedValue);
        c.a = cachedAlpha;
        targetRenderer.color = c;
    }
}
