using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
        if (PhotonNetwork.IsMasterClient)
        {
            cardManager = new();
            List<PlayerClass> list = new List<PlayerClass>();
            for (int i = 0; i < PlayerManager._instance.players.Length; i++)
            {
                list.Add(PlayerManager._instance.players[i].balootPlayerClass);
            }
            this.cardManager.StartGame(list);
            photonView.RPC(nameof(SyncNewGame), RpcTarget.All, new object[] { JsonConvert.SerializeObject(cardManager) });
        }


        //GiveCardsToPlayer();
    }
    [PunRPC]
    public void SyncNewGame(string cardManagerParam)
    {
        cardManager = JsonConvert.DeserializeObject<CardManager>(cardManagerParam);
        for(int i = 0;i< cardManager.playerClasses.Count;i++)
        {
            if (cardManager.playerClasses[i].photonId == PhotonNetwork.LocalPlayer.UserId)
            {
                RoomManager._instance.localPlayerTurn = cardManager.playerClasses[i].turnNumber;
                RoomManager._instance.indexInGlobalPlayerList = i;
            }
        }
       
        //GameUIManager._instance.RefereshUi();
        GameUIManager._instance.DistributeCards();
    }
   
    [PunRPC]
    public void SyncCardManagerRPC(string cardManager)
    {

        this.cardManager = JsonConvert.DeserializeObject<CardManager>(cardManager);
        GameUIManager._instance.RefereshUi();
        Debug.LogError(cardManager.Length);
        //Debug.LogError(cardManager);
        File.WriteAllText("logde.json",cardManager);
        Debug.LogError(this.cardManager.remainingDeck.Count + this.cardManager.playedCards.Count + this.cardManager.playerClasses[0].cards.Count + this.cardManager.playerClasses[1].cards.Count);

    }
    public void SyncCardManager()
    {
        photonView.RPC(nameof(SyncCardManagerRPC), RpcTarget.AllBufferedViaServer, JsonConvert.SerializeObject(cardManager));
    }
    void GiveCardToPlayer(PlayerClass player, CardClass card)
    {
        player.cards.Add(card);
    }
   
   

}
