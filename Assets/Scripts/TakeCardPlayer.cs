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
        if (BalootGameManager._instance.cardManager.turn != RoomManager._instance.localPlayerTurn)
        {
            return;
        }

        BalootGameManager._instance.cardManager.TakeCard();
        if (!BalootGameManager._instance.cardManager.CanPlay(BalootGameManager._instance.cardManager.playerClasses[RoomManager._instance.indexInGlobalPlayerList]))
        {
            BalootGameManager._instance.cardManager.ChangeTurn();
        }
        GameUIManager._instance.slots[BalootGameManager._instance.cardManager.turn].takeCard.gameObject.SetActive(false);
        Debug.Log("Card Taken From Remaining Deck");
    }
}
