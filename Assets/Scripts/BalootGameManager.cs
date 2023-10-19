using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Newtonsoft.Json;
using System.IO;

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
                  cardManager = new();
            List<PlayerClass> list = new List<PlayerClass>();
            for (int i = 0; i < PlayerManager._instance.players.Length; i++)
            {
                list.Add(PlayerManager._instance.players[i].balootPlayerClass);
            Debug.LogError(this.cardManager.remainingDeck.Count);
        }
        this.cardManager.StartGame(list);
            Debug.LogError(this.cardManager.remainingDeck.Count);
        Debug.LogError("sync se pehle :  remaining: " + this.cardManager.remainingDeck.Count + " played: " + this.cardManager.playedCards.Count + " p1: " + this.cardManager.playerClasses[0].cards.Count + " p2: " + this.cardManager.playerClasses[1].cards.Count);

        photonView.RPC(nameof(SyncNewGame), RpcTarget.All, new object[] { JsonConvert.SerializeObject(cardManager) });
      

        //GiveCardsToPlayer();
    }
    [PunRPC]
    public void SyncNewGame(string cardManagerParam)
    {
        this.cardManager = null;
        cardManager = JsonUtility.FromJson<CardManager>(cardManagerParam);
        for(int i = 0;i< cardManager.playerClasses.Count;i++)
        {
            if (cardManager.playerClasses[i].photonId == PhotonNetwork.LocalPlayer.UserId)
            {
                RoomManager._instance.localPlayerTurn = cardManager.playerClasses[i].turnNumber;
                RoomManager._instance.indexInGlobalPlayerList = i;
            }
        }
        Debug.LogError("remaining: "+this.cardManager.remainingDeck.Count + " played: " + this.cardManager.playedCards.Count +" p1: "+ this.cardManager.playerClasses[0].cards.Count + " p2: " + this.cardManager.playerClasses[1].cards.Count);

        //GameUIManager._instance.RefereshUi();
        GameUIManager._instance.DistributeCards();
    }
   
    [PunRPC]
    public void SyncCardManagerRPC(string cardManager)
    {
        this.cardManager = null;
        this.cardManager = JsonUtility.FromJson<CardManager>(cardManager);
        GameUIManager._instance.RefereshUi();
        Debug.LogError(cardManager.Length);
        //Debug.LogError(cardManager);
        File.WriteAllText("logde.json",cardManager);
        Debug.LogError("remaining: "+this.cardManager.remainingDeck.Count + " played: " + this.cardManager.playedCards.Count +" p1: "+ this.cardManager.playerClasses[0].cards.Count + " p2: " + this.cardManager.playerClasses[1].cards.Count);

    }
    public void SyncCardManager()
    {
        Debug.LogError("deraahaha hai:    remaining: "+this.cardManager.remainingDeck.Count + " played: " + this.cardManager.playedCards.Count +" p1: "+ this.cardManager.playerClasses[0].cards.Count + " p2: " + this.cardManager.playerClasses[1].cards.Count);

        photonView.RPC(nameof(SyncCardManagerRPC), RpcTarget.AllBufferedViaServer, JsonConvert.SerializeObject(cardManager));
    }
    void GiveCardToPlayer(PlayerClass player, CardClass card)
    {
        player.cards.Add(card);
    }
   
   

}
