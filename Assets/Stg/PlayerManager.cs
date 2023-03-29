using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager _instance;
    public GameObject player;
    public BalootPlayer[] players ;
    private void Awake()
    {
        _instance = this;
        players = new BalootPlayer[4];
       /* for (int i = 0; i < 4; i++)
        {
            var balootPlayer = Instantiate(player).GetComponent<BalootPlayer>();
            players[i] = balootPlayer;
        }*/
    }
    public void AssignPlayer(BalootPlayer player)
    {

        if (CanPlayerJoinGame())
        {
        Debug.LogError("Assigning player");
            for (int i = 0; i < 4; i++)
            {
                if (players[i] == null)
                {
                    players[i] = player;
                    Debug.LogError("Player Assigned");
                    return;
                }
            }
        }
    }
    public bool CanPlayerJoinGame()
    {
        for (int i = 0; i < 4; i++)
        {
            if (players[i] == null)
            {
                return true;
            }
        }
        return false;
    }
}
