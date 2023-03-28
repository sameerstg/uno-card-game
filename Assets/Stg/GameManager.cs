using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    public GameState gameState;
    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
        GiveCardsToPlayer();
    }
    void GiveCardsToPlayer()
    {

        foreach (var item in PlayerManager._instance.players)
        {
            for (int i = 0; i < 4; i++)
            {
                CardClass card = BalootGameManager._instance.baloot.cardsToBeCollected[
                    Random.Range(0, BalootGameManager._instance.baloot.cardsToBeCollected.Count)
                    ];
                GiveCardToPlayer(item, card);
                BalootGameManager._instance.baloot.cardsToBeCollected.Remove(card);
            }
        }
    }
    public void PlayCard(CardClass card)
    {
        BalootGameManager._instance.baloot.playedCards.Add(card);
    }
    void GiveCardToPlayer(BalootPlayer player, CardClass card)
    {
        player.cards.Add(card);
    }
}
public enum GameState { Initiating, PostInstantiate, WaitingForOpponent, InGame }
