using System;
using UnityEngine;

public class FLAG : MonoBehaviour
{
    private string myName = "FLAG";
    private bool canMove = true;
    
    //Components
    private Player2D _player2D;
    private PlayerRotation _playerRotation;
    
    private void Awake()
    {
        // Getting component references
        _player2D = GetComponent<Player2D>();
        if (_player2D == null) {Debug.Log($"{myName} does not have a Player2D!");}
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
            if (isYou){
                Debug.Log($"{name} is now YOU!");
                if (_player2D != null && _playerRotation != null){
                    _player2D.enabled =  true;
                    _playerRotation.enabled = true;
                }
            }
            else
            {
                Debug.Log($"{name} is NOT YOU!");
                if (_player2D != null && _playerRotation != null){
                    _player2D.enabled = false;
                    _playerRotation.enabled = false;
                }
            }
            canMove =  isYou;
        }
    }
}
