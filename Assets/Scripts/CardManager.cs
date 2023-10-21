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
    public House chosenSuit = House.Spade;

    public bool isTurnReversed = false;
    public bool skipTurn = false;
    //bool isForcedDrawCards = false;
    //bool keepTurn = false;
    //int takeCards = 0;
    public int twosCount = 0;
    public int blackJacksCount = 0;
    public bool turnChanged = true;

    public CardManager()
    {
        //totalCards = new();
       
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
            for (int i = 0; i <7; i++)
            {
                if (remainingDeck.Count <= 0)
                {
                    return null;
                }
                var randomCardFromRemaining = UnityEngine.Random.Range(0, remainingDeck.Count);
                playersCards[j].Add(remainingDeck[randomCardFromRemaining]);
                remainingDeck.RemoveAt(randomCardFromRemaining);
                //Debug.LogError(remainingDeck.Count);
            }
        }
        return playersCards;
    }
    public bool StartGame(List<PlayerClass> players)
    {

        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }
        remainingDeck = new();
        foreach (var house in Enum.GetNames(typeof(House)))
        {
            foreach (var cardName in Enum.GetNames(typeof(CardName)))
            {
                remainingDeck.Add(new(Enum.Parse<House>(house), Enum.Parse<CardName>(cardName)));
            }
        }


        Debug.LogError(remainingDeck.Count + "2");

        playedCards = new();
        if (players == null || players.Count == 0)
        {
            return false;
        }
       


        var playersCards = GetCardsForPlayers(players.Count);
        for (int i = 0; i < playersCards.Count; i++)
        {
            foreach (var item in playersCards[i])
            {
                players[i].cards.Add(item);
            }
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
    public bool CanPlaySameTurn(CardClass card)
    {
        if (card.house == playedCards[^1].house)
        {
            if ((int)card.cardName > (int)playedCards[^1].cardName || card.cardName == CardName.Ace)
                return true;
            else
                return false;
        }
        else if (card.house != playedCards[^1].house)
        {
            if (card.cardName == playedCards[^1].cardName)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    public bool CanPlay(CardClass card)
    {
        if(turnChanged && playedCards[^1].cardName == CardName.Ace)
        {
            if (card.house == chosenSuit)
                return true;
            else
                return false;
        }
        else if (card.house == playedCards[^1].house)
        {
            return true;
        }
        else if (card.cardName == playedCards[^1].cardName)
        {
            return true;
        }
        else
            return false;
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
        BalootGameManager._instance.cardManager.GetPlayerByTurn().cards.Add(remainingDeck[randCard]);
        remainingDeck.RemoveAt(randCard);
        if(cardPicked)
        {
            BalootGameManager._instance.cardManager.GetPlayerByTurn().cardTaken = true;
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
            bool canPlay = false;
            if(turnChanged)
            {
                canPlay = CanPlay(selectedCard);
            }
            else
            {
                canPlay = CanPlaySameTurn(selectedCard);
            }
            //Debug.LogError("Can Play Card: " + canPlay);
            //if (canPlay)
            //{
            var card = BalootGameManager._instance.cardManager.GetPlayerByTurn().cards.Find(x => selectedCard.house == x.house && selectedCard.cardName == x.cardName);
            bool s = BalootGameManager._instance.cardManager.GetPlayerByTurn().cards.Remove(card);
            //Debug.LogError("Card removed: " + s);
            playedCards.Add(card);

            if (blackJacksCount != 0)
            {
                if (selectedCard.cardName != CardName.Jack)
                {
                    if(turnChanged)
                    {
                        for (int i = 0; i < (blackJacksCount * 5); i++)
                        {
                            TakeCard();
                        }
                        blackJacksCount = 0;
                    }
                }
                else if (selectedCard.house == House.Spade || selectedCard.house == House.Club)
                {
                    blackJacksCount++;
                }
                else if (selectedCard.house == House.Diamond || selectedCard.house == House.Heart)
                {
                    blackJacksCount = 0;
                }
            }
            else if (twosCount != 0)
            {
                if (selectedCard.cardName != CardName.Two)
                {
                    if(turnChanged)
                    {
                        for (int i = 0; i < (twosCount * 2); i++)
                        {
                            TakeCard();
                        }
                        twosCount = 0;
                    }
                }
                else
                {
                    twosCount++;
                }
            }

            if (!canPlay)
            {
                for (int i = 0; i < 2; i++)
                {
                    TakeCard();
                }
            }
            CheckForWildCards(selectedCard);

            turnChanged = false;
            GameUIManager._instance.chosenSuit.SetActive(false);

            if (selectedCard.cardName == CardName.Ace)
            {
                ChooseSuit();
            }
            else if (!canPlay)
            {
                if (playerClasses[BalootGameManager._instance.cardManager.turn].photonId == RoomManager._instance.photonId)
                {
                    GameUIManager._instance.slots[BalootGameManager._instance.cardManager.turn].endTurn.SetActive(false);
                }
                ChangeTurn();
            }
            else
            {
                if (playerClasses[BalootGameManager._instance.cardManager.turn].photonId == RoomManager._instance.photonId)
                {
                    GameUIManager._instance.slots[BalootGameManager._instance.cardManager.turn].endTurn.SetActive(true);
                }
            }
            //else
            //{
                //ChangeTurn();
                //if (selectedCard.cardName == CardName.Two)
                //{
                //    if (!CheckIfPlayerHasCardName(CardName.Two, playerClasses[RoomManager._instance.indexInGlobalPlayerList]))
                //    {
                //        for (int i = 0; i < (twosCount * 2); i++)
                //        {
                //            TakeCard();
                //        }
                //        twosCount = 0;
                //    }
                //}
                //else if (selectedCard.cardName == CardName.Jack && (selectedCard.house == House.Spade || selectedCard.house == House.Club))
                //{
                //    if (!CheckIfPlayerHasCardName(CardName.Jack, playerClasses[RoomManager._instance.indexInGlobalPlayerList]))
                //    {
                //        for (int i = 0; i < (blackJacksCount * 5); i++)
                //        {
                //            TakeCard();
                //        }
                //        blackJacksCount = 0;
                //    }
                //}
            //}

            BalootGameManager._instance.SyncCardManager();
            return true;
            //}
            //else { return false; }
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
            if (turn >= BalootGameManager._instance.cardManager.playerClasses.Count)
            {
                turn = 0;
            }
        }
        else
        {
            turn--;
            if (turn < 0)
            {
                turn = BalootGameManager._instance.cardManager.playerClasses.Count - 1;
            }
        }
        if (skipTurn)
        {
            skipTurn = false;
            ChangeTurn();
        }
        turnChanged = true;
        foreach(var item in BalootGameManager._instance.cardManager.playerClasses)
        {
            item.lastCardPressed = false;
        }
    }

    void CheckForWildCards(CardClass playedCard)
    {
        if (playedCard.cardName == CardName.Ten)
        {
            isTurnReversed = !isTurnReversed;
        }
        else if (playedCard.cardName == CardName.Eight)
        {
            skipTurn = true;
        }
        //else if (playedCard.cardName == CardName.Queen)
        //{
        //    keepTurn = true;
        //}
        else if (twosCount == 0 && playedCard.cardName == CardName.Two)
        {
            twosCount++;
        }
        else if (blackJacksCount == 0 && playedCard.cardName == CardName.Jack && (playedCard.house == House.Spade || playedCard.house == House.Club))
        {
            blackJacksCount++;
        }
        else if (playedCard.cardName == CardName.Jack && (playedCard.house == House.Heart || playedCard.house == House.Diamond))
        {
            blackJacksCount = 0;
        }
    }

    public void AddAccruedCardsOnChangeTurn()
    {
        if (blackJacksCount != 0)
        {
            if (turnChanged)
            {
                for (int i = 0; i < (blackJacksCount * 5); i++)
                {
                    TakeCard();
                }
                blackJacksCount = 0;
            }
        }
        else if (twosCount != 0)
        {
            if (turnChanged)
            {
                for (int i = 0; i < (twosCount * 2); i++)
                {
                    TakeCard();
                }
                twosCount = 0;
            }
        }
    }

    void ChooseSuit()
    {
        GameUIManager._instance.OpenChooseSuitMenu();
    }
    public bool IsGameEnded()
    {
        return playerClasses.Exists(x => x.cards.Count == 0);
    }
    public PlayerClass GetPlayerByTurn()
    {
        return playerClasses[turn];
    }

}
