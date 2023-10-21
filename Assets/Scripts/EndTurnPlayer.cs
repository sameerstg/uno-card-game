using UnityEngine;
using UnityEngine.UI;

public class EndTurnPlayer : MonoBehaviour
{
    //private void OnEnable()
    //{
    //    if (BalootGameManager._instance.cardManager.playerClasses[RoomManager._instance.indexInGlobalPlayerList].cards.Count == 0)
    //    { 
    //        GameUIManager._instance.slots[RoomManager._instance.indexInGlobalPlayerList].lastCard.SetActive(true);
    //    }
    //}
    private void Start()
    {

        GetComponent<Button>().onClick.AddListener(() => EndTurn());
    }

    private void EndTurn()
    {
        Debug.LogError("s");
        if (BalootGameManager._instance.cardManager.GetPlayerByTurn().photonId != RoomManager._instance.photonId)
        {
            return;
        }
        if(BalootGameManager._instance.cardManager.GetPlayerByTurn().cards.Count == 1 && !BalootGameManager._instance.cardManager.GetPlayerByTurn().lastCardPressed)
        {
            BalootGameManager._instance.cardManager.TakeCard();
        }
        BalootGameManager._instance.cardManager.ChangeTurn();
        BalootGameManager._instance.SyncCardManager();
        Debug.Log("Turn Ended");
        gameObject.SetActive(false);
    }
}
