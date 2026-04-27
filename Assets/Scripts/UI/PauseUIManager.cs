using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseUIManager : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    [Header("Pause UI Objects")]
    [SerializeField] private RectTransform leftPanel;
    [SerializeField] private RectTransform bottomPanel;
    [SerializeField] private TMP_Text pauseText;
    [SerializeField] private RawImage glitchImage;
    [SerializeField] private Image darkenImage;

    [Header("Flash")]
    [SerializeField] private Image flashImage;
    [SerializeField] private float flashDuration = 0.5f;

    [Header("Positions")]
    [SerializeField] private Vector2 leftPanelShownPos;
    [SerializeField] private Vector2 leftPanelHiddenPos;

    [SerializeField] private Vector2 bottomPanelShownPos;
    [SerializeField] private Vector2 bottomPanelHiddenPos;

    [Header("Timing")]
    [SerializeField] private float panelMoveDuration = 0.35f;

    private bool isPaused = false;
    private bool isAnimating = false;

    private void Start()
    {
        HidePauseInstant();
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey) && !isAnimating)
        {
            if (isPaused)
                StartCoroutine(ClosePause());
            else
                StartCoroutine(OpenPause());
        }
    }

    private void HidePauseInstant()
    {
        if (leftPanel != null)
            leftPanel.anchoredPosition = leftPanelHiddenPos;

        if (bottomPanel != null)
            bottomPanel.anchoredPosition = bottomPanelHiddenPos;

        if (pauseText != null)
            pauseText.gameObject.SetActive(false);

        if (glitchImage != null)
            glitchImage.gameObject.SetActive(false);

        if (darkenImage != null)
            darkenImage.gameObject.SetActive(false);

        if (flashImage != null)
        {
            Color c = flashImage.color;
            c.a = 0f;
            flashImage.color = c;
            flashImage.gameObject.SetActive(false);
        }
    }

    private IEnumerator OpenPause()
    {
        isAnimating = true;
        isPaused = true;

        //freeze game
        Time.timeScale = 0f;

        if (pauseText != null)
            pauseText.gameObject.SetActive(false);

        yield return StartCoroutine(FlashScreen());

        if (leftPanel != null)
        {
            StartCoroutine(MoveRectUnscaled(
                leftPanel,
                leftPanelHiddenPos,
                leftPanelShownPos,
                panelMoveDuration
            ));
        }

        if (bottomPanel != null)
        {
            yield return StartCoroutine(MoveRectUnscaled(
                bottomPanel,
                bottomPanelHiddenPos,
                bottomPanelShownPos,
                panelMoveDuration
            ));
        }

        if (pauseText != null)
            pauseText.gameObject.SetActive(true);
    
        if (glitchImage != null)
            glitchImage.gameObject.SetActive(true);

        if (darkenImage != null)
            darkenImage.gameObject.SetActive(true);

        isAnimating = false;
    }

    private IEnumerator ClosePause()
    {
        isAnimating = true;

        if (pauseText != null)
            pauseText.gameObject.SetActive(false);

        if (glitchImage != null)
            glitchImage.gameObject.SetActive(false);

        if (darkenImage != null)
            darkenImage.gameObject.SetActive(false);

        if (leftPanel != null)
        {
            StartCoroutine(MoveRectUnscaled(
                leftPanel,
                leftPanelShownPos,
                leftPanelHiddenPos,
                panelMoveDuration
            ));
        }

        if (bottomPanel != null)
        {
            yield return StartCoroutine(MoveRectUnscaled(
                bottomPanel,
                bottomPanelShownPos,
                bottomPanelHiddenPos,
                panelMoveDuration
            ));
        }

        isPaused = false;
        Time.timeScale = 1f;
        isAnimating = false;
    }

    private IEnumerator FlashScreen()
    {
        if (flashImage == null)
            yield break;

        flashImage.gameObject.SetActive(true);

        float time = 0f;

        while (time < flashDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / flashDuration;

            Color c = flashImage.color;

            //flash in then out
            if (t < 0.5f)
                c.a = Mathf.Lerp(0f, 1f, t * 2f);
            else
                c.a = Mathf.Lerp(1f, 0f, (t - 0.5f) * 2f);

            flashImage.color = c;

            yield return null;
        }

        Color final = flashImage.color;
        final.a = 0f;
        flashImage.color = final;
        flashImage.gameObject.SetActive(false);
    }

    private IEnumerator MoveRectUnscaled(RectTransform rect, Vector2 from, Vector2 to, float duration)
    {
        float time = 0f;
        rect.anchoredPosition = from;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            t = Mathf.SmoothStep(0f, 1f, t);

            rect.anchoredPosition = Vector2.Lerp(from, to, t);

            yield return null;
        }

        rect.anchoredPosition = to;
    }

    public bool IsPaused()
    {
        return isPaused;
    }
    public void ResumeGame()
    {
        if (!isPaused) return;

        StartCoroutine(ClosePause());
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game");

        Application.Quit();

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
}