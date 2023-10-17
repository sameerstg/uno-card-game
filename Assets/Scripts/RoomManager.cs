using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using Newtonsoft;
using Newtonsoft.Json;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager _instance;
    public TextMeshProUGUI statusText;
    PhotonView photonViewComponent;
    public PlayerClass balootPlayerClass;
    public int localPlayerTurn;
    public string nameOfPlayer;
    internal BalootPlayer recentlyJoinedPlayer;
    public int indexInGlobalPlayerList ;

    public bool DidTimeout { private set; get; }
    static readonly RoomOptions s_RoomOptions = new RoomOptions
    {
        MaxPlayers = 2,
        EmptyRoomTtl = 5,
        PublishUserId = true,

    };

    void Awake()
    {
        indexInGlobalPlayerList = -1;
        _instance = this;
        Assert.AreEqual(1, FindObjectsOfType<RoomManager>().Length);
        photonViewComponent = GetComponent<PhotonView>();
        localPlayerTurn = -1;
        


    }
    private void Start()
    {
        JoinOrCreateRoom("32531ss5saf");
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


        photonViewComponent.RPC(nameof(InstantiateGame), RpcTarget.AllBufferedViaServer);

        Debug.LogError(PhotonNetwork.CurrentRoom.PlayerCount);
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
        PhotonNetwork.InstantiateRoomObject("GameManager", new Vector3(), Quaternion.identity);
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
            photonViewComponent.RPC(nameof(GiveCardsToPlayerPun), RpcTarget.AllBufferedViaServer);
        }
        GameManager._instance.gameState = GameState.PostInstantiate;
        
    }
    IEnumerator SynchroniseGame()
    {


        while (GameManager._instance == null )
        {
            yield return null;
        }
              if (!PlayerManager._instance.CanPlayerJoinGame())
        {
            StopCoroutine(SynchroniseGame());
        }

              balootPlayerClass = new PlayerClass() { playerName = GameUIManager._instance.nameOfPlayer ,photonId = PhotonNetwork.LocalPlayer.UserId};

        var json = JsonConvert.SerializeObject(balootPlayerClass);
        while (json == null)
        {
            yield return null;
        }
        localPlayerTurn = -1;
                photonViewComponent.RPC(nameof(InstantiateAndAssignPlayer), RpcTarget.AllBufferedViaServer,new object[] {json } );
        while(localPlayerTurn == -1 || balootPlayerClass==null )
        {
            yield return null;
        }


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
        if (!PlayerManager._instance.CanPlayerJoinGame())
        {


        }


            Debug.Log("waiting for player");
        while (GameManager._instance.gameState == GameState.WaitingForOpponent)
        {
            if (!PlayerManager._instance.CanPlayerJoinGame())
            {
                GameManager._instance.gameState = GameState.InGame;
            }
            yield return null;
        }


        while (GameManager._instance.gameState != GameState.InGame)
        {
            yield return null;
        }
       
        
        

    }
    [PunRPC]
    private void InstantiateAndAssignPlayer(string balootPlayer)
    {
        var balootPlayerClass = JsonConvert.DeserializeObject<PlayerClass>(balootPlayer);
        
        PlayerManager._instance.AssignPlayer(balootPlayerClass);

        
    }

    [PunRPC]
    void GiveCardsToPlayerPun()
    {
        BalootGameManager._instance.NewGame();
    }
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