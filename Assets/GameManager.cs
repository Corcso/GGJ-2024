using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Player registry
    [SerializeField] PlayerController[] playersInPlay; // Serialized field for now
    [SerializeField] int chaserIndex;
    [SerializeField] int chaseeIndex;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGameState == GameState.CHASING) {
            // Chaser has caught chasee if they are overlapping
            if (playersInPlay[chaserIndex].currentPlacementAngle >= playersInPlay[chaseeIndex].currentPlacementAngle) {
                currentGameState = GameState.CAUGHT_ANIMATION;
            }
        }
    }

}
