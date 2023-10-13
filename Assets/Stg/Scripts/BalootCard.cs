using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BalootCard : MonoBehaviour
{
    public CardClass cardClass;
}
[System.Serializable]
public class CardClass
{
    public House house;
    public CardName cardName;
    public int points;
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

[System.Serializable]
public class CardManager
{
    public List<PlayerClass> playerClasses = new(); 
    public List<CardClass> totalCards;
    public List<CardClass> remainingDeck;
    public List<CardClass> playedCards;
    public OnPlay onPlayCard;
    public int turn;
    public CardManager()
    {
        totalCards = new();
        foreach (var house in Enum.GetNames(typeof(House)))
        {
            foreach (var cardName in Enum.GetNames(typeof(CardName)))
            {
                totalCards.Add (new(Enum.Parse<House>(house), Enum.Parse<CardName>(cardName)));
            }
        }
        remainingDeck = totalCards.ToList();
       
    }
    public List<List<CardClass>> GetCardsForPlayers(int playerCount)
    {
        List<List<CardClass>> playersCards = new() {} ;
        for (int i = 0; i < playerCount; i++)
        {
            playersCards.Add(new());
        }
        for (int j = 0; j < playerCount; j++)
        {
            for (int i = 0; i < 7; i++)
            {
                if (remainingDeck.Count <=0)
                {
                    return null;
                }
                var randomCardFromRemaining = remainingDeck[UnityEngine.Random.Range(0, remainingDeck.Count)];
                playersCards[j].Add(randomCardFromRemaining);
                remainingDeck.Remove(randomCardFromRemaining);
            }
        }
        return playersCards;
    }
    public bool StartGame(List<PlayerClass> players)
    {
        if (players == null|| players.Count == 0)
        {
            return false;
        }
        List<int> numbers = new();
        for (int i = 0; i < players.Count; i++)
        {
            numbers.Add(i);
        }

        playedCards = new();

        
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
        if (remainingDeck.Count <=0)
        {
            return false;
        }
        var luckyCard = remainingDeck[UnityEngine.Random.Range(0, remainingDeck.Count)];
        playedCards.Add(luckyCard);
        remainingDeck.Remove(luckyCard);
        playerClasses = players;
        return true;   
    }
    public bool CanPlay(CardClass card)
    {
        return card.cardName == CardName.Ace|| card.house == playedCards[^1].house || card.cardName == playedCards[^1].cardName;
    }
    public bool PlayCard(CardClass card,PlayerClass player)
    {
        if (CanPlay(card) && player.cards.Contains(card) && turn == player.turnNumber)
        {
            playedCards.Add(card);
            player.cards.Remove(card);
            onPlayCard();
            return true;
        }
        else { return false; }
    }
    public bool PlayCard(CardClass card)
    {
        if (CanPlay(card) )
        {
            PlayerClass player = playerClasses.Find(x => x.cards.Contains(card));
            playedCards.Add(card);
            player.cards.Remove(card);
            onPlayCard();
            return true;
        }
        else { return false; }
    }
    public bool PlayCard()
    {
                   PlayerClass player = playerClasses.Find(x => x.turnNumber == turn);
        playedCards.Add(player.cards[0]);
            player.cards.Remove(player.cards[0]);
        GameUIManager._instance.RefereshCards();
        return true;
            }
}
