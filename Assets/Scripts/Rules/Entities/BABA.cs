using System;
using UnityEngine;

public class BABA : MonoBehaviour
{
    private string myName = "BABA";
    
    //Components
    private NewBlock2D _block2D;
    private PlayerRotation _playerRotation;
    private PlayerShooting _playerShooting;
    private PlayerDamaged _playerDamaged;
    
    private void Awake()
    {
        // Getting component references
        //Block2D (current player movement)
        _block2D = GetComponent<NewBlock2D>();
        if (_block2D == null) {Debug.Log($"{myName} does not have a Player2D!");}
        //Player Rotation
        _playerRotation = GetComponent<PlayerRotation>();
        if (_playerRotation == null){Debug.Log($"{myName} does not have a PlayerRotation!");}
        //Player Shooting
        _playerShooting = GetComponent<PlayerShooting>();
        if (_playerShooting == null) {Debug.Log($"{myName} does not have a PlayerShooting!");}
        //Player Damaged
        _playerDamaged = GetComponent<PlayerDamaged>();
        if (_playerDamaged == null){Debug.Log($"{myName} does not have a PlayerDamaged!");}
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
    
    void OnIsYou(string nameCalled, bool isYou)
    {
        if (nameCalled == myName){
            Debug.Log(isYou ? $"{nameCalled} is now YOU!" : $"{nameCalled} is NOT YOU!");

            if (_block2D != null) _block2D.isPlayer = isYou;
            if (_playerRotation != null) _playerRotation.enabled = isYou;
            if (_playerShooting != null) _playerShooting.enabled = isYou;
            if (_playerDamaged != null) _playerDamaged.enabled = isYou;
        }
    }
}
