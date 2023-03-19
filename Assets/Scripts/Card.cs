using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public CardClass cardClass;
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
public enum House
{
    spade,heart,diamond,club
}
public enum CardName
{
    Ace,King,Queen,Jack,ten,nine,eight,seven
}
[System.Serializable]
public class Baloot
{
    public CardClass[] cards;
    public Baloot()
    {
        cards = new CardClass[32];
        for (int i = 0; i < cards.Length; i++)
        {
            foreach (var house in Enum.GetNames(typeof(House)))
            {
                foreach (var cardName in Enum.GetNames(typeof(CardName)))
                {
                    cards[i] = new(Enum.Parse<House>(house), Enum.Parse<CardName>(cardName));
                    i++;
                    continue;
                }
            }
        }
    }

}
