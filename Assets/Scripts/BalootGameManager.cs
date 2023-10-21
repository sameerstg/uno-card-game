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
        List<PlayerClass> list = RoomManager._instance.GetPlayersFromRoomProp();
        List<PlayerClass> scrambleList = new();
        while (list.Count>0)
        {
            var randPlayer = list[Random.Range(0, list.Count)];
            scrambleList.Add(randPlayer);
            list.Remove(randPlayer);
        }
               //PhotonNetwork.CurrentRoom.CustomProperties["playerList"] = JsonConvert.SerializeObject( scrambleList);
        RoomManager._instance.SetInRoomProp("playerList", scrambleList);
        Debug.Log(scrambleList);
        Debug.LogError(list.Count);
        list = RoomManager._instance.GetPlayersFromRoomProp();
        cardManager = new();

        this.cardManager.StartGame(list);
        Debug.LogError(this.cardManager.remainingDeck.Count);
        Debug.LogError("sync se pehle :  remaining: " + this.cardManager.remainingDeck.Count + " played: " + this.cardManager.playedCards.Count + " p1: " + this.cardManager.playerClasses[0].cards.Count + " p2: " + this.cardManager.playerClasses[1].cards.Count);

        RoomManager._instance.SetInRoomProp("cardManager", cardManager);
        RoomManager._instance.SetCardManagerInRoomProp();

        photonView.RPC(nameof(SyncNewGame), RpcTarget.AllBufferedViaServer);
        //GiveCardsToPlayer();
    }
    [PunRPC]
    public void SyncNewGame()
    {
        this.cardManager = null;
        cardManager = RoomManager._instance.GetCardManagerFromRoomProp();
        Debug.LogError("remaining: " + this.cardManager.remainingDeck.Count + " played: " + this.cardManager.playedCards.Count + " p1: " + this.cardManager.playerClasses[0].cards.Count + " p2: " + this.cardManager.playerClasses[1].cards.Count);
        //GameUIManager._instance.RefereshUi();
        GameUIManager._instance.DistributeCards();
    }

    [PunRPC]
    public void SyncCardManagerRPC()
    {
        this.cardManager = null;
        this.cardManager = RoomManager._instance.GetCardManagerFromRoomProp();
        GameUIManager._instance.RefereshUi();
        //Debug.LogError(cardManager.Length);
        //Debug.LogError(cardManager);
        //File.WriteAllText("logde.json", RoomManager._instance.GetCardManagerFromRoomProp());
        Debug.LogError("remaining: " + this.cardManager.remainingDeck.Count + " played: " + this.cardManager.playedCards.Count + " p1: " + this.cardManager.playerClasses[0].cards.Count + " p2: " + this.cardManager.playerClasses[1].cards.Count);

    }
    public void SyncCardManager()
    {
        Debug.LogError("deraahaha hai:    remaining: " + this.cardManager.remainingDeck.Count + " played: " + this.cardManager.playedCards.Count + " p1: " + this.cardManager.playerClasses[0].cards.Count + " p2: " + this.cardManager.playerClasses[1].cards.Count);
        RoomManager._instance.SetCardManagerInRoomProp();

        photonView.RPC(nameof(SyncCardManagerRPC), RpcTarget.AllBufferedViaServer);
    }
    void GiveCardToPlayer(PlayerClass player, CardClass card)
    {
        player.cards.Add(card);
    }
    



}
