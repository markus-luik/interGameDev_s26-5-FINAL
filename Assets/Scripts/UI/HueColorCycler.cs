using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotlineMiamiColorCycler : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image image;

    [Header("Timing")]
    [SerializeField] private float colorChangeDuration = 1.2f;

    [Header("Hotline Miami Palette")]
    [SerializeField] private Color[] neonColors = new Color[]
    {
        new Color(1f, 0.1f, 0.6f),   // hot pink
        new Color(0.1f, 1f, 1f),     // cyan
        new Color(0.6f, 0.1f, 1f),   // purple
        new Color(1f, 0.9f, 0.1f),   // yellow
        new Color(1f, 0.2f, 0.2f),   // red
        new Color(0.1f, 1f, 0.5f)    // green
    };

    [Header("Gradient")]
    [SerializeField] private bool useTextGradient = true;
    [SerializeField] private float bottomDarken = 0.65f;

    private int currentIndex;
    private int nextIndex;
    private float timer;

    private void Start()
    {
        // if (neonColors == null || neonColors.Length < 2)
        // {
        //     Debug.LogError("Need at least 2 neon colors.");
        //     enabled = false;
        //     return;
        // }

        currentIndex = Random.Range(0, neonColors.Length);
        nextIndex = GetNextIndex();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / colorChangeDuration);

        t = Mathf.SmoothStep(0f, 1f, t);

        Color mainColor = Color.Lerp(neonColors[currentIndex], neonColors[nextIndex], t);

        if (text != null)
        {
            if (useTextGradient)
            {
                Color bottomColor = mainColor * bottomDarken;
                bottomColor.a = 1f;

                text.colorGradient = new VertexGradient(
                    mainColor,    // top left
                    mainColor,    // top right
                    bottomColor,  // bottom left
                    bottomColor   // bottom right
                );
            }
            else
            {
                text.color = mainColor;
            }
        }

        if (image != null)
        {
            image.color = mainColor;
        }

        if (timer >= colorChangeDuration)
        {
            timer = 0f;
            currentIndex = nextIndex;
            nextIndex = GetNextIndex();
        }
    }

    private int GetNextIndex()
    {
        int newIndex = currentIndex;
        while (newIndex == currentIndex)
        {
            newIndex = Random.Range(0, neonColors.Length);
        }
        return newIndex;
    }
}