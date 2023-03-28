using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalootManager : MonoBehaviour
{
    public int deckSize;
    public GameObject[] deck = new GameObject[32];
    public GameObject[] shuffledDeck;
    public Transform[] playerCardSlots, teammateCardSlots, Bot1CardSlots, Bot2CardSlots, PurchaserCardSlots;
    public Transform PlayerCardSlotsPrefab, TeammateCardSlotsPrefab, Bot1CardSlotsPrefab, Bot2CardSlotsPrefab;
    public GameObject PlayerPrefab;
    public bool isSunGame, isHokmGame;

    GameObject cardPrefab;

    void Awake()
    {

        Card card;

    }


    void Start()
    {
        ShuffleCards();
        DrawCards();
    }

    public void ShuffleCards()
    {
        int i;
        int k;
        GameObject temp;
        for (i = 0; i < 32; i++)
        {
            k = Random.Range(0, 32);
            temp = deck[k];
            deck[k] = deck[i];
            deck[i] = temp;

        }

    }

    public void DrawCards()
    {

        int i = 0;
        foreach (Transform Slot in playerCardSlots)
        {
            Instantiate(deck[i], Slot).GetComponent<Card>().isDragable = true;
            i++;
            Slot.gameObject.SetActive(true);

        }

        foreach (Transform Tslot in teammateCardSlots)
        {

            Instantiate(deck[i], Tslot).GetComponent<Card>().FlipCard();
            i++;
            Tslot.gameObject.SetActive(true);

        }

        foreach (Transform B1Slot in Bot1CardSlots)
        {


            Instantiate(deck[i], B1Slot).GetComponent<Card>().FlipCard();
            i++;
            B1Slot.gameObject.SetActive(true);
        }

        foreach (Transform B2Slot in Bot2CardSlots)
        {

            Instantiate(deck[i], B2Slot).GetComponent<Card>().FlipCard();
            i++;
            B2Slot.gameObject.SetActive(true);
        }

	foreach (Transform PSlot in PurchaserCardSlots)
	{ 
		Instantiate(deck[i], PSlot).GetComponent<Card>();
            i++;
            PSlot.gameObject.SetActive(true);	
	} 
        i = 0;

    }

	void Update()
	{ 
	
		

	}  
}
