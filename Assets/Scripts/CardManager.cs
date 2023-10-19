using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CardManager
{
    public List<CardClass> remainingDeck;
    public List<PlayerClass> playerClasses = new();
    //internal List<CardClass> totalCards;
    public List<CardClass> playedCards;
    internal OnPlay onPlayCard;
    public int turn = 0;
    internal CardClass selectedCard;

    bool isTurnReversed = false;
    bool skipTurn = false;
    bool isForcedDrawCards = false;
    bool keepTurn = false;
    int takeCards = 0;

    public CardManager()
    {
        //totalCards = new();
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        remainingDeck = new();
        foreach (var house in Enum.GetNames(typeof(House)))
        {
            foreach (var cardName in Enum.GetNames(typeof(CardName)))
            {
                remainingDeck.Add(new(Enum.Parse<House>(house), Enum.Parse<CardName>(cardName)));
            }
        }

        Debug.LogError(remainingDeck.Count);
        //remainingDeck = totalCards.ToList();
    }
    public List<List<CardClass>> GetCardsForPlayers(int playerCount)
    {
        List<List<CardClass>> playersCards = new() { };
        for (int i = 0; i < playerCount; i++)
        {
            playersCards.Add(new());
        }
        for (int j = 0; j < playerCount; j++)
        {
            for (int i = 0; i < 7; i++)
            {
                if (remainingDeck.Count <= 0)
                {
                    return null;
                }
                var randomCardFromRemaining = UnityEngine.Random.Range(0, remainingDeck.Count);
                playersCards[j].Add(remainingDeck[randomCardFromRemaining]);
                remainingDeck.RemoveAt(randomCardFromRemaining);
                Debug.LogError(remainingDeck.Count);
            }
        }
        return playersCards;
    }
    public bool StartGame(List<PlayerClass> players)
    {
        Debug.LogError(remainingDeck.Count+"2");

        playedCards = new();
        if (players == null || players.Count == 0)
        {
            return false;
        }
        List<int> numbers = new();
        for (int i = 0; i < players.Count; i++)
        {
            numbers.Add(i);
        }



        var playersCards = GetCardsForPlayers(players.Count);
        for (int i = 0; i < playersCards.Count; i++)
        {
            foreach (var item in playersCards[i])
            {
                players[i].cards.Add(item);
            }
            int s = numbers[UnityEngine.Random.Range(0, numbers.Count)];
            players[i].turnNumber = s;
            numbers.Remove(s);
        }
        if (remainingDeck.Count <= 0)
        {
            return false;
        }
        var luckyCard = UnityEngine.Random.Range(0, remainingDeck.Count);
        playedCards.Add(remainingDeck[luckyCard]);
        remainingDeck.RemoveAt(luckyCard);
        playerClasses = players;
        Debug.LogError(remainingDeck.Count+"ss");

        return true;
    }
    public bool CanPlay(CardClass card)
    {
        return card.cardName == CardName.Ace || card.house == playedCards[^1].house || card.cardName == playedCards[^1].cardName;
    }
  
    public void TakeCard(bool cardPicked = false)
    {
        if (remainingDeck.Count == 0)
        {
            for (int i = playedCards.Count - 2; i > 0 ; i--)
            {
                remainingDeck.Add(playedCards[i]);
                playedCards.RemoveAt(i);
            }
        }
        var randCard = UnityEngine.Random.Range(0, remainingDeck.Count);
        playerClasses[RoomManager._instance.indexInGlobalPlayerList].cards.Add(remainingDeck[randCard]);
        remainingDeck.RemoveAt(randCard);
        if(cardPicked)
        {
            playerClasses[RoomManager._instance.indexInGlobalPlayerList].cardTaken = true;
            //playerClasses[RoomManager._instance.indexInGlobalPlayerList].cardTaken = true;
        }
        BalootGameManager._instance.SyncCardManager();
    }
    public bool PlayCard()
    {
        //int indexOfPlayer = playerClasses.IndexOf( playerClasses.Find(x => x.turnNumber == turn));
        //Debug.LogError(indexOfPlayer);
        if (selectedCard != null)
        {
            var canPlay = CanPlay(selectedCard);
            //Debug.LogError("Can Play Card: " + canPlay);
            if (canPlay)
            {
                var card = playerClasses[RoomManager._instance.indexInGlobalPlayerList].cards.Find(x => selectedCard.house == x.house && selectedCard.cardName == x.cardName);
                bool s = playerClasses[RoomManager._instance.indexInGlobalPlayerList].cards.Remove(card);
                //Debug.LogError("Card removed: " + s);
                playedCards.Add(card);
                CheckForWildCards(selectedCard);
                if (!keepTurn)
                {
                    ChangeTurn();
                    if (selectedCard.cardName == CardName.Two)
                    {
                        if (!CheckIfPlayerHasCardName(CardName.Two, playerClasses[RoomManager._instance.indexInGlobalPlayerList]))
                        {
                            for (int i = 0; i < takeCards; i++)
                            {
                                TakeCard();
                            }
                            takeCards = 0;
                        }
                    }
                    else if (selectedCard.cardName == CardName.Jack && (selectedCard.house == House.Spade || selectedCard.house == House.Club))
                    {
                        if (!CheckIfPlayerHasCardName(CardName.Jack, playerClasses[RoomManager._instance.indexInGlobalPlayerList]))
                        {
                            for (int i = 0; i < takeCards; i++)
                            {
                                TakeCard();
                            }
                            takeCards = 0;
                        }
                    }
                }
                else
                {
                    keepTurn = false;
                }
                BalootGameManager._instance.SyncCardManager();
                return true;
            }
            else { return false; }
        }
        else { return false; }
    }
    public bool CanPlay(PlayerClass playerClass)
    {

        foreach (var item in playerClass.cards)
        {
            if (CanPlay(item))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckIfPlayerHasCardName(CardName cardName, PlayerClass playerClass)
    {
        foreach (var item in playerClass.cards)
        {
            if (item.cardName == cardName)
            {
                return true;
            }
        }
        return false;
    }

   
   internal void ChangeTurn()
    {
        if (!isTurnReversed)
        {
            turn++;
            if (turn >= PlayerManager._instance.players.Length)
            {
                turn = 0;
            }
        }
        else
        {
            turn--;
            if (turn < 0)
            {
                turn = PlayerManager._instance.players.Length - 1;
            }
        }
        if (skipTurn)
        {
            skipTurn = false;
            ChangeTurn();
        }
    }

    void CheckForWildCards(CardClass playedCard)
    {
        if (playedCard.cardName == CardName.King)
        {
            isTurnReversed = !isTurnReversed;
        }
        else if (playedCard.cardName == CardName.Eight)
        {
            skipTurn = true;
        }
        else if (playedCard.cardName == CardName.Queen)
        {
            keepTurn = true;
        }
        else if (playedCard.cardName == CardName.Two)
        {
            takeCards += 2;
        }
        else if (playedCard.cardName == CardName.Jack && (playedCard.house == House.Spade || playedCard.house == House.Club))
        {
            takeCards += 5;
        }
        else if (playedCard.cardName == CardName.Jack && (playedCard.house == House.Heart || playedCard.house == House.Diamond))
        {
            takeCards = 0;
        }
    }
    public bool IsGameEnded()
    {
        return playerClasses.Exists(x => x.cards.Count == 0);
    }

}
