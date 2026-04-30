using System;
using UnityEngine;

public class BABA : MonoBehaviour
{
    private string myName = "BABA";
    
    //Components
    private NewBlock2D _block2D;
    private PlayerRotation _playerRotation;
    
    private void Awake()
    {
        // Getting component references
        _block2D = GetComponent<NewBlock2D>();
        if (_block2D == null) {Debug.Log($"{myName} does not have a Player2D!");}
        _playerRotation = GetComponent<PlayerRotation>();
        if (_playerRotation == null){Debug.Log($"{myName} does not have a PlayerRotation!");}
    }
    
    private void OnEnable()
    {
        Debug.Log($"{myName} OnEnable");
        EventBroadcaster.IsYou += OnIsYou;
    }

    private void OnDisable()
    {
        Debug.Log($"{myName} OnDisable");
        EventBroadcaster.IsYou -= OnIsYou;
    }
    
    void OnIsYou(string name, bool isYou)
    {
        if (name == myName){
            Debug.Log(isYou ? $"{name} is now YOU!" : $"{name} is NOT YOU!");

            if (_block2D != null) _block2D.isPlayer = isYou;
            if (_playerRotation != null) _playerRotation.enabled = isYou;
        }
    }
}
