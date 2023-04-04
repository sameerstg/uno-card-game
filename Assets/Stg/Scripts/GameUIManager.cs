using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager _instance;
    public GameObject slotParrent;
    public Slot[] slots;
    public GameObject cardPrefab;
    
    public string nameOfPlayer;
    
    private void Awake()
    {
        _instance = this;
        
       

    }
    public void RefereshCards()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            foreach (var item in slots[i].cards)
            {
                Destroy(item);
            }
            slots[i].cards.Clear();
            for (int k = 0; k < PlayerManager._instance.players[i].balootPlayerClass.cards.Count; k++)
            {
                Debug.Log(PlayerManager._instance.players[i].balootPlayerClass.cards[k].house);
                Debug.Log(PlayerManager._instance.players[i].balootPlayerClass.cards[k].cardName);
                

                Texture2D texture = Instantiate
                    (Resources.Load<Texture2D>(($"Cards\\{PlayerManager._instance.players[i].balootPlayerClass.cards[k].house}\\"
                    + $"{PlayerManager._instance.players[i].balootPlayerClass.cards[k].cardName}")));
                Debug.Log(texture);
                GameObject card = Instantiate(cardPrefab, slots[i].cardParent.transform);
                slots[i].cards.Add(card);
                               var cardClass = card.AddComponent<BalootCard>();
                cardClass.cardClass = new(PlayerManager._instance.players[i].balootPlayerClass.cards[k].house, PlayerManager._instance.players[i].balootPlayerClass.cards[k].cardName);


                cardClass.GetComponent<RawImage>().texture = texture;
                
                          }
        }


        }
}
