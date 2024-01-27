using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public enum GameState { 
        CHOOSING,
        CHASING,
        CAUGHT_ANIMATION,
        ESCAPE_ANIMATION
    }
    public GameState currentGameState;
    [SerializeField] bool stateChangedThisFrame; // Serialized field for now

    // Player registry
    [SerializeField] int currentPlayerCount;
    [SerializeField] PlayerController[] playersInPlay; // Serialized field for now
    [SerializeField] int chaserIndex;
    [SerializeField] int chaseeIndex;

    // Chase Round
    public KeyCode[] chaseSceneKeys;
    int[] numberOfConsecutivePresses;

    // Game Settings
    [SerializeField] public float chaseRadius;
    [SerializeField] public float sitRadius;
    [SerializeField] public float chaseBobAmp;
    [SerializeField] public float chaseBobFreq;
    [SerializeField] public float chaseMinAngSpeed;
    [SerializeField] public float chaseMaxAngSpeed;

    [SerializeField] public float chaseMinDeceleration;
    [SerializeField] public float chaseDecelerationGradient;

    void Start()
    {
        // Set initial game state
        currentGameState = GameState.CHASING;

        chaseSceneKeys = new KeyCode[currentPlayerCount];
        numberOfConsecutivePresses = new int[currentPlayerCount];
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGameState == GameState.CHASING) {
            // Perform chase set up if just happened
            if (stateChangedThisFrame) { 
                stateChangedThisFrame = false;

                assignNewKey(chaserIndex);
                assignNewKey(chaseeIndex);
            }


            // Chaser has caught chasee if they are overlapping
            if (playersInPlay[chaserIndex].currentPlacementAngle >= playersInPlay[chaseeIndex].currentPlacementAngle) {
                currentGameState = GameState.CAUGHT_ANIMATION;
                // Wipe chase keys
                for (int i = 0; i < currentPlayerCount; i++)
                {
                    chaseSceneKeys[i] = KeyCode.None;
                    numberOfConsecutivePresses[i] = 0;
                }
            }
        }
    }

    public void countKeyPress(int playerIndex) {
        ++numberOfConsecutivePresses[playerIndex];
        if (numberOfConsecutivePresses[playerIndex] > 10 && Random.Range(0, 3) == 0) {
            assignNewKey(playerIndex);
        }
    }

    public void setGameState(GameState newGameState) { 
        currentGameState = newGameState; 
        stateChangedThisFrame = true;
    }

    void assignNewKey(int playerIndex) {
        bool keyUnique = false;
        KeyCode newKey = KeyCode.None;
        while (!keyUnique)
        {
            newKey = (KeyCode)Random.Range(97, 123);
            keyUnique = true;
            for (int i = 0; i < currentPlayerCount; i++)
            {
                if (newKey == chaseSceneKeys[i])
                {
                    keyUnique = false; break;
                }
            }
        }
        chaseSceneKeys[playerIndex] = newKey;
        numberOfConsecutivePresses[playerIndex] = 0;
        Debug.Log(newKey.ToString());

    }

}
