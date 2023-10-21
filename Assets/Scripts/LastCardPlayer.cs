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
        if (BalootGameManager._instance.cardManager.GetPlayerByTurn().photonId != RoomManager._instance.photonId)
        {
            return;
        }
        BalootGameManager._instance.cardManager.GetPlayerByTurn().lastCardPressed = true;
        Debug.Log("Last Card Pressed");
        gameObject.SetActive(false);
    }
}
