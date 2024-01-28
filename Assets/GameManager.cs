using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public enum GameState { 
        CHOOSING,
        CHASING,
        CAUGHT_ANIMATION,
        ESCAPE_ANIMATION,
        WALKING
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
    [SerializeField] int chosenPlayer;
    [SerializeField] int noOfDucks = 0;
    [SerializeField] bool inMovingTransition;
    float angleMovedThisSegment;

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
    [SerializeField] GameObject choosingButtonPrefab;
    [SerializeField] Transform choosingButtonPanel;

    // GUI Popups
    [SerializeField] RectTransform caughtPopupText;
    [SerializeField] RectTransform escapedPopupText;
    float timeInGuiPopup;
    bool midPopupRan;

    // Background changing 
    [SerializeField] Material backgroundMaterial;
    [SerializeField] Texture[] backgroundTextures;
    [SerializeField] public int currentBackground = 0;

    void Start()
    {
        // Change background texture, example
        backgroundMaterial.mainTexture = backgroundTextures[0];

        // Set initial game state
        currentGameState = GameState.CHASING;

        chaseSceneKeys = new KeyCode[currentPlayerCount];
        numberOfConsecutivePresses = new int[currentPlayerCount];
        playerKeyPopups = new GameObject[currentPlayerCount];
        playersInPlay = new PlayerController[currentPlayerCount];

        //spawnPlayers();

        for (int i = 0; i < (currentPlayerCount - 1) * 2; i++)
        {
            GameObject newButton = Instantiate(choosingButtonPrefab, choosingButtonPanel);
            newButton.GetComponent<Button>().onClick.AddListener(delegate { chooseStepsToTake(i + 1); });
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
        }
    }

    void changeBackground()
    {
        currentBackground += 1;

        if (currentBackground == 3)
        {
            currentBackground = 0;
        }
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
        if (currentGameState == GameState.WALKING)
        {
            //if it player angle is NOT equal to chosenPlayer * (2 pi / noOfPlayers)
                //move player by 2 pi / noOfPlayers
            //if (playersInPlay[chaserIndex].currentPlacementAngle >= playersInPlay[chaseeIndex].currentPlacementAngle)
            //{
            //    //change enum to chasing
            //    currentGameState = GameState.CHASING;
               
            //}
            if ((noOfDucks == chosenPlayer - 2) && !inMovingTransition)
            {
                //move chasee by 2 pi / noOfPlayers
                currentGameState = GameState.CHASING;
            }
            else
            {
                if (inMovingTransition == false)
                {
                    noOfDucks++;
                    //playersInPlay[chaseeIndex].currentPlacementAngle += (3.1415f * 2) / currentPlayerCount;
                    inMovingTransition = true;
                    angleMovedThisSegment = 0;
                    //move it normally somehow?
                    //Maybe the above if statement could take place when the angle is equal to noOfDucks - 1
                }
                else
                {
                    angleMovedThisSegment += (0.6f * Time.deltaTime);
                    playersInPlay[chaseeIndex].currentPlacementAngle += (0.6f * Time.deltaTime);
                    playersInPlay[chaseeIndex].transform.position = new Vector3((sitRadius * Mathf.Sin(playersInPlay[chaseeIndex].currentPlacementAngle)),
                                                0,
                                                (sitRadius * Mathf.Cos(playersInPlay[chaseeIndex].currentPlacementAngle)));

                    playersInPlay[chaseeIndex].transform.LookAt(Camera.main.transform.position);


                    if (angleMovedThisSegment >= (2 * 3.1415) / noOfDucks){
                        inMovingTransition = false;
                    }
                    
                }
            }
        }


        if (currentGameState == GameState.CHOOSING && stateChangedThisFrame)
        {
            choosingButtonPanel.gameObject.SetActive(true);

            

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
            playersInPlay[i] = playerController;
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

    public void chooseStepsToTake(int targetSteps)
    {
        setGameState(GameState.WALKING);
        noOfDucks = 0;
        chosenPlayer = targetSteps;
        choosingButtonPanel.gameObject.SetActive(false);
    }

}
