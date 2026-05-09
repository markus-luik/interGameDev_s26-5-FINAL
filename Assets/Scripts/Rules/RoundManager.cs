using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;
    //Scene variables
    private int currentSceneIndex = 0;
    private int nextSceneIndex = 0;
    //Finishing logic
    private bool playerFinished = false;
    //Canvas prefab that signals round end
    [SerializeField] private GameObject goalCanvas;
    private GameObject currentGoalCanvas;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> levelClearClips = new List<AudioClip>();

    private void Awake()
    {
        if (Instance != null && Instance != this)//If RoundManager exists && it is not itself, destroy that other game manager
        {
            Debug.Log("Other RoundManager detected, will destroy...");
            Destroy(gameObject);

        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
        }
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //SCENE INDEXES
        Debug.Log($"OnSceneLoaded: {scene.name}");
        Debug.Log(mode);
        Debug.Log("Setting scene indexes:");
        //Current scene index
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log($"Current scene: {currentSceneIndex}");
        //Next scene index
        nextSceneIndex = currentSceneIndex + 1;
        Debug.Log($"Next scene: {nextSceneIndex}");
        Debug.Log("FINISHED setting Scene Indexes");

        ReportGoalExit("Player");
    }
    
    void LoadNextScene()
    {
        if(playerFinished && currentSceneIndex > 0 || currentSceneIndex == 0 || Input.GetKey(KeyCode.LeftShift)){ //If player finished in one of the levels OR player is on start screen OR holding Left Shift overrride
            if (Input.GetKey(KeyCode.Space))
            {
                if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                {
                    Debug.Log("Loading next scene...");
                    SceneManager.LoadScene(nextSceneIndex);
                }
                else
                {
                    nextSceneIndex = 0;
                    Debug.Log("LAST scene! Loading 0th scene...");
                    SceneManager.LoadScene(nextSceneIndex);
                }

                ReportGoalExit("Player");
            }
        }
    }
    
    //--------------------------------- GOAL ENTER & EXIT
    /// <summary>
    /// Triggers when player ENTERS Goal.
    /// Reported by Goal.
    /// Spawns goalCanvas and gets its reference.
    /// </summary>
    /// <param name="entityName">Name of entity (usually player but set in Goal.cs) that entered Goal</param>
    public void ReportGoalEnter(string entityName)
    {
        Debug.Log($"{entityName} entered goal.");
        currentGoalCanvas = Instantiate(goalCanvas);
        playerFinished = true;
        PlayVictorySFX();
    }
    
    /// <summary>
    /// Triggers when player EXITS Goal.
    /// Reported by Goal
    /// Destroys previously spawned currentGoalCanvas
    /// </summary>
    /// <param name="entityName">Name of entity (usually player but set in Goal.cs) that exited Goal</param>
    public void ReportGoalExit(string entityName)
    {
        Debug.Log($"{entityName} exited goal.");
        Destroy(currentGoalCanvas);
        playerFinished = false;
    }
    //---------------------------------
    
    //--------------------------------- RELOAD SCENE
    /// <summary>
    /// Restarts scene when Left-Shift and R have been pressed
    /// </summary>
    void ReloadScene()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("Reloading scene...");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
    //--------------------------------- 


    private void PlayVictorySFX()
    {
        if (levelClearClips != null && levelClearClips.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, levelClearClips.Count);
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(levelClearClips[index]);
        }
    }
    
    //--------------------------------- UPDATE
    void Update()
    {
        //ReloadScene();
        LoadNextScene();
    }
    //---------------------------------
}

