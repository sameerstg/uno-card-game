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
    public BalootPlayer balootPlayer;
    public bool DidTimeout { private set; get; }
    static readonly RoomOptions s_RoomOptions = new RoomOptions
    {
        MaxPlayers = 4,
        EmptyRoomTtl = 5,
        PublishUserId = true,

    };

    void Awake()
    {
        _instance = this;
        Assert.AreEqual(1, FindObjectsOfType<RoomManager>().Length);
        photonViewComponent = GetComponent<PhotonView>();

    }
    private void Start()
    {
        JoinOrCreateRoom("1");

    }
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
        PhotonNetwork.InstantiateRoomObject("GameManager", new Vector3(), Quaternion.identity);
    } [PunRPC]
    public void InstantiatePlayer()
    {
        Debug.Log("Player Created");
        PhotonNetwork.InstantiateRoomObject("Player", new Vector3(), Quaternion.identity);
    }
    IEnumerator SynchroniseGame()
    {


        while (GameManager._instance == null)
        {
            yield return null;
        }
        GameManager._instance.gameState = GameState.Initiating;
        while (GameManager._instance.gameState != GameState.Initiating)
        {
            Debug.Log("GameManager.gameState == GameState.Initiating");
            yield return null;
        }
        if (PlayerManager._instance.CanPlayerJoinGame())
        {
            photonViewComponent.RPC(nameof(InstantiatePlayer), RpcTarget.AllBufferedViaServer);
            
            GameManager._instance.gameState = GameState.PostInstantiate;
        }
        else
        {
            Debug.LogError("Players slot is full");
        }

        while (GameManager._instance.gameState != GameState.PostInstantiate)
        {
            Debug.Log("GameManager.gameState != GameState.PostInstantiate");

            yield return null;
        }
        BalootPlayer[] players = FindObjectsOfType<BalootPlayer>();
        foreach (var item in players)
        {
            if (item.GetComponent<PhotonView>().IsMine)
            {
                balootPlayer = item;
                Debug.Log("Got Player");
                break;
            }
        }
        photonViewComponent.RPC(nameof(AssignPlayer), RpcTarget.AllBufferedViaServer,new object[] { balootPlayer});

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
        if (!PlayerManager._instance.CanPlayerJoinGame())
        {


           /* SetGameState(GameState.GameStarted);
            SetGameData();
            UpdateGameStateToAllPun();*/

        }


        while (GameManager._instance.gameState == GameState.WaitingForOpponent)
        {
            Debug.Log("waiting for player");
            //GameObject.FindGameObjectWithTag("Loading").GetComponent<TextMeshProUGUI>().text = "Waiting for player ....";
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
[PunRPC]
    private void AssignPlayer(BalootPlayer balootPlayer)
    {
        Debug.Log(balootPlayer);
        PlayerManager._instance.AssignPlayer(balootPlayer); 
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