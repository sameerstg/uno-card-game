using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class BalootGameManager : MonoBehaviour
{
    public static BalootGameManager _instance;
    
    public Baloot baloot;
    private void Awake()
    {
        _instance = this;
      

    }
    public void NewGame()
    {
        if (PlayerManager._instance.CanPlayerJoinGame())
        {
            return;
        }
        baloot = new();
        GiveCardsToPlayer();
    }
    public void GiveCardsToPlayer()
    {

        foreach (var item in PlayerManager._instance.players)
        {
            for (int i = 0; i < 8; i++)
            {
                CardClass card = baloot.cardsToBeCollected[
                    Random.Range(0, baloot.cardsToBeCollected.Count)
                    ];
                GiveCardToPlayer(item, card);
               baloot.cardsToBeCollected.Remove(card);
            }
        }
    }
    public void PlayCard(BalootPlayer player, CardClass card)
    {
        player.cards.Remove(card);
                baloot.playedCards.Add(card);
    }
    void GiveCardToPlayer(BalootPlayer player, CardClass card)
    {
        player.cards.Add(card);
    }

}
