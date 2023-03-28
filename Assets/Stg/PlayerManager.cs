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
        for (int i = 0; i < 4; i++)
        {
            var balootPlayer = Instantiate(player).GetComponent<BalootPlayer>();
            players[i] = balootPlayer;
        }
    }
}
