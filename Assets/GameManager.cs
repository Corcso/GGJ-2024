using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] GameObject[] playersToSpawnPrefabs;
    [SerializeField] int currentPlayerCount;
    [SerializeField] PlayerController[] playersInPlay; // Serialized field for now
    [SerializeField] Color[] coloursOfPlayers; // Serialized field for now
    [SerializeField] int chaserIndex;
    [SerializeField] int chaseeIndex;

    // Chase Round
    public KeyCode[] chaseSceneKeys;
    int[] numberOfConsecutivePresses;
    [SerializeField] Transform chasePromptZone;
    [SerializeField] GameObject playerMashPopupPrefab;
    GameObject[] playerKeyPopups;

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
        currentGameState = GameState.CHOOSING;

        chaseSceneKeys = new KeyCode[currentPlayerCount];
        numberOfConsecutivePresses = new int[currentPlayerCount];
        playerKeyPopups = new GameObject[currentPlayerCount];

        spawnPlayers();
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

        Destroy(playerKeyPopups[playerIndex]);
        GameObject newPopup = Instantiate(playerMashPopupPrefab, chasePromptZone);
        newPopup.GetComponent<TextMeshProUGUI>().text = newKey.ToString();
        newPopup.GetComponent<KeyPopupScript>().myKey = newKey;
        newPopup.GetComponent<TextMeshProUGUI>().color = coloursOfPlayers[playerIndex];
        playerKeyPopups[playerIndex] = newPopup;
    }
    void spawnPlayers() {
        int indexOfChasee = Random.Range(0, currentPlayerCount);

        for (int i = 0; i < currentPlayerCount; i++)
        {
            GameObject newPlayer = Instantiate(playersToSpawnPrefabs[i]);
            PlayerController playerController = newPlayer.GetComponent<PlayerController>();
            playerController.playerIndex = i;

            if (i != indexOfChasee)
            {
                // Set player location and position
                playerController.currentPlacementAngle = i * (2 * 3.1415f / (currentPlayerCount - 1));
                newPlayer.transform.position = new Vector3((sitRadius * Mathf.Sin(playerController.currentPlacementAngle)),
                                                0,
                                                (sitRadius * Mathf.Cos(playerController.currentPlacementAngle)));

                newPlayer.transform.LookAt(Camera.main.transform.position);
            }
            else {
                // Set player location and position; 
                playerController.currentPlacementAngle = 0;
                newPlayer.transform.position = new Vector3(0, 0, chaseRadius);

                newPlayer.transform.LookAt(Camera.main.transform.position);
            }
        }
    }
}
