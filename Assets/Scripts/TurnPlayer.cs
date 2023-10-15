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
        //Debug.LogError("s");
        if (BalootGameManager._instance.cardManager.turn != RoomManager._instance.localPlayerTurn)
        {
            return;
        }

        var played = BalootGameManager._instance.cardManager.PlayCard();
        Debug.LogError("Card Played Pressed: " + played);
    }
}
