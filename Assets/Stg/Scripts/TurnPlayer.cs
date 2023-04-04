using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnPlayer : MonoBehaviour
{
    private void Start()
    {
        
        GetComponent<Button>().onClick.AddListener(()=>PlayCardTurn());
    }

    private void PlayCardTurn()
    {
        if (BalootGameManager._instance.baloot.turn != RoomManager._instance.indexOfPlayer)
        {
            return;
        }
        BalootGameManager._instance.PlayCard() ;
    }
}
