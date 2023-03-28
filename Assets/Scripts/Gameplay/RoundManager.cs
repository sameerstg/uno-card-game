using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI; 
using TMPro;

public class RoundManager : MonoBehaviour
{ 
    public enum GameState { PlayerTurn, Bot1Turn, TeammateTurn, Bot2Turn, GameEnd }; 
    public float playerTimeLeft, bot1TimeLeft, bot2TimeLeft, teammateTimeLeft; 
    public GameState state;
    public TMP_Text timerText;
    public static event Action<GameState> OnGameStateChanged;

    private bool isTimerRunning;
    private float currentTime;

    private void Awake()
    {
        // Set the initial state of the game to the player's turn
        state = GameState.PlayerTurn;
    }

    private void Start()
    {
        // Start the timer when the game starts
        StartTimer();
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0f)
            {
                // End the current turn and switch to the next turn
                EndTurn();
            }
            else
            {
                // Update the timer text
                UpdateTimerText(currentTime);
            }
        }
    }

    private void StartTimer()
    {
        // Set the timer to 10 seconds and start it
        currentTime = 10f;
        isTimerRunning = true;
        UpdateTimerText(currentTime);
    }

    private void StopTimer()
    {
        // Stop the timer
        isTimerRunning = false;
    }

    private void UpdateTimerText(float time)
    {
        // Format the time as mm:ss and update the timer text
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        timerText.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
    }

    private void EndTurn()
    {
        // Stop the timer and switch to the next turn
        StopTimer();

        switch (state)
        {
            case GameState.PlayerTurn:
                state = GameState.Bot1Turn;
                break;
            case GameState.Bot1Turn:
                state = GameState.TeammateTurn;
                break;
            case GameState.TeammateTurn:
                state = GameState.Bot2Turn;
                break;
            case GameState.Bot2Turn:
                state = GameState.PlayerTurn;
                break;
        }

        // Raise the OnGameStateChanged event
        OnGameStateChanged?.Invoke(state);

        // Start the timer for the next turn
        StartTimer();
    }
}
