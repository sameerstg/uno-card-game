using UnityEngine;
using UnityEngine.UI;

public class TakeCardPlayer : MonoBehaviour
{
    private void Start()
    {

        GetComponent<Button>().onClick.AddListener(() => TakeCard());
    }

    private void TakeCard()
    {
        Debug.LogError("s");
        if (BalootGameManager._instance.cardManager.GetPlayerByTurn().photonId != RoomManager._instance.photonId)
        {
            return;
        }

        BalootGameManager._instance.cardManager.TakeCard(true);
        if (!BalootGameManager._instance.cardManager.CanPlay(BalootGameManager._instance.cardManager.GetPlayerByTurn()))
        {
            BalootGameManager._instance.cardManager.AddAccruedCardsOnChangeTurn();
            BalootGameManager._instance.cardManager.ChangeTurn();
            BalootGameManager._instance.SyncCardManager();
        }
        //GameUIManager._instance.slots[BalootGameManager._instance.cardManager.turn].takeCard.gameObject.SetActive(false);
        Debug.Log("Card Taken From Remaining Deck");
    }
}
