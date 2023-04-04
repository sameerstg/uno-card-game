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
[System.Serializable]

public enum House
{
    Spade,Heart,Diamond,Club
}
[System.Serializable]
public enum CardName
{
    Ace,King,Queen,Jack,Ten,Nine,Eight,Seven
}
[System.Serializable]
public class Baloot
{
    public CardClass[] totalCards;
    public List<CardClass> cardsToBeCollected;
    public List<CardClass> playedCards;
    public House trump;
    public int turn;
    public Baloot()
    {
        totalCards = new CardClass[32];
        for (int i = 0; i < totalCards.Length; i++)
        {
            foreach (var house in Enum.GetNames(typeof(House)))
            {
                foreach (var cardName in Enum.GetNames(typeof(CardName)))
                {
                    totalCards[i] = new(Enum.Parse<House>(house), Enum.Parse<CardName>(cardName));
                    i++;
                    continue;
                }
            }
        }
        cardsToBeCollected = new();
        cardsToBeCollected = totalCards.ToList();
        playedCards = new();
    }

}
