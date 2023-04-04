using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalootPlayer : MonoBehaviour
{
    public BalootPlayerClass balootPlayerClass;
    private void Awake()
    {
        balootPlayerClass = new();
    }
}
[System.Serializable]
public class BalootPlayerClass
{
    public string playerName;
    public List<CardClass> cards = new();
}
