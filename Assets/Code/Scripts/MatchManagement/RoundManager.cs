using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _players = new List<GameObject>();

    //private void Start()
    //{
    //    foreach (GameObject player in _players)
    //    {
    //        EntityInfo entityInfo = player.GetComponent<EntityInfo>();
    //        if (entityInfo != null)
    //        {
    //            entityInfo.OnEntityDeath += RemovePlayer;
    //        }
    //    }
    //}

    public void AddPlayer(GameObject player)
    {
        if (!_players.Contains(player))
        {
            _players.Add(player);
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
        if (_players.Contains(player))
        {
            player.SetActive(false);
            _players.Remove(player);
            Debug.Log(player.name + " has been removed from the round.");
        }
    }
}
