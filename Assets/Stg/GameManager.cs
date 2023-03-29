using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    public GameState gameState;
    private void Awake()
    {
        _instance = this;
    }
       
    
}
public enum GameState { Initiating, PostInstantiate, WaitingForOpponent, InGame }
