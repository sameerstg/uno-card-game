using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Newtonsoft.Json;

public class BalootGameManager : MonoBehaviour
{
    public static BalootGameManager _instance;
    
    public CardManager cardManager;
    public PhotonView photonView;
    private void Awake()
    {
        _instance = this;
        photonView = GetComponent<PhotonView>();
      

    }
    public void NewGame()
    {
        if (PlayerManager._instance.CanPlayerJoinGame())
        {
            return;
        }
        cardManager = new();
        List<PlayerClass> list = new List<PlayerClass>(); 
        for (int i = 0; i < PlayerManager._instance.players.Length; i++)
        {
            list.Add(PlayerManager._instance.players[i].balootPlayerClass);
        }
        cardManager.StartGame(list);
        GameUIManager._instance.slots[cardManager.turn].turn.SetActive(true);
        GameUIManager._instance.RefereshCards();

        //GiveCardsToPlayer();
    }
    public void GiveCardsToPlayer()
    {
        Debug.LogError("given cards");
        for (int j = 0; j < PlayerManager._instance.players.Length; j++)
        {

            for (int i = 0; i < 7; i++)
            {
                
                Debug.Log(cardManager.remainingDeck.Count);
                CardClass card = cardManager.remainingDeck[
                    Random.Range(0, cardManager.remainingDeck.Count)
                    ];
                GiveCardToPlayer(PlayerManager._instance.players[j].balootPlayerClass, card);
               cardManager.remainingDeck.Remove(card);
                if (cardManager.remainingDeck.Count == 38)
                {
                    GameUIManager._instance.slots[cardManager.turn].turn.SetActive(true);

                    GameUIManager._instance.RefereshCards();
                    return;
                }
                
            }


        }
        
    }

    [PunRPC]
    public void PlayCardPun(/*string turn*/)
    {

        /*turn.player.cards.Remove(turn.card);
                baloot.playedCards.Add(turn.card);

        ChangeTurn();
        GameUIManager._instance.RefereshCards();*/

        //cardManager.playedCards.Add(PlayerManager._instance.players[cardManager.turn].balootPlayerClass.cards[0]);
        //PlayerManager._instance.players[cardManager.turn].balootPlayerClass.cards.
        //    Remove(PlayerManager._instance.players[cardManager.turn].balootPlayerClass.cards[0]);

        ChangeTurn();
        GameUIManager._instance.RefereshCards();
    }
    public void PlayCard(/*Turn turn*/)
    {
        photonView.RPC(nameof(
        PlayCardPun),RpcTarget.AllBufferedViaServer)/*JsonConvert.DeserializeObject<Turn>(turn)*/;

    }
    void GiveCardToPlayer(PlayerClass player, CardClass card)
    {
        player.cards.Add(card);
    }
    void ChangeTurn()
    {
        GameUIManager._instance.slots[cardManager.turn].turn.gameObject.SetActive(false);
        cardManager.turn++;
        if (cardManager.turn >= PlayerManager._instance.players.Length)
        {
            cardManager.turn = 0;
        }
        GameUIManager._instance.slots[cardManager.turn].turn.gameObject.SetActive(true);

    }

}
