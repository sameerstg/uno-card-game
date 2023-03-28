using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalootGameManager : MonoBehaviour
{
    public static BalootGameManager _instance;
    
    public Baloot baloot;
    private void Awake()
    {
        _instance = this;
        baloot = new();

    }
   }
