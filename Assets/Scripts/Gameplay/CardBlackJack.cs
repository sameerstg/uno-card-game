using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardBlackJack : MonoBehaviour
{
    bool selected = false;

    private void Start()
    {

        GetComponent<Button>().onClick.AddListener(() => SelectCard());
    }

    private void SelectCard()
    {
        if (BalootGameManager._instance.cardManager.turn != RoomManager._instance.indexOfPlayer)
        {
            return;
        }
        if(!selected)
        {
            transform.localScale = Vector3.one * 1.2f;
        }
        else
        {
            transform.localScale = Vector3.one;
        }
        //BalootGameManager._instance.cardManager.selectedCard = ;
    }
}
