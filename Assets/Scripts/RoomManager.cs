using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using Newtonsoft;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using Unity.IO.LowLevel.Unsafe;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager _instance;
    public TextMeshProUGUI statusText;
    PhotonView photonViewComponent;
    public PlayerClass balootPlayerClass;
    public string photonId;
    public string nameOfPlayer;
    internal BalootPlayer recentlyJoinedPlayer;

    public bool DidTimeout { private set; get; }
    public static readonly RoomOptions s_RoomOptions = new RoomOptions
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
    [ContextMenu("check")]
    public void Check()
    {
        ExitGames.Client.Photon.Hashtable properties = new() ;
        var str = JsonConvert.SerializeObject(new List<PlayerClass>() { new("stg", "123") });
        properties.Add("playerData",str);
        Debug.Log(str);
        Debug.Log(properties["playerData"]);
       

    }
    private void Start()
    {
        JoinOrCreateRoom("32531ss5saf");
    }
    public void Leave()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
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
        Debug.LogError("Created Room");
        //InstantiateGame();
        //ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
        //properties.Add("playerList", JsonConvert.SerializeObject( new List<PlayerClass>()));
        //PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        //Debug.LogError(GetPlayersFromRoomProp().Count);
        //photonViewComponent.RPC(nameof(InstantiateGame), RpcTarget.AllBufferedViaServer);

        //Debug.LogError(PhotonNetwork.CurrentRoom.PlayerCount);
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room");
        photonId = PhotonNetwork.LocalPlayer.UserId;
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
        {
            Debug.Log($"Connecting to server ,state ={PhotonNetwork.NetworkClientState}");
            PhotonNetwork.JoinLobby();

        }

        while (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer)
        {

            yield return null;
        }


        if (preferredRoomName != null)
        {
            Debug.LogError("Joining or creating Room");

            bool isJoined = PhotonNetwork.JoinOrCreateRoom(preferredRoomName, s_RoomOptions, TypedLobby.Default);
        }
        else
        {
            Debug.LogError("Joined Random Room");

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
        //PhotonNetwork.InstantiateRoomObject("GameManager", new Vector3(), Quaternion.identity);
    }
    [PunRPC]
    public void InstantiatePlayer()
    {
        Debug.Log("Player Created");
        var obj = Instantiate(Resources.Load<GameObject>("Player"));
            //balootPlayer = obj.GetComponent<BalootPlayer>();
        Debug.LogError("Instantiated Player");
        BalootPlayer[] players = FindObjectsOfType<BalootPlayer>();
        Debug.Log(players.Length);
        foreach (var item in players)
        {

            if (item.GetComponent<PhotonView>().IsMine)
            {
                //balootPlayer = item;
                Debug.Log("Got Player");
                break;
            }
        }
        if (players.Length == 2)
        {

            Debug.LogError("Cards distributed");
            //photonViewComponent.RPC(nameof(GiveCardsToPlayerPun), RpcTarget.AllBufferedViaServer);
        }
        GameManager._instance.gameState = GameState.PostInstantiate;
        
    }
    void AddNewPlayerRoomProp()
    {
        balootPlayerClass = new PlayerClass() { playerName = GameUIManager._instance.nameOfPlayer, photonId = PhotonNetwork.LocalPlayer.UserId };


        ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
        //Debug.LogError((string)PhotonNetwork.CurrentRoom.CustomProperties["playerList"]);

     
        if (!properties.ContainsKey("playerList"))
        {
            var playerList = new List<PlayerClass>();

            Debug.LogError(playerList.Count);
            playerList.Add(balootPlayerClass);
            properties.Add("playerList", JsonConvert.SerializeObject(playerList));

        }
        else
        {
            Debug.LogError((string)PhotonNetwork.CurrentRoom.CustomProperties["playerList"]);
            var playerList = JsonConvert.DeserializeObject<List<PlayerClass>>((string)PhotonNetwork.CurrentRoom.CustomProperties["playerList"]);
            Debug.LogError(playerList.Count);
            playerList.Add(balootPlayerClass);
            properties["playerList"] = JsonConvert.SerializeObject(playerList);
        }
        Debug.LogError(properties["playerList"]);
        //Debug.LogError(playerList[0].playerName);
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);

        Debug.LogError((string)PhotonNetwork.CurrentRoom.CustomProperties["playerList"]);
    }
    public List<PlayerClass> GetPlayersFromRoomProp()
    {

        ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
        var list = new List<PlayerClass>();
        if (!properties.ContainsKey("playerList"))
        {
            Debug.LogError("player list not found");
            return list;
        }
        else
        {
            Debug.LogError((string)PhotonNetwork.CurrentRoom.CustomProperties["playerList"]);
            string str = (string)properties["playerList"];
            Debug.LogError(str);
            var players = JsonConvert.DeserializeObject<List<PlayerClass>>(str);
            Debug.LogError(players.Count);
            return players;
        }

    }
    public CardManager GetCardManagerFromRoomProp()
    {
        ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
        if (!properties.ContainsKey("cardManager"))
        {
            Debug.LogError("card manager not found");
            return null;
        }
        else
        {

            string str = (string)properties["cardManager"];
            Debug.LogError(str);

            var cardManager = JsonConvert.DeserializeObject<CardManager>(str);
            Debug.LogError(cardManager);
            return cardManager;

        }
    }
    public void SetCardManagerInRoomProp()
    {


        ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
        //Debug.LogError((string)PhotonNetwork.CurrentRoom.CustomProperties["playerList"]);


        if (!properties.ContainsKey("cardManager"))
        {

            properties.Add("cardManager", JsonConvert.SerializeObject(BalootGameManager._instance.cardManager));

        }
        else
        {
            properties["cardManager"] = JsonConvert.SerializeObject(BalootGameManager._instance.cardManager);
        }
        Debug.LogError(properties["cardManager"]);
        //Debug.LogError(playerList[0].playerName);
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);

        //Debug.LogError((string)PhotonNetwork.CurrentRoom.CustomProperties["cardManager"]);
    }
    public void SetInRoomProp(string key,object obj)
    {
        ExitGames.Client.Photon.Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
        //Debug.LogError((string)PhotonNetwork.CurrentRoom.CustomProperties["playerList"]);


        if (!properties.ContainsKey(key))
        {

            properties.Add(key, JsonConvert.SerializeObject(obj));

        }
        else
        {
            properties[key] = JsonConvert.SerializeObject(obj);
        }
        Debug.LogError(properties[key]);
        //Debug.LogError(playerList[0].playerName);
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }
    IEnumerator SynchroniseGame()
    {


        while (GameManager._instance == null )
        {
            yield return null;
        }
        yield return new WaitForSeconds(1);
        AddNewPlayerRoomProp();

        //      if (!PlayerManager._instance.CanPlayerJoinGame())
        //{
        //    StopCoroutine(SynchroniseGame());
        //}



        //while (json == null)
        //{
        //    yield return null;
        //}
        //        //photonViewComponent.RPC(nameof(InstantiateAndAssignPlayer), RpcTarget.AllBufferedViaServer);
        //while(balootPlayerClass==null )
        //{
        //    yield return null;
        //}


        while (GameManager._instance.gameState == GameState.WaitingForOpponent)
        {
            yield return null;

        }
        if (PhotonNetwork.IsMasterClient)

        {
            SetGameData();
        }
        else
        {
                   
        }
     
        GameManager._instance.gameState = GameState.WaitingForOpponent;
        //if (!PlayerManager._instance.CanPlayerJoinGame())
        //{


        //}


            Debug.Log("waiting for player");
        while (GameManager._instance.gameState == GameState.WaitingForOpponent)
        {
            //Debug.LogError(GetPlayersFromRoomProp().Count);
            //Debug.LogError(PhotonNetwork.CurrentRoom.MaxPlayers);
            //Debug.LogError(PhotonNetwork.CurrentRoom.PlayerCount);
            if (PhotonNetwork.CurrentRoom.PlayerCount== PhotonNetwork.CurrentRoom.MaxPlayers && GetPlayersFromRoomProp().Count == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                GameManager._instance.gameState = GameState.InGame;
            }
            yield return null;
        }
        Debug.LogError(GetPlayersFromRoomProp().Count);
        Debug.LogError(PhotonNetwork.CurrentRoom.MaxPlayers);
        Debug.LogError(PhotonNetwork.CurrentRoom.PlayerCount);

        while (GameManager._instance.gameState != GameState.InGame)
        {
            yield return null;
        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            BalootGameManager._instance.NewGame();
        }
        

    }
    [PunRPC]
    private void InstantiateAndAssignPlayer()
    {
        //var balootPlayerClass = JsonConvert.DeserializeObject<PlayerClass>(balootPlayer);

        //PlayerManager._instance.AssignPlayer(balootPlayerClass);

        
    }

    //[PunRPC]
    //void GiveCardsToPlayerPun()
    //{
    //    BalootGameManager._instance.NewGame();
    //}
    //[PunRPC]
    //private void AssignPlayer(string balootPlayer)
    //{
    //    var balootPlayerClass = JsonConvert.DeserializeObject<PlayerClass>(balootPlayer);
    //    Debug.Log(balootPlayer);
    //    Debug.Log(nameOfPlayer);
    //   PlayerManager._instance.AssignPlayer(balootPlayerClass);
        
    //        }

    internal void SetGameData()
    {

           }
         
  
    #endregion
}