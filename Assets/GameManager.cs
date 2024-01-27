using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public enum GameState { 
        CHOOSING,
        CHASING
    }
    public GameState currentGameState;

    // Game Settings
    [SerializeField] public float chaseRadius;
    [SerializeField] public float sitRadius;
    [SerializeField] public float chaseBobAmp;
    [SerializeField] public float chaseBobFreq;
    [SerializeField] public float chaseMinAngSpeed;
    [SerializeField] public float chaseMaxAngSpeed;

    void Start()
    {
        // Set initial game state
        currentGameState = GameState.CHASING;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
