using UnityEngine;
using UnityEngine.UI;

public class LastCardPlayer : MonoBehaviour
{
    private void Start()
    {

        GetComponent<Button>().onClick.AddListener(() => PressLastCard());
    }

    private void PressLastCard()
    {
        Debug.LogError("s");
        if (BalootGameManager._instance.cardManager.turn != RoomManager._instance.localPlayerTurn)
        {
            return;
        }
        BalootGameManager._instance.cardManager.playerClasses[RoomManager._instance.localPlayerTurn].lastCardPressed = true;
        Debug.Log("Last Card Pressed");
        gameObject.SetActive(false);
    }
}
