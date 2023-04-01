using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager _instance;
    public GameObject slotParrent;
    public GameObject[] slots;
    public TextMeshProUGUI[] nameTitles;
    public string nameOfPlayer;
    
    private void Awake()
    {
        _instance = this;
        slots = new GameObject[slotParrent.transform.childCount];
        nameTitles = new TextMeshProUGUI[slotParrent.transform.childCount];
        slotParrent.gameObject.SetActive(true);
        for (int i = 0; i < slotParrent.transform.childCount; i++)
        {
            
            slots[i] = slotParrent.transform.GetChild(i).gameObject;
            nameTitles[i] = slotParrent.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
            nameTitles[i].text = "";
        }
        slotParrent.gameObject.SetActive(false);


    }
}
