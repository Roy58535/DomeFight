using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public bool IsRoundActive => _isRoundActive;

    [SerializeField] private List<GameObject> _activePlayers = new List<GameObject>();
    [SerializeField] private List<GameObject> _inactivePlayers = new List<GameObject>();
    [SerializeField] private Dictionary<GameObject, int> _playerScores = new Dictionary<GameObject, int>();
    [SerializeField] private int _roundTime = 60; // in seconds

    private bool _isRoundActive;
    private float _roundTimer;

    private void Update()
    {
        _roundTimer -= Time.deltaTime;

        if (_activePlayers.Count == 1 && _isRoundActive)
        {
            Debug.Log(_activePlayers[0].name + " is the winner!");
            _playerScores[_activePlayers[0]] += 1; // Increment the score for the winner
            EndRound();
        }
        else if (_roundTimer <= 0 && _isRoundActive)
        {
            Debug.Log("Round time is over!");
            EndRound();
        }
    }

    public void AddPlayer(GameObject player)
    {
        if (!_activePlayers.Contains(player))
        {
            _activePlayers.Add(player);
            EntityInfo entityInfo = player.GetComponent<EntityInfo>();
            if (entityInfo != null)
            {
                entityInfo.OnEntityDeath += RemovePlayer;
            }
            Debug.Log(player.name + " has been added to the round.");
        }
    }

    public void RemovePlayer(EntityInfo entityInfo)
    {
        GameObject player = entityInfo.gameObject;
        if (_activePlayers.Contains(player))
        {
            player.GetComponent<PlayerMovement>().ProhibitMovement(); // Disable player movement
            _activePlayers.Remove(player);
            _inactivePlayers.Add(player);
            Debug.Log(player.name + " has been removed from the round.");
        }
    }

    public void StartRound()
    {
        
        if (_activePlayers.Count > 1)
        {
            _isRoundActive = true;
            _roundTimer = _roundTime;
            Debug.Log("Round started. Time remaining: " + _roundTimer + " seconds.");
        }
        else
        {
            Debug.Log("Not enough players to start the round.");
        }
    }

    public void EndRound()
    {
        _isRoundActive = false;
        Debug.Log("Round ended.");
        
    }
}
