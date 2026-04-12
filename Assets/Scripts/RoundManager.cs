using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private GameObject goalCanvas;
    private GameObject currentGoalCanvas;
    
    public void ReportGoalEnter(string entityName)
    {
        Debug.Log($"{entityName} entered goal.");
        currentGoalCanvas = Instantiate(goalCanvas);
    }
    
    public void ReportGoalExit(string entityName)
    {
        Debug.Log($"{entityName} exited goal.");
        Destroy(currentGoalCanvas);
    }
}
