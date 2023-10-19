using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BalootCard : MonoBehaviour
{
    public CardClass cardClass;
    public bool selected = false;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => SelectCard());
    }

    private void SelectCard()
    {
        if (BalootGameManager._instance.cardManager.turn != RoomManager._instance.localPlayerTurn)
        {
            return;
        }

        //if (BalootGameManager._instance.cardManager.playerClasses.Find(x=>x.turnNumber == RoomManager._instance.indexOfPlayer).cards.Exists(x=>x.cardName == cardClass.cardName && x.house == cardClass.house))
        //    return;
        //if (BalootGameManager._instance.cardManager.turn != transform.parent.parent.GetSiblingIndex())
        //    return;

        if (!BalootGameManager._instance.cardManager.playerClasses[RoomManager._instance.indexInGlobalPlayerList].cards.Exists(x => x.cardName == cardClass.cardName && x.house == cardClass.house))
            return;
        //Debug.LogError(BalootGameManager._instance.cardManager.CanPlay(cardClass));
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            transform.parent.GetChild(i).GetComponent<BalootCard>().selected = false;
            transform.parent.GetChild(i).localScale = Vector3.one;
        }
        if (!selected)
        {
            selected = true;
            transform.localScale = Vector3.one * 1.2f;
            BalootGameManager._instance.cardManager.selectedCard = cardClass;
        }
        else
        {
            selected = false;
            BalootGameManager._instance.cardManager.selectedCard = null;
            transform.localScale = Vector3.one;
        }
    }
}
[System.Serializable]
public class CardClass
{
    public House house;
    public CardName cardName;
    public CardClass(House house, CardName cardName)
    {
        this.house = house;
        this.cardName = cardName;
    }
}
[Serializable]

public enum House
{
    Spade, Heart, Diamond, Club
}
[Serializable]
public enum CardName
{
    Ace, King, Queen, Jack, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten
}
public delegate void OnPlay();
