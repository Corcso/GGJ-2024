using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    public bool stateChangedThisFrame; // Serialized field for now

    // Player registry
    [SerializeField] GameObject[] playersToSpawnPrefabs;
    [SerializeField] int currentPlayerCount;
    [SerializeField] PlayerController[] playersInPlay; // Serialized field for now
    [SerializeField] Color[] coloursOfPlayers; // Serialized field for now
    [SerializeField] int chaserIndex;
    [SerializeField] int chaseeIndex;
    Vector3 camLookPosition;

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

    // GUI Popups
    [SerializeField] RectTransform caughtPopupText;
    [SerializeField] RectTransform escapedPopupText;
    float timeInGuiPopup;
    bool midPopupRan;
    void Start()
    {
        // Set initial game state
        currentGameState = GameState.CHASING;

        chaseSceneKeys = new KeyCode[currentPlayerCount];
        numberOfConsecutivePresses = new int[currentPlayerCount];
        playerKeyPopups = new GameObject[currentPlayerCount];

        //spawnPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGameState == GameState.CHASING) {
            // Perform chase set up if just happened
            if (stateChangedThisFrame) { 

                assignNewKey(chaserIndex);
                assignNewKey(chaseeIndex);

                playersInPlay[chaserIndex].isChasee = false;
                playersInPlay[chaseeIndex].isChasee = true;
                playersInPlay[chaserIndex].inChase = true;
                playersInPlay[chaseeIndex].inChase = true;
            }

            // Chaser has caught chasee if they are overlapping
            if (playersInPlay[chaserIndex].currentPlacementAngle >= playersInPlay[chaseeIndex].currentPlacementAngle) {
                setGameState(GameState.CAUGHT_ANIMATION);
                // Wipe chase keys
                for (int i = 0; i < currentPlayerCount; i++)
                {
                    chaseSceneKeys[i] = KeyCode.None;
                    numberOfConsecutivePresses[i] = 0;
                }
            }

            camLookPosition = playersInPlay[chaseeIndex].transform.position;
        }
        if (currentGameState == GameState.CAUGHT_ANIMATION && stateChangedThisFrame) {
            caughtPopupText.GameObject().SetActive(true);
            caughtPopupText.localScale = Vector3.zero;
            timeInGuiPopup = 0;
            midPopupRan = false;
        }
        if (currentGameState == GameState.CAUGHT_ANIMATION) {
            timeInGuiPopup += Time.deltaTime;
            caughtPopupText.localScale = Vector3.one * (2 * Mathf.Sin(4 * timeInGuiPopup - (3.141f / 2)) + 2);
            if (!midPopupRan && timeInGuiPopup >= (3.141f / 4)) {
                playersInPlay[chaseeIndex].setAsChasee();
                playersInPlay[chaserIndex].evadedCapture();
                chaserIndex = -1;
                midPopupRan = true;
            }
            if (timeInGuiPopup >= (3.141f / 2))
            {
                setGameState(GameState.CHOOSING);
                caughtPopupText.GameObject().SetActive(false);

            } 
        }
        if (currentGameState == GameState.ESCAPE_ANIMATION && stateChangedThisFrame)
        {
            escapedPopupText.GameObject().SetActive(true);
            escapedPopupText.localScale = Vector3.zero;
            timeInGuiPopup = 0;
            midPopupRan = false;
        }
        if (currentGameState == GameState.ESCAPE_ANIMATION)
        {
            timeInGuiPopup += Time.deltaTime;
            escapedPopupText.localScale = Vector3.one * (2 * Mathf.Sin(4 * timeInGuiPopup - (3.141f / 2)) + 2);
            if (!midPopupRan && timeInGuiPopup >= (3.141f / 4))
            {
                playersInPlay[chaserIndex].angleAtHome = playersInPlay[chaserIndex].angleAtHome;
                playersInPlay[chaseeIndex].evadedCapture();
                playersInPlay[chaserIndex].setAsChasee();
                chaseeIndex = chaserIndex;
                chaserIndex = -1;
                midPopupRan = true;
            }
            if (timeInGuiPopup >= (3.141f / 2))
            {
                setGameState(GameState.CHOOSING);
                escapedPopupText.GameObject().SetActive(false);

            }
        }
        Camera.main.transform.LookAt(camLookPosition);
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0, Camera.main.transform.rotation.eulerAngles.y, 0));
    }

    private void LateUpdate()
    {
        if (stateChangedThisFrame) stateChangedThisFrame = false;
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
                int removeOne = (i > indexOfChasee) ? 1 : 0;
                playerController.currentPlacementAngle = (i - removeOne) * (2 * 3.1415f / (currentPlayerCount - 1));
                newPlayer.transform.position = new Vector3((sitRadius * Mathf.Sin(playerController.currentPlacementAngle)),
                                                0,
                                                (sitRadius * Mathf.Cos(playerController.currentPlacementAngle)));

                newPlayer.transform.LookAt(Camera.main.transform.position);
            }
            else {
                playerController.playerIndex = indexOfChasee;
                playerController.currentPlacementAngle = 0;
                newPlayer.transform.position = new Vector3(0, 0, chaseRadius);
                newPlayer.transform.LookAt(Camera.main.transform.position);
            }
        }
    }
}
