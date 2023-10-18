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
            photonView.RPC(nameof(SyncNewGame), RpcTarget.All, new object[] { JsonConvert.SerializeObject(cardManager) });
        }


        //GiveCardsToPlayer();
    }
    [PunRPC]
    public void SyncNewGame(string cardManagerParam)
    {
        cardManager = JsonConvert.DeserializeObject<CardManager>(cardManagerParam);
        RoomManager._instance.localPlayerTurn = cardManager.playerClasses.Find(x => x.photonId == PhotonNetwork.LocalPlayer.UserId).turnNumber;
        RoomManager._instance.indexInGlobalPlayerList = cardManager.playerClasses.IndexOf(cardManager.playerClasses.Find(x => x.photonId == PhotonNetwork.LocalPlayer.UserId));
        //GameUIManager._instance.RefereshUi();
        GameUIManager._instance.DistributeCards();
    }
   
    [PunRPC]
    public void SyncCardManagerRPC(string cardManager)
    {
        this.cardManager = JsonConvert.DeserializeObject<CardManager>(cardManager);
        GameUIManager._instance.RefereshUi();
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
