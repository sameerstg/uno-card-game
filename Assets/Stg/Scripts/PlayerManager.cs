using Photon.Pun;
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
    public void AssignPlayer(BalootPlayerClass player)
    {

                if (CanPlayerJoinGame())
        {
            var obj = Instantiate(Resources.Load<GameObject>("Player"), Vector2.zero, Quaternion.identity);


          
            Debug.LogError("Assigning player");
            for (int i = 0; i < 4; i++)
            {
                if (players[i] == null)
                {

                    players[i] = obj.GetComponent<BalootPlayer>();
                    /*foreach (var item in FindObjectsOfType<BalootPlayer>())
                    {
                        if (item.balootPlayerClass== player)
                        {
                            players[i] = item;
                        }
                    }*/
                    players[i].balootPlayerClass = player;
                    //players[i] = player;
                    GameUIManager._instance.slots[i].nameTitle.text = $"{player.playerName}";
                    Debug.LogError("Player Assigned");
                    if (RoomManager._instance.balootPlayerClass.playerName == player.playerName)
                    {
                        GameUIManager._instance.slots[i].nameTitle.color = Color.red;
                        RoomManager._instance.indexOfPlayer = i;
                    }
                    if (i==3)
                    {
                        BalootGameManager._instance.GiveCardsToPlayer();
                        
                    }

                    return;
                }
            }
        }
        Debug.LogError("Space not found");
        
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
