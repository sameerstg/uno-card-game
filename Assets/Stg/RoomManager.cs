using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager _instance;
    public TextMeshProUGUI statusText;
    PhotonView photonViewComponent;
    GameManager gm;
    public string balanceOfMatch;
    public bool DidTimeout { private set; get; }
    static readonly RoomOptions s_RoomOptions = new RoomOptions
    {
        MaxPlayers = 2,
        EmptyRoomTtl = 5,
        PublishUserId = true,

    };

    void Awake()
    {
        _instance = this;
        Assert.AreEqual(1, FindObjectsOfType<RoomManager>().Length);
        photonViewComponent = GetComponent<PhotonView>();

    }
    //private void Start()
    //{
    //    JoinOrCreateRoom("1");

    //}
    public void ReleaseBalance(string name)
    {
       
    }
    public void CeaseMoney()
    {
        Debug.Log("money ceased");

    }

    #region All Room Settings
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Created Room");
        //InstantiateGame();

        photonViewComponent.RPC(nameof(InstantiateGame), RpcTarget.AllBufferedViaServer);


    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room");
        StartCoroutine(SynchroniseGame());

    }



    public void JoinOrCreateRoom(string preferredRoomName)
    {
        //Debug.LogError(preferredRoomName);
        StopAllCoroutines();
        const float timeoutSeconds = 15f;
        StartCoroutine(DoCheckTimeout(timeoutSeconds));
        StartCoroutine(DoJoinOrCreateRoom(preferredRoomName));
    }

    IEnumerator DoCheckTimeout(float timeout)
    {
        DidTimeout = false;
        while (!PhotonNetwork.InRoom && timeout >= 0)
        {
            yield return null;
            timeout -= Time.deltaTime;
        }

        if (timeout <= 0)
        {
            DidTimeout = true;
            StopAllCoroutines();
        }

    }



    static IEnumerator DoJoinOrCreateRoom(string preferredRoomName)
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connecting to server");
            PhotonNetwork.ConnectUsingSettings();
        }

        while (!PhotonNetwork.IsConnectedAndReady)
        {

            yield return null;
        }

        if (!PhotonNetwork.InLobby && PhotonNetwork.NetworkClientState != ClientState.JoiningLobby)
            Debug.Log($"Connecting to server ,state ={PhotonNetwork.NetworkClientState}");
        {
            PhotonNetwork.JoinLobby();

        }

        while (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer)
        {

            yield return null;
        }


        if (preferredRoomName != null)
        {
            Debug.Log("Joining or creating Room");

            bool isJoined = PhotonNetwork.JoinOrCreateRoom(preferredRoomName, s_RoomOptions, TypedLobby.Default);
        }
        else
        {
            Debug.Log("Joined Random Room");

            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Room Created on joining fail");

        PhotonNetwork.CreateRoom(null, s_RoomOptions, TypedLobby.Default);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)

    {
        Debug.Log("Room Created on joining fail");

        PhotonNetwork.CreateRoom(null, s_RoomOptions, TypedLobby.Default);
    }

    #endregion
    #region Synchronization Code
    [PunRPC]
    public void InstantiateGame()
    {
        Debug.Log("Room Objects Created");
        PhotonNetwork.InstantiateRoomObject("Manager Holder", new Vector3(), Quaternion.identity);
    }
    IEnumerator SynchroniseGame()
    {
        var roomName = PhotonNetwork.CurrentRoom.Name;

        Debug.Log(roomName.Substring(roomName.IndexOf(":") + 2));

        balanceOfMatch = roomName.Substring(roomName.IndexOf(":") + 2);
        while (GameManager._instance == null)
        {
            yield return null;
        }
        gm = GameManager._instance;

        while (GameManager._instance.gameState == GameState.Initiating)
        {
            Debug.Log("GameManager.gameState == GameState.Initiating");
            yield return null;
        }

        while (GameManager._instance.gameState != GameState.PostInstantiate)
        {
            Debug.Log("GameManager.gameState != GameState.PostInstantiate");

            yield return null;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            SetGameData();

        }
        else
        {
                   }

       /* if (AssignPlayer())
        {
            SetPlayerAndPlayerBoard();

            SetGamePlayerData();
            photonView.RPC(nameof(UpdateGameDataToAll), RpcTarget.All);
        }*/

        GameManager._instance.gameState = GameState.WaitingForOpponent;
       /* if (GameManager._instance.playerManagerChess.GetPlayerCount() == 2)
        {


            SetGameState(GameState.GameStarted);
            SetGameData();
            UpdateGameStateToAllPun();

        }*/


        while (GameManager._instance.gameState == GameState.WaitingForOpponent)
        {
            //Debug.Log("waiting for player");
            GameObject.FindGameObjectWithTag("Loading").GetComponent<TextMeshProUGUI>().text = "Waiting for player ....";
            yield return null;
        }


        Destroy(GameObject.FindGameObjectWithTag("Loading"));

        var roomName1 = PhotonNetwork.CurrentRoom.Name;

        //DatabaseManager._instance.AddBalance(roomName.Substring(roomName.IndexOf(":") + 2));
        while (GameManager._instance.gameState != GameState.InGame)
        {
            //Debug.Log("waiting to start game");
            yield return null;
        }
        CeaseMoney();


    }

    void SetGameState(GameState state)
    {
        photonView.RPC(nameof(SetGameStatePun), RpcTarget.All, new object[] { state });

    }
    [PunRPC]
    void SetGameStatePun(GameState state)
    {
        gm.gameState = state;

    }
    internal void SetGameData()
    {

           }
          //   internal void SaveGameMovesData()
    //{
    //    Debug.Log("Saving Game Moves Data");
    //    ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
    //    properties["history"] = JsonUtility.ToJson(bm.grid.history);
    //    GameManager._instance.turn = (colorSide)JsonUtility.FromJson(properties["turn"].ToString(), typeof(colorSide));
    //    PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    //    UpdateGameStateToAllPun();

    //}

  
    #endregion
}