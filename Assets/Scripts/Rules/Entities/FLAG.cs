using System;
using System.Collections.Generic;
using UnityEngine;

public class FLAG : MonoBehaviour
{
    private string myName = "FLAG";
    
    //Components
    private NewBlock2D  _block2D;
    private PlayerRotation _playerRotation;
    //Goal prefab
    [SerializeField] private GameObject _prefabGoal;
    private GameObject _spawnedGoal;

    [Header("Audio")]
    [SerializeField] private AudioSource speakingAudioSource;
    [SerializeField] private List<AudioClip> statementMadeClips = new List<AudioClip>();

    private void Awake()
    {
        // Getting component references
        _block2D = GetComponent<NewBlock2D>();
        if (_block2D == null) {Debug.Log($"{myName} does not have a NewBlock2D!");}
        _playerRotation = GetComponent<PlayerRotation>();
        if (_playerRotation == null){Debug.Log($"{myName} does not have a PlayerRotation!");}
    }
    
    private void OnEnable()
    {
        Debug.Log($"{myName} OnEnable");
        EventBroadcaster.IsYou += OnIsYou;
        EventBroadcaster.IsWin += OnIsWin;
    }

    private void OnDisable()
    {
        Debug.Log($"{myName} OnDisable");
        EventBroadcaster.IsYou -= OnIsYou;
        EventBroadcaster.IsWin -= OnIsWin;
    }

    void OnIsWin(string who, bool what)
    {
        if (who == myName){
            if (what)
            {
                Debug.Log($"{myName} is WIN");
                if (_spawnedGoal == null){
                    _spawnedGoal = Instantiate(_prefabGoal, transform.position, Quaternion.identity); //spawns prefab goal at self with no rotation
                }

                // Play Statement Made SFX
                PlayStatementSound();

            }
            else
            {
                Destroy(_spawnedGoal);
            }
        }
    }
    
    void OnIsYou(string who, bool what)
    {
        if (who == myName){
            Debug.Log(what ? $"{who} is now YOU!" : $"{who} is NOT YOU!");

            if (_block2D != null) _block2D.isPlayer = what;
            if (_playerRotation != null) _playerRotation.enabled = what;
        }
    }

    public void PlayStatementSound()
    {
        if (statementMadeClips != null && statementMadeClips.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, statementMadeClips.Count);
            speakingAudioSource.PlayOneShot(statementMadeClips[index]);
        }
    }

}
