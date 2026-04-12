using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{
    //Canvas prefab that signals round end
    [SerializeField] private GameObject goalCanvas;
    private GameObject currentGoalCanvas;
    
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
    
    //--------------------------------- UPDATE
    void Update()
    {
        ReloadScene();
    }
    //---------------------------------
}
