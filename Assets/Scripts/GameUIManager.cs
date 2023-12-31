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
    public GameObject chooseSuitMenu;
    public GameObject chosenSuit;

    public string nameOfPlayer;
    public bool devMode;
    private void Awake()
    {
        _instance = this;
    }
    public void RefereshUi()
    {

        Debug.LogError(BalootGameManager._instance.cardManager);
        Debug.LogError(BalootGameManager._instance.cardManager.turnChanged);
        Debug.LogError(BalootGameManager._instance.cardManager.playedCards[^1].cardName);

        if (BalootGameManager._instance.cardManager.playedCards[^1].cardName == CardName.Ace && BalootGameManager._instance.cardManager.turnChanged)
        {
            Sprite sprite = Instantiate(Resources.Load<Sprite>($"Cards\\" + BalootGameManager._instance.cardManager.chosenSuit.ToString() + "icon"));
            chosenSuit.GetComponent<Image>().sprite = sprite;
            chosenSuit.SetActive(true);
        }
        else
        {
            chosenSuit.SetActive(false);
        }
        foreach (var item in slots)
        {
            item.turn.SetActive(false);
            item.takeCard.SetActive(false);
        }
        for (int i = 0; i < BalootGameManager._instance.cardManager.playerClasses.Count; i++)
        {
            foreach (var item in slots[i].cards)
            {
                Destroy(item);
            }
            slots[i].cards.Clear();
            slots[i].nameTitle.text = BalootGameManager._instance.cardManager.playerClasses[i].playerName;

            if (BalootGameManager._instance.cardManager.playerClasses[i].photonId == RoomManager._instance.photonId)
            {
                slots[i].nameTitle.color = Color.green;
            }
            else
            {
                slots[i].nameTitle.color = Color.white;

            }
            if (BalootGameManager._instance.cardManager.IsGameEnded())
            {
                if (BalootGameManager._instance.cardManager.playerClasses[i].cards.Count == 0)
                {
                    for(int j = 0; j < BalootGameManager._instance.cardManager.playerClasses.Count; j++)
                    {
                        slots[i].turn.SetActive(false);
                        slots[i].takeCard.SetActive(false);
                        slots[i].endTurn.SetActive(false);
                        slots[i].lastCard.SetActive(false);
                    }
                    slots[i].nameTitle.text += " Winner";
                }
            }
            else
            {
                ShowTurn();
            }
            for (int k = 0; k < BalootGameManager._instance.cardManager.playerClasses[i].cards.Count; k++)
            {
                //UnrevealedCard
               
              
                //Debug.Log(texture);
                GameObject card = Instantiate(cardPrefab, slots[i].cardParent.transform);
                slots[i].cards.Add(card);
                var cardClass = card.AddComponent<BalootCard>();
                cardClass.cardClass = new(BalootGameManager._instance.cardManager.playerClasses[i].cards[k].house, BalootGameManager._instance.cardManager.playerClasses[i].cards[k].cardName);
               
                if (BalootGameManager._instance.cardManager.playerClasses[i].photonId != RoomManager._instance.photonId && !devMode)
                {
                    Texture2D texture = Instantiate
                       (Resources.Load<Texture2D>(($"Cards\\" + "UnrevealedCard")));
                    cardClass.GetComponent<RawImage>().texture = texture;
                }
                else
                {

                    Texture2D texture = Instantiate
      (Resources.Load<Texture2D>(($"Cards\\" + BalootGameManager._instance.cardManager.playerClasses[i].cards[k].house + "\\"
      + $"{BalootGameManager._instance.cardManager.playerClasses[i].cards[k].cardName}")));
                    cardClass.GetComponent<RawImage>().texture = texture;
                }


            }
        }
        RefreshPlayedCards();


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
        //Debug.Log(texture);
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
        //Debug.LogError(RoomManager._instance.indexInGlobalPlayerList);
        //Debug.LogError(RoomManager._instance.localPlayerTurn);
        //Debug.LogError(BalootGameManager._instance.cardManager.turn);
        //if (BalootGameManager._instance.cardManager.turn == RoomManager._instance.localPlayerTurn)
        if (BalootGameManager._instance.cardManager.GetPlayerByTurn().photonId == RoomManager._instance.photonId)
        {
            //Debug.LogError(RoomManager._instance.indexInGlobalPlayerList);
            //Debug.LogError(RoomManager._instance.localPlayerTurn);
            //slots[RoomManager._instance.indexInGlobalPlayerList].nameTitle.text += $" Turn";
            slots[BalootGameManager._instance.cardManager.turn].turn.SetActive(true);
            if (BalootGameManager._instance.cardManager.GetPlayerByTurn().cardTaken)
            {
                slots[BalootGameManager._instance.cardManager.turn].takeCard.SetActive(false);
            }
            else if(BalootGameManager._instance.cardManager.turnChanged)
            {
                slots[BalootGameManager._instance.cardManager.turn].takeCard.SetActive(true);
            }
            if(BalootGameManager._instance.cardManager.playedCards[^1].cardName == CardName.Ace && !BalootGameManager._instance.cardManager.turnChanged)
            {
                slots[BalootGameManager._instance.cardManager.turn].turn.SetActive(false);
                slots[BalootGameManager._instance.cardManager.turn].takeCard.SetActive(false);
            }


            foreach (PlayerClass item in BalootGameManager._instance.cardManager.playerClasses)
            {
                item.cardTaken = false;
            }
        }
        if(BalootGameManager._instance.cardManager.GetPlayerByTurn().cards.Count != 1)
        {
            slots[BalootGameManager._instance.cardManager.turn].lastCard.SetActive(false);
        }
        else if(!BalootGameManager._instance.cardManager.turnChanged && BalootGameManager._instance.cardManager.GetPlayerByTurn().photonId == RoomManager._instance.photonId)
        {
            slots[BalootGameManager._instance.cardManager.turn].lastCard.SetActive(true);
        }
        //if (BalootGameManager._instance.cardManager.turn == RoomManager._instance.localPlayerTurn)
        //{
        //    slots[BalootGameManager._instance.cardManager.turn].turn.gameObject.SetActive(true);

        //}
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
            //if (BalootGameManager._instance.cardManager.turn == BalootGameManager._instance.cardManager.playerClasses[i].turnNumber)
            //{
            //    slots[i].nameTitle.text += $" Turn";
            //}
            int k = 0;
            StartCoroutine(CardAnimation(i, k));
        }
    }

    IEnumerator CardAnimation(int i, int k)
    {
        print("CardAnimation started " + i + " " + k + " " + Time.time);



        //Debug.Log(texture);
        GameObject card = Instantiate(cardPrefab, deck);
        GameObject carBack = Instantiate(cardBack, card.transform);
        var cardClass = card.AddComponent<BalootCard>();
        cardClass.cardClass = new(BalootGameManager._instance.cardManager.playerClasses[i].cards[k].house, BalootGameManager._instance.cardManager.playerClasses[i].cards[k].cardName);


        if (BalootGameManager._instance.cardManager.playerClasses[i].photonId != RoomManager._instance.photonId && !devMode)
        {
            Texture2D texture = Instantiate
               (Resources.Load<Texture2D>(($"Cards\\" + "UnrevealedCard")));
            cardClass.GetComponent<RawImage>().texture = texture;
        }
        else
        {
            Texture2D texture = Instantiate
                     (Resources.Load<Texture2D>(($"Cards\\" + BalootGameManager._instance.cardManager.playerClasses[i].cards[k].house + "\\"
                     + $"{BalootGameManager._instance.cardManager.playerClasses[i].cards[k].cardName}")));
            cardClass.GetComponent<RawImage>().texture = texture;
        }
       
        card.transform.position = deck.position;
        card.GetComponent<RectTransform>().DOSizeDelta(new Vector2(100, 140), 0f);
        Vector3 targetPosition = slots[i].cardParent.transform.position;
        if (slots[i].cardParent.transform.childCount != 0)
        {
            targetPosition += new Vector3(k * 25, 0f, 0f);
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
            RefereshUi();
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

    public void OpenChooseSuitMenu()
    {
        if (BalootGameManager._instance.cardManager.GetPlayerByTurn().photonId == RoomManager._instance.photonId)
        {
            slots[BalootGameManager._instance.cardManager.turn].turn.SetActive(false);
            slots[BalootGameManager._instance.cardManager.turn].takeCard.SetActive(false);
            slots[BalootGameManager._instance.cardManager.turn].endTurn.SetActive(false);
            chooseSuitMenu.SetActive(true);
        }
    }

    public void ChooseSuit(int house)
    {
        chooseSuitMenu.SetActive(false);
        BalootGameManager._instance.cardManager.chosenSuit = (House)house;

        Sprite sprite = Instantiate(Resources.Load<Sprite>($"Cards\\" + BalootGameManager._instance.cardManager.chosenSuit.ToString() + "icon"));
        chosenSuit.GetComponent<Image>().sprite = sprite;
        chosenSuit.SetActive(true);
        slots[BalootGameManager._instance.cardManager.turn].endTurn.SetActive(true);
    }

    //public void EndTurn()
    //{
    //    BalootGameManager._instance.cardManager.ChangeTurn();
    //    BalootGameManager._instance.SyncCardManager();
    //}
}
