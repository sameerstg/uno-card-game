using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Newtonsoft.Json;

public class BalootGameManager : MonoBehaviour
{
    public static BalootGameManager _instance;
    
    public Baloot baloot;
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
        baloot = new();
        GiveCardsToPlayer();
    }
    public void GiveCardsToPlayer()
    {
        Debug.LogError("given cards");
        for (int j = 0; j < PlayerManager._instance.players.Length; j++)
        {

            for (int i = 0; i < 8; i++)
            {
                
                Debug.Log(baloot.cardsToBeCollected.Count);
                CardClass card = baloot.cardsToBeCollected[
                    Random.Range(0, baloot.cardsToBeCollected.Count)
                    ];
                GiveCardToPlayer(PlayerManager._instance.players[j].balootPlayerClass, card);
               baloot.cardsToBeCollected.Remove(card);
                if (baloot.cardsToBeCollected.Count == 0)
                {
                    GameUIManager._instance.slots[baloot.turn].turn.gameObject.SetActive(true);

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

        baloot.playedCards.Add(PlayerManager._instance.players[baloot.turn].balootPlayerClass.cards[0]);
        PlayerManager._instance.players[baloot.turn].balootPlayerClass.cards.
            Remove(PlayerManager._instance.players[baloot.turn].balootPlayerClass.cards[0]);

        ChangeTurn();
        GameUIManager._instance.RefereshCards();
    }
    public void PlayCard(/*Turn turn*/)
    {
        photonView.RPC(nameof(
        PlayCardPun),RpcTarget.AllBufferedViaServer)/*JsonConvert.DeserializeObject<Turn>(turn)*/;

    }
    void GiveCardToPlayer(BalootPlayerClass player, CardClass card)
    {
        player.cards.Add(card);
    }
    void ChangeTurn()
    {
        GameUIManager._instance.slots[baloot.turn].turn.gameObject.SetActive(false);
        baloot.turn++;
        if (baloot.turn>3)
        {
            baloot.turn = 0;
        }
        GameUIManager._instance.slots[baloot.turn].turn.gameObject.SetActive(true);

    }

}
