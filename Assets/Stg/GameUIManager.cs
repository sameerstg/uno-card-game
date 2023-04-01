using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager _instance;
    public GameObject slotParrent;
    public Slot[] slots;
    
    public string nameOfPlayer;
    
    private void Awake()
    {
        _instance = this;
        
       

    }
    public void RefereshCards()
    {
        for (int i = 0; i < PlayerManager._instance.players.Length; i++)
        {
        slots[i].cardsText.text = "||";
            for (int k = 0; k < PlayerManager._instance.players[i].balootPlayerClass.cards.Count; k++)
            {
                slots[i].cardsText.text += $" {PlayerManager._instance.players[i].balootPlayerClass.cards[k].house} |" +
                    $" {PlayerManager._instance.players[i].balootPlayerClass.cards[k].cardName} ||";
            }
        }

        
    }
}
