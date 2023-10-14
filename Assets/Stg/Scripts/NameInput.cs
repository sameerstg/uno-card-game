using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameInput : MonoBehaviour
{
    public GameObject roomManager;
    public TMP_InputField text;
    private void Awake()
    {
        text = GetComponentInChildren<TMP_InputField>();
        GetComponentInChildren<Button>().onClick.AddListener(() => OnButtonPressed());
    }
    void OnButtonPressed()
    {
        if (string.IsNullOrEmpty( text.text))
        {
            return;
        }
        PlayerPrefs.SetString("username", text.text);
        roomManager.SetActive(true);
        
        GameUIManager._instance.slotParrent.gameObject.SetActive(true);
        GameUIManager._instance.playedCardSlot.gameObject.SetActive(true);
        gameObject.SetActive(false);
        GameUIManager._instance.nameOfPlayer = text.text;
    }
   }
