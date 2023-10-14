using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager _instance;
    public GameObject slotParrent;
    public Slot[] slots;
    public Slot playedCardSlot;
    public GameObject cardPrefab;
    
    public string nameOfPlayer;
    
    private void Awake()
    {
        _instance = this;
    }
    public void RefereshCards()
    {
        for (int i = 0; i < BalootGameManager._instance.cardManager.playerClasses.Count; i++)
        {
            foreach (var item in slots[i].cards)
            {
                Destroy(item);
            }
            slots[i].cards.Clear();
            slots[i].nameTitle.text = BalootGameManager._instance.cardManager.playerClasses[i].playerName;
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
    }

    public void RefreshPlayedCards()
    {
        foreach (var item in playedCardSlot.cards)
        {
            Destroy(item);
        }
        playedCardSlot.cards.Clear();
        if(BalootGameManager._instance.cardManager.playedCards.Count == 0)
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
    }
}
