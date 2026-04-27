using UnityEngine;

public class Goal : MonoBehaviour
{
    //Boolean, just in case
    private bool PlayerInGoal = false;
    private GameObject _roundManagerObj;
    private RoundManager _roundManager;

    //What tag to check for
    private string CheckFor = "Player";
    
    /// <summary>
    /// Finds the RoundManager object and gets its RoundManager script.
    /// </summary>
    void Awake()
    {
        _roundManagerObj = GameObject.FindWithTag("RoundManager");
        _roundManager = _roundManagerObj.GetComponent<RoundManager>();
    }
    
    /// <summary>
    /// Checks for collision ENTRY with CheckFor tagged object.
    /// Reports to _roundManager if Player (or another CheckFor) enters collider.
    /// </summary>
    /// <param name="col">collider</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(CheckFor))
        {
            PlayerInGoal = true;
           _roundManager.ReportGoalEnter(CheckFor);
        }
    }
    
    /// <summary>
    /// Checks for collision EXIT with CheckFor tagged object.
    /// Reports to _roundManager if Player (or another CheckFor) exits collider.
    /// </summary>
    /// <param name="col">collider</param>
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag(CheckFor))
        {
            PlayerInGoal = false;
            _roundManager.ReportGoalExit(CheckFor);
        }
    }
}

