using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager _instance;
    public GameObject slotParrent;
    public Slot[] slots;
    public Slot playedCardSlot;
    public GameObject cardPrefab;
    public GameObject cardBack;
    public Transform deck;

    public string nameOfPlayer;

    private void Awake()
    {
        _instance = this;
    }
    public void RefereshUi()
    {
        for (int i = 0; i < BalootGameManager._instance.cardManager.playerClasses.Count; i++)
        {
            foreach (var item in slots[i].cards)
            {
                Destroy(item);
            }
            slots[i].cards.Clear();
            slots[i].nameTitle.text = BalootGameManager._instance.cardManager.playerClasses[i].playerName;
            if (BalootGameManager._instance.cardManager.turn == BalootGameManager._instance.cardManager.playerClasses[i].turnNumber)
            {
                slots[i].nameTitle.text += $" Turn";
            }
            for (int k = 0; k < BalootGameManager._instance.cardManager.playerClasses[i].cards.Count; k++)
            {
                //Debug.Log(PlayerManager._instance.players[i].balootPlayerClass.cards[k].house);
                //Debug.Log(PlayerManager._instance.players[i].balootPlayerClass.cards[k].cardName);


                //Texture2D texture = Instantiate
                //    (Resources.Load<Texture2D>(($"Cards\\{House.Spade}\\"
                //    + $"{CardName.Ace}")));
                Texture2D texture = Instantiate
                    (Resources.Load<Texture2D>(($"Cards\\" + BalootGameManager._instance.cardManager.playerClasses[i].cards[k].house + "\\"
                    + $"{BalootGameManager._instance.cardManager.playerClasses[i].cards[k].cardName}")));
                //Debug.Log(texture);
                GameObject card = Instantiate(cardPrefab, slots[i].cardParent.transform);
                slots[i].cards.Add(card);
                var cardClass = card.AddComponent<BalootCard>();
                cardClass.cardClass = new(BalootGameManager._instance.cardManager.playerClasses[i].cards[k].house, BalootGameManager._instance.cardManager.playerClasses[i].cards[k].cardName);
                cardClass.GetComponent<RawImage>().texture = texture;
                cardClass.cardClass.player = i;

            }
        }

        RefreshPlayedCards();
        ShowTurn();
    }

    public void RefreshPlayedCards()
    {
        foreach (var item in playedCardSlot.cards)
        {
            Destroy(item);
        }
        playedCardSlot.cards.Clear();
        if (BalootGameManager._instance.cardManager.playedCards.Count == 0)
        {
            return;
        }
        Debug.Log(BalootGameManager._instance.cardManager.playedCards[^1].house);
        Debug.Log(BalootGameManager._instance.cardManager.playedCards[^1].cardName);
        Texture2D texture = Instantiate
                    (Resources.Load<Texture2D>(($"Cards\\" + BalootGameManager._instance.cardManager.playedCards[^1].house + "\\"
                    + $"{BalootGameManager._instance.cardManager.playedCards[^1].cardName}")));
        Debug.Log(texture);
        GameObject card = Instantiate(cardPrefab, playedCardSlot.cardParent.transform);
        playedCardSlot.cards.Add(card);
        var cardClass = card.AddComponent<BalootCard>();
        cardClass.cardClass = new(BalootGameManager._instance.cardManager.playedCards[^1].house, BalootGameManager._instance.cardManager.playedCards[^1].cardName);
        cardClass.GetComponent<RawImage>().texture = texture;
        cardClass.GetComponent<RawImage>().raycastTarget = false;
        card.GetComponent<RectTransform>().DOSizeDelta(new Vector2(100, 140), 0f);
    }
    internal void ShowTurn()
    {
        foreach (var item in slots)
        {
            item.turn.gameObject.SetActive(false);
            item.takeCard.gameObject.SetActive(false);
        }

        if (BalootGameManager._instance.cardManager.turn != RoomManager._instance.localPlayerTurn)
        {
            return;
        }
        slots[BalootGameManager._instance.cardManager.turn].turn.gameObject.SetActive(true);
        //slots[BalootGameManager._instance.cardManager.turn].takeCard.gameObject.SetActive(true);
    }

    public void DistributeCards()
    {
        for (int i = 0; i < BalootGameManager._instance.cardManager.playerClasses.Count; i++)
        {
            foreach (var item in slots[i].cards)
            {
                Destroy(item);
            }
            slots[i].cards.Clear();
            slots[i].nameTitle.text = BalootGameManager._instance.cardManager.playerClasses[i].playerName;
            if (BalootGameManager._instance.cardManager.turn == BalootGameManager._instance.cardManager.playerClasses[i].turnNumber)
            {
                slots[i].nameTitle.text += $" Turn";
            }
            int k = 0;
            StartCoroutine(CardAnimation(i, k));
            //for (int k = 0; k < BalootGameManager._instance.cardManager.playerClasses[i].cards.Count; k++)
            //{
            //    //Debug.Log(PlayerManager._instance.players[i].balootPlayerClass.cards[k].house);
            //    //Debug.Log(PlayerManager._instance.players[i].balootPlayerClass.cards[k].cardName);


            //    //Texture2D texture = Instantiate
            //    //    (Resources.Load<Texture2D>(($"Cards\\{House.Spade}\\"
            //    //    + $"{CardName.Ace}")));
            //    Texture2D texture = Instantiate
            //        (Resources.Load<Texture2D>(($"Cards\\" + BalootGameManager._instance.cardManager.playerClasses[i].cards[k].house + "\\"
            //        + $"{BalootGameManager._instance.cardManager.playerClasses[i].cards[k].cardName}")));
            //    //Debug.Log(texture);
            //    GameObject card = Instantiate(cardPrefab, slots[i].cardParent.transform);
            //    var cardClass = card.AddComponent<BalootCard>();
            //    cardClass.cardClass = new(BalootGameManager._instance.cardManager.playerClasses[i].cards[k].house, BalootGameManager._instance.cardManager.playerClasses[i].cards[k].cardName);
            //    cardClass.GetComponent<RawImage>().texture = texture;
            //    cardClass.cardClass.player = i;
            //    card.transform.position = deck.transform.position;
            //    card.transform.DOMove(slots[i].cardParent.transform.position, 2f);
            //    slots[i].cards.Add(card);
            //}
        }
    }

    IEnumerator CardAnimation(int i, int k)
    {
        print("CardAnimation started " + i + " " + k + " " + Time.time);
        Texture2D texture = Instantiate
                    (Resources.Load<Texture2D>(($"Cards\\" + BalootGameManager._instance.cardManager.playerClasses[i].cards[k].house + "\\"
                    + $"{BalootGameManager._instance.cardManager.playerClasses[i].cards[k].cardName}")));
        Debug.Log(texture);
        GameObject card = Instantiate(cardPrefab, deck);
        GameObject carBack = Instantiate(cardBack, card.transform);
        var cardClass = card.AddComponent<BalootCard>();
        cardClass.cardClass = new(BalootGameManager._instance.cardManager.playerClasses[i].cards[k].house, BalootGameManager._instance.cardManager.playerClasses[i].cards[k].cardName);
        cardClass.GetComponent<RawImage>().texture = texture;
        cardClass.cardClass.player = i;
        card.transform.position = deck.position;
        card.GetComponent<RectTransform>().DOSizeDelta(new Vector2(100, 140), 0f);
        Vector3 targetPosition = slots[i].cardParent.transform.position;
        if (slots[i].cardParent.transform.childCount != 0)
        {
            targetPosition += new Vector3(k * 50, 0f, 0f);
        }
        card.transform.DORotate(new Vector3(0f, 90f, 0f), 0.5f, RotateMode.Fast).OnComplete(delegate
        {
            carBack.GetComponent<RawImage>().color = Color.clear;
            card.transform.DORotate(new Vector3(0f, 0f, 0f), 0.5f, RotateMode.Fast);
        });
        card.transform.DOMove(targetPosition, 1.5f).OnComplete(delegate
        {
            print("Card Moved: " + card.name);
            card.transform.SetParent(slots[i].cardParent.transform);
            slots[i].cards.Add(card);

            //k++;

            //if (k < BalootGameManager._instance.cardManager.playerClasses[i].cards.Count)
            //{
            //    CardAnimation(i, k);
            //}
            //else
            //{
            //    ShowTurn();
            //}
        });

        yield return new WaitForSeconds(0.5f);

        k++;

        if (k < BalootGameManager._instance.cardManager.playerClasses[i].cards.Count)
        {
            StartCoroutine(CardAnimation(i, k));
        }
        else
        {
            StartCoroutine(PlayedCardAnimation());
            yield return new WaitForSeconds(1.5f);
            ShowTurn();
        }
    }

    IEnumerator PlayedCardAnimation()
    {
        yield return new WaitForSeconds(1f);

        RefreshPlayedCards();
        Vector3 targetPosition = playedCardSlot.cards[^1].transform.position;
        GameObject carBack = Instantiate(cardBack, playedCardSlot.cards[^1].transform);
        playedCardSlot.cards[^1].transform.DORotate(new Vector3(0f, 90f, 0f), 0.5f, RotateMode.Fast).OnComplete(delegate
        {
            carBack.GetComponent<RawImage>().color = Color.clear;
            playedCardSlot.cards[^1].transform.DORotate(new Vector3(0f, 0f, 0f), 0.5f, RotateMode.Fast);
        });
        playedCardSlot.cards[^1].transform.position = deck.position;
        playedCardSlot.cards[^1].transform.DOMove(targetPosition, 1.5f);
    }
}
