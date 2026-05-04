using UnityEngine;

public class EventBroadcaster : MonoBehaviour
{
    public delegate void TestBroadcastDelegate();
    public static event TestBroadcastDelegate OnTestCast1;

    public delegate void RuleBroadcastDelegate(string entityName, bool ruleActive);
    public static event RuleBroadcastDelegate IsYou;
    private bool BabaIsYou = false;
    private bool FlagIsYou = false;
    
    public delegate void SpaceBarDelegate(string nameToBroadcast);
    public static event SpaceBarDelegate OnSpaceBar;

    [Header("NUMBERS")]
    [SerializeField] private string nameToBroadcast = "BABA";
    
    void ReadInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            OnSpaceBar?.Invoke(nameToBroadcast);
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("I was pressed!");
            OnTestCast1?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {   
            if(!BabaIsYou){
                Debug.Log("B was pressed, Baba is You");
                IsYou?.Invoke(entityName: "BABA", ruleActive: true);
                BabaIsYou = true;
            }else{
                Debug.Log("B was pressed, Baba is not You");
                IsYou?.Invoke(entityName: "BABA", ruleActive: false);
                BabaIsYou = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {   
            if(!FlagIsYou){
                Debug.Log("F was pressed, Flag is You");
                IsYou?.Invoke(entityName: "FLAG", ruleActive: true);
                FlagIsYou = true;
            }else{
                Debug.Log("F was pressed, Flag is not You");
                IsYou?.Invoke(entityName: "FLAG", ruleActive: false);
                FlagIsYou = false;
            }
        }
    }
    
    void Update()
    {
        ReadInput();
    }
}
