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
        //if (PlayerManager._instance.CanPlayerJoinGame())
        //{
        //    return;
        //}
        if (PhotonNetwork.IsMasterClient)
        {
            cardManager = new();
            List<PlayerClass> list = new List<PlayerClass>();
            for (int i = 0; i < PlayerManager._instance.players.Length; i++)
            {
                list.Add(PlayerManager._instance.players[i].balootPlayerClass);
            }
            this.cardManager.StartGame(list);
            photonView.RPC(nameof(SyncNewGame), RpcTarget.All, new object[] { JsonConvert.SerializeObject( list ), JsonConvert.SerializeObject(cardManager) });
        }
        

        //GiveCardsToPlayer();
    }
    [PunRPC]
    public void SyncNewGame(string players ,string cardManagerParam )
    {
        cardManager = JsonConvert.DeserializeObject<CardManager>(cardManagerParam);
        RoomManager._instance.indexOfPlayer = cardManager.playerClasses.Find(x => x.photonId == PhotonNetwork.LocalPlayer.UserId).turnNumber;
        RoomManager._instance.RealIndex = cardManager.playerClasses.IndexOf(cardManager.playerClasses.Find(x => x.photonId == PhotonNetwork.LocalPlayer.UserId));
        GameUIManager._instance.RefereshCards();
        ShowTurn();
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
    public void PlayCardPun(string cardManager)
    {

       
        this.cardManager = JsonConvert.DeserializeObject<CardManager>(cardManager);
        ShowTurn();
        //GameUIManager._instance.slots[this.cardManager.turn].turn.SetActive(true);
        GameUIManager._instance.RefereshCards();

    }
    public void PlayCard()
    {
        photonView.RPC(nameof(PlayCardPun),RpcTarget.AllBufferedViaServer,JsonConvert.SerializeObject(cardManager));
    }
    void GiveCardToPlayer(PlayerClass player, CardClass card)
    {
        player.cards.Add(card);
    }
    void ShowTurn()
    {
        foreach (var item in GameUIManager._instance.slots)
        {
            item.turn.gameObject.SetActive(false);
        }

        if (cardManager.turn != RoomManager._instance.indexOfPlayer)
        {
            return;
        }
        GameUIManager._instance.slots[cardManager.turn].turn.gameObject.SetActive(true);

    }

}
