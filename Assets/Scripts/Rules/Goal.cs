using UnityEngine;

public class Goal : MonoBehaviour
{
    //Boolean, just in case
    [SerializeField, ReadOnly] private bool PlayerInGoal = false;
    private GameObject _roundManagerObj;
    private RoundManager _roundManager;

    //What tag to check for
    [SerializeField] private string checkForTag = "Player";
    
    /// <summary>
    /// Finds the RoundManager object and gets its RoundManager script.
    /// </summary>
    void Awake()
    {
        _roundManagerObj = GameObject.FindWithTag("RoundManager");
        _roundManager = _roundManagerObj.GetComponent<RoundManager>();
    }
    
    /// <summary>
    /// Checks for collision ENTRY with checkForTag tagged object.
    /// Reports to _roundManager if Player (or another checkForTag) enters collider.
    /// </summary>
    /// <param name="col">collider</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(checkForTag))
        {
            PlayerInGoal = true;
           _roundManager.ReportGoalEnter(checkForTag);
        }
    }
    
    /// <summary>
    /// Checks for collision EXIT with checkForTag tagged object.
    /// Reports to _roundManager if Player (or another checkForTag) exits collider.
    /// </summary>
    /// <param name="col">collider</param>
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag(checkForTag))
        {
            PlayerInGoal = false;
            _roundManager.ReportGoalExit(checkForTag);
        }
    }
}

