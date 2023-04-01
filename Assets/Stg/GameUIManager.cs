using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager _instance;
    public GameObject[] slots;
    public TextMeshProUGUI[] nameTitles;

    private void Awake()
    {
        _instance = this;
        slots = new GameObject[transform.childCount];
        nameTitles = new TextMeshProUGUI[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            slots[i] = transform.GetChild(i).gameObject;
            nameTitles[i] = transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
            nameTitles[i].text = "";
        }
    }
}
