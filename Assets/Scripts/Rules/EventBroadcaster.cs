using UnityEngine;

public class EventBroadcaster : MonoBehaviour
{
    public delegate void TestBroadcastDelegate();
    public static event TestBroadcastDelegate OnTestCast1;

    public delegate void RuleBroadcastDelegate(string entityName, bool ruleActive);
    public static event RuleBroadcastDelegate IsYou;
    public static event RuleBroadcastDelegate IsWin;
    private bool BabaIsYou = false;
    private bool FlagIsYou = false;
    private bool BabaIsWin = false;
    private bool FlagIsWin = false;
    
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
        if (Input.GetKey(KeyCode.Y)){
            if (Input.GetKeyDown(KeyCode.B))
            {   
                if(!BabaIsYou){
                    Debug.Log("Y & B was pressed, Baba is You");
                    IsYou?.Invoke(entityName: "BABA", ruleActive: true);
                    BabaIsYou = true;
                }else{
                    Debug.Log("Y & B was pressed, Baba is not You");
                    IsYou?.Invoke(entityName: "BABA", ruleActive: false);
                    BabaIsYou = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.F))
            {   
                if(!FlagIsYou){
                    Debug.Log("Y & F was pressed, Flag is You");
                    IsYou?.Invoke(entityName: "FLAG", ruleActive: true);
                    FlagIsYou = true;
                }else{
                    Debug.Log("Y & F was pressed, Flag is not You");
                    IsYou?.Invoke(entityName: "FLAG", ruleActive: false);
                    FlagIsYou = false;
                }
            }
        }
        if (Input.GetKey(KeyCode.G)){
            if (Input.GetKeyDown(KeyCode.B))
            {   
                if(!BabaIsWin){
                    Debug.Log("G & B was pressed, Baba is Win");
                    IsWin?.Invoke(entityName: "BABA", ruleActive: true);
                    BabaIsWin = true;
                }else{
                    Debug.Log("G & B was pressed, Baba is not Win");
                    IsWin?.Invoke(entityName: "BABA", ruleActive: false);
                    BabaIsWin = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.F))
            {   
                if(!FlagIsWin){
                    Debug.Log("G & F was pressed, Flag is Win");
                    IsWin?.Invoke(entityName: "FLAG", ruleActive: true);
                    FlagIsWin = true;
                }else{
                    Debug.Log("G & F was pressed, Flag is not Win");
                    IsWin?.Invoke(entityName: "FLAG", ruleActive: false);
                    FlagIsWin = false;
                }
            }
        }
    }
    
    void Update()
    {
        ReadInput();
    }
}
