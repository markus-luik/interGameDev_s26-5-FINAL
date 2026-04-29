using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConversationUIManager : MonoBehaviour
{
    public static ConversationUIManager Instance;

    [Header("Black Bars")]
    [SerializeField] private RectTransform topBar;
    [SerializeField] private RectTransform bottomBar;
    [SerializeField] private float barMoveDuration = 0.5f;


    [Header("Images")]
    [SerializeField] private Image characterBackgroundImage;
    [SerializeField] private Image characterImage;
    [SerializeField] private float characterFlyDuration = 0.5f;



    [Header("Dialogue UI")]
    //[SerializeField] private GameObject conversationPanel;
    [SerializeField] private TMP_Text dialogueText;


    [Header("Black Bars Positions")]
    [SerializeField] private Vector2 topBarShownPos;
    [SerializeField] private Vector2 topBarHiddenPos;
    [SerializeField] private Vector2 bottomBarShownPos;
    [SerializeField] private Vector2 bottomBarHiddenPos;

    [Header("Character Positions")]
    [SerializeField] private Vector2 characterShownPos;
    [SerializeField] private Vector2 characterHiddenPos;
    [SerializeField] private Vector2 characterBgShownPos;
    [SerializeField] private Vector2 characterBgHiddenPos;

    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.02f;

    private List<DialogueLine> currentLines = new List<DialogueLine>();
    private int currentLineIndex = 0;
    private Sprite currentDisplayedSprite;
    private bool isTyping = false;
    // public bool conversationHasEnded = false;
    private Coroutine typingCoroutine;

    // private Vector2 topBarHiddenPos;
    // private Vector2 topBarShownPos;
    // private Vector2 bottomBarHiddenPos;
    // private Vector2 bottomBarShownPos;

    // private Vector2 characterHiddenPos;

    private bool conversationActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {

        
        HideUIInstant();
    }

    private void Update()
    {
        if (!conversationActive) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            NextLine();
        }
    }

   

    private void HideUIInstant()
    {
        if (topBar != null)
            topBar.anchoredPosition = topBarHiddenPos;

        if (bottomBar != null)
            bottomBar.anchoredPosition = bottomBarHiddenPos;

        if (characterBackgroundImage != null)
        {
            characterBackgroundImage.rectTransform.anchoredPosition = characterBgHiddenPos;
            characterBackgroundImage.gameObject.SetActive(false);
        }

        if (characterImage != null)
        {
            characterImage.rectTransform.anchoredPosition = characterHiddenPos;
            characterImage.gameObject.SetActive(false);
        }

        // if (backgroundImage != null)
        //     backgroundImage.gameObject.SetActive(false);

        if (dialogueText != null)
            dialogueText.text = "";
    }

    public void StartConversation(List<DialogueLine> lines)
    {
        if (conversationActive) return;

        currentLines = lines;
        currentLineIndex = 0;

        //reset sprite
        currentDisplayedSprite = null;

        StartCoroutine(ConversationIntro());
    }
    private IEnumerator ConversationIntro()
    {
        conversationActive = true;

        if (characterBackgroundImage != null)
        {
            characterBackgroundImage.gameObject.SetActive(true);
            characterBackgroundImage.rectTransform.anchoredPosition = characterBgHiddenPos;
        }

        if (characterImage != null)
        {
            characterImage.gameObject.SetActive(true);
            characterImage.rectTransform.anchoredPosition = characterHiddenPos;
        }

        yield return StartCoroutine(MoveBars(
            topBarHiddenPos, topBarShownPos,
            bottomBarHiddenPos, bottomBarShownPos,
            barMoveDuration));

        if (characterBackgroundImage != null)
        {
            StartCoroutine(MoveRect(
                characterBackgroundImage.rectTransform,
                characterBgHiddenPos,
                characterBgShownPos,
                characterFlyDuration));
        }

        ShowLine();
    }

    private IEnumerator MoveBars(Vector2 topFrom, Vector2 topTo, Vector2 bottomFrom, Vector2 bottomTo, float duration)
    {
        float time = 0f;

        topBar.anchoredPosition = topFrom;
        bottomBar.anchoredPosition = bottomFrom;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            t = Mathf.SmoothStep(0f, 1f, t);

            topBar.anchoredPosition = Vector2.Lerp(topFrom, topTo, t);
            bottomBar.anchoredPosition = Vector2.Lerp(bottomFrom, bottomTo, t);

            yield return null;
        }

        topBar.anchoredPosition = topTo;
        bottomBar.anchoredPosition = bottomTo;
    }

    private IEnumerator MoveCharacter(Vector2 from, Vector2 to, float duration)
    {
        float time = 0f;

        characterImage.rectTransform.anchoredPosition = from;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            t = Mathf.SmoothStep(0f, 1f, t);

            characterImage.rectTransform.anchoredPosition = Vector2.Lerp(from, to, t);

            yield return null;
        }

        characterImage.rectTransform.anchoredPosition = to;
    }

    private void ShowLine()
    {
        if (currentLineIndex < 0 || currentLineIndex >= currentLines.Count)
        {
            EndConversation();
            return;
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        StartCoroutine(ShowLineRoutine());
    }

    private IEnumerator ShowLineRoutine()
    {
        DialogueLine line = currentLines[currentLineIndex];

        bool spriteChanged = line.characterSprite != currentDisplayedSprite;

        if (spriteChanged && characterImage != null)
        {
            currentDisplayedSprite = line.characterSprite;

            //move old sprite out instantly
            characterImage.rectTransform.anchoredPosition = characterHiddenPos;
            characterImage.sprite = currentDisplayedSprite;
            characterImage.gameObject.SetActive(currentDisplayedSprite != null);

            //fly in again
            if (currentDisplayedSprite != null)
            {
                yield return StartCoroutine(MoveRect(
                    characterImage.rectTransform,
                    characterHiddenPos,
                    characterShownPos,
                    characterFlyDuration
                ));
            }
        }

        typingCoroutine = StartCoroutine(TypeLine(line.text));
    }
    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void NextLine()
    {
        if (!conversationActive) return;

        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            dialogueText.text = currentLines[currentLineIndex].text;
            isTyping = false;
            return;
        }

        currentLineIndex++;

        if (currentLineIndex >= currentLines.Count)
        {
            EndConversation();
        }
        else
        {
            ShowLine();
        }
    }

    public void EndConversation()
    {
        StopAllCoroutines();
        StartCoroutine(ConversationOutro());
    }

    private IEnumerator ConversationOutro()
    {
        
        conversationActive = false;
        if (dialogueText != null)
            dialogueText.text = "";

        if (characterImage != null)
        {
            StartCoroutine(MoveRect(
                characterImage.rectTransform,
                characterShownPos,
                characterHiddenPos,
                characterFlyDuration));
        }

        if (characterBackgroundImage != null)
        {
            yield return StartCoroutine(MoveRect(
                characterBackgroundImage.rectTransform,
                characterBgShownPos,
                characterBgHiddenPos,
                characterFlyDuration));
        }

        yield return StartCoroutine(MoveBars(
            topBarShownPos, topBarHiddenPos,
            bottomBarShownPos, bottomBarHiddenPos,
            barMoveDuration));

        HideUIInstant();
    }
    private IEnumerator MoveRect(RectTransform rect, Vector2 from, Vector2 to, float duration)
    {
        float time = 0f;
        rect.anchoredPosition = from;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            t = Mathf.SmoothStep(0f, 1f, t);

            rect.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }

        rect.anchoredPosition = to;
    }
    public bool IsConversationActive()
    {
        return conversationActive;
    }
}