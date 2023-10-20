using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalootPlayer : MonoBehaviour
{
    public PlayerClass balootPlayerClass;
    private void Awake()
    {
        //balootPlayerClass = new();
    }
}
[System.Serializable]
public class PlayerClass
{
    public string playerName;
    public string photonId;
    public List<CardClass> cards = new();
    public int turnNumber;
    public bool cardTaken = false;
    public bool lastCardPressed = false;

    public PlayerClass()
    {

    }
    public PlayerClass(string name)
    {
        playerName = name;
    }

}
