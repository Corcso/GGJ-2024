using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        WALKING,
        RUN_ANIMATION
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
    float timeOnPatAnimation;

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
    [SerializeField] RectTransform runPopupText;
    float timeInGuiPopup;
    bool midPopupRan;

    // Background changing 
    [SerializeField] Material backgroundMaterial;
    [SerializeField] Texture[] backgroundTextures;
    [SerializeField] public int currentBackground = 0;

    globalSettingsManager settingsManager;
    void Awake()
    {
        settingsManager = GameObject.Find("GLOBAL_SETTINGS").GetComponent<globalSettingsManager>();
        playersToSpawnPrefabs = settingsManager.playersToSpawnPrefabs;
        currentPlayerCount = settingsManager.currentPlayerCount;
        coloursOfPlayers = settingsManager.playerColors;
    }

    void Start()
    {
        // Set initial game state
        currentGameState = GameState.CHOOSING;

        chaseSceneKeys = new KeyCode[currentPlayerCount];
        numberOfConsecutivePresses = new int[currentPlayerCount];
        playerKeyPopups = new GameObject[currentPlayerCount];
        playersInPlay = new PlayerController[currentPlayerCount];

        spawnPlayers();

        for (int i = 0; i < (currentPlayerCount - 1) * 2; i++)
        {
            GameObject newButton = Instantiate(choosingButtonPrefab, choosingButtonPanel);
            int iPlusOne = i + 1;
            newButton.GetComponent<Button>().onClick.AddListener(delegate { this.chooseStepsToTake(iPlusOne); });
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

        backgroundMaterial.mainTexture = backgroundTextures[currentBackground];
    }

    // Update is called once per frame
    void Update()
    {
        
        if (currentGameState == GameState.CAUGHT_ANIMATION && stateChangedThisFrame) {
            caughtPopupText.GameObject().SetActive(true);
            caughtPopupText.localScale = Vector3.zero;
            timeInGuiPopup = 0;
            midPopupRan = false;
            stateChangedThisFrame = false;
        }
        if (currentGameState == GameState.WALKING && stateChangedThisFrame) {
            inMovingTransition = false;
            noOfDucks = 0;
            stateChangedThisFrame = false;
            int closestChickenIndex = -1;
            float closestChickenDistance = float.MaxValue;
            for (int i = 0; i < currentPlayerCount; i++)
            {
                if (i == chaseeIndex) continue;
                if (closestChickenDistance > Vector3.Distance(playersInPlay[chaseeIndex].transform.position, playersInPlay[i].transform.position))
                {
                    closestChickenIndex = i;
                    closestChickenDistance = Vector3.Distance(playersInPlay[chaseeIndex].transform.position, playersInPlay[i].transform.position);
                }
            }
            playersInPlay[closestChickenIndex].setSprite(PlayerController.SpriteFrameType.PATTED);
        }
        if (currentGameState == GameState.WALKING)
        {
            camLookPosition = playersInPlay[chaseeIndex].transform.position;
            
            
            if (inMovingTransition == false)
            {
                
                //playersInPlay[chaseeIndex].currentPlacementAngle += (3.1415f * 2) / currentPlayerCount;
                timeOnPatAnimation += Time.deltaTime;

                if (timeOnPatAnimation >= 1.0f) { 
                    inMovingTransition = true;
                    angleMovedThisSegment = 0;
                    noOfDucks++;
                    Debug.Log(noOfDucks);
                    playersInPlay[chaseeIndex].setSprite(PlayerController.SpriteFrameType.WALKING);
                    int closestChickenIndex = -1;
                    float closestChickenDistance = float.MaxValue;
                    for (int i = 0; i < currentPlayerCount; i++)
                    {
                        if (i == chaseeIndex) continue;
                        if (closestChickenDistance > Vector3.Distance(playersInPlay[chaseeIndex].transform.position, playersInPlay[i].transform.position))
                        {
                            closestChickenIndex = i;
                            closestChickenDistance = Vector3.Distance(playersInPlay[chaseeIndex].transform.position, playersInPlay[i].transform.position);
                        }
                    }
                    playersInPlay[closestChickenIndex].setSprite(PlayerController.SpriteFrameType.SITTING);

                }
                
                //move it normally somehow?
                //Maybe the above if statement could take place when the angle is equal to noOfDucks - 1
                if ((noOfDucks == chosenPlayer))
                {
                    setGameState(GameState.RUN_ANIMATION);
                    int closestChickenIndex = -1;
                    float closestChickenDistance = float.MaxValue;
                    for (int i = 0; i < currentPlayerCount; i++) {
                        if (i == chaseeIndex) continue;
                        if (closestChickenDistance > Vector3.Distance(playersInPlay[chaseeIndex].transform.position, playersInPlay[i].transform.position)) {
                            closestChickenIndex = i;
                            closestChickenDistance = Vector3.Distance(playersInPlay[chaseeIndex].transform.position, playersInPlay[i].transform.position);
                        }
                    }
                    chaserIndex = closestChickenIndex;
                }
            }
            else
            {
                angleMovedThisSegment += (0.6f * Time.deltaTime);
                playersInPlay[chaseeIndex].currentPlacementAngle += (0.6f * Time.deltaTime);
                playersInPlay[chaseeIndex].transform.position = new Vector3((chaseRadius * Mathf.Sin(playersInPlay[chaseeIndex].currentPlacementAngle)),
                                            0,
                                            (chaseRadius * Mathf.Cos(playersInPlay[chaseeIndex].currentPlacementAngle)));

                playersInPlay[chaseeIndex].transform.LookAt(Camera.main.transform.position);


                if (angleMovedThisSegment >= (2 * 3.1415) / (currentPlayerCount - 1)){
                    inMovingTransition = false;
                    Debug.Log("MOVED SEGMENT");
                    playersInPlay[chaseeIndex].setSprite(PlayerController.SpriteFrameType.PATTING);
                    int closestChickenIndex = -1;
                    float closestChickenDistance = float.MaxValue;
                    for (int i = 0; i < currentPlayerCount; i++)
                    {
                        if (i == chaseeIndex) continue;
                        if (closestChickenDistance > Vector3.Distance(playersInPlay[chaseeIndex].transform.position, playersInPlay[i].transform.position))
                        {
                            closestChickenIndex = i;
                            closestChickenDistance = Vector3.Distance(playersInPlay[chaseeIndex].transform.position, playersInPlay[i].transform.position);
                        }
                    }
                    playersInPlay[closestChickenIndex].setSprite(PlayerController.SpriteFrameType.PATTED);
                    timeOnPatAnimation = 0;
                }
                
            }
            
        }


        if (currentGameState == GameState.CHOOSING && stateChangedThisFrame)
        {
            choosingButtonPanel.gameObject.SetActive(true);
            stateChangedThisFrame = false;
        }
        if (currentGameState == GameState.CAUGHT_ANIMATION) {
            timeInGuiPopup += Time.deltaTime;
            caughtPopupText.localScale = Vector3.one * (2 * Mathf.Sin(4 * timeInGuiPopup - (3.141f / 2)) + 2);
            if (!midPopupRan && timeInGuiPopup >= (3.141f / 4)) {
                playersInPlay[chaseeIndex].setAsChasee();
                playersInPlay[chaserIndex].evadedCapture();
                playersInPlay[chaseeIndex].setSprite(PlayerController.SpriteFrameType.PATTING);
                playersInPlay[chaserIndex].setSprite(PlayerController.SpriteFrameType.SITTING);
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
            stateChangedThisFrame = false;
            // Clear keys
            for (int i = 0; i < currentPlayerCount; i++)
            {
                chaseSceneKeys[i] = KeyCode.None;
                numberOfConsecutivePresses[i] = 0;
                Destroy(playerKeyPopups[i]);
            }
        }
        if (currentGameState == GameState.ESCAPE_ANIMATION)
        {
            timeInGuiPopup += Time.deltaTime;
            escapedPopupText.localScale = Vector3.one * (2 * Mathf.Sin(4 * timeInGuiPopup - (3.141f / 2)) + 2);
            if (!midPopupRan && timeInGuiPopup >= (3.141f / 4))
            {
                playersInPlay[chaseeIndex].angleAtHome = playersInPlay[chaserIndex].angleAtHome;
                playersInPlay[chaseeIndex].evadedCapture();
                playersInPlay[chaserIndex].setAsChasee();
                playersInPlay[chaserIndex].setSprite(PlayerController.SpriteFrameType.PATTING);
                playersInPlay[chaseeIndex].setSprite(PlayerController.SpriteFrameType.SITTING);
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
        if (currentGameState == GameState.RUN_ANIMATION && stateChangedThisFrame)
        {
            runPopupText.GameObject().SetActive(true);
            runPopupText.localScale = Vector3.zero;
            timeInGuiPopup = 0;
            midPopupRan = false;
            stateChangedThisFrame = false;
        }
        if (currentGameState == GameState.RUN_ANIMATION)
        {
            timeInGuiPopup += Time.deltaTime;
            runPopupText.localScale = Vector3.one * (2 * Mathf.Sin(4 * timeInGuiPopup - (3.141f / 2)) + 2);
            if (!midPopupRan && timeInGuiPopup >= (3.141f / 4))
            {
                playersInPlay[chaserIndex].angleAtHome = playersInPlay[chaserIndex].currentPlacementAngle;
                playersInPlay[chaseeIndex].setAsChasee();
                playersInPlay[chaserIndex].currentPlacementAngle = -0.3f;
                playersInPlay[chaserIndex].inChase = true;
                playersInPlay[chaseeIndex].inChase = true;

                playersInPlay[chaseeIndex].setSprite(PlayerController.SpriteFrameType.RUNNING);
                playersInPlay[chaserIndex].setSprite(PlayerController.SpriteFrameType.RUNNING);

                midPopupRan = true;
            }
            if (timeInGuiPopup >= (3.141f / 2))
            {
                setGameState(GameState.CHASING);
                runPopupText.GameObject().SetActive(false);

            }
        }
        
        if (currentGameState == GameState.CHASING)
        {
            // Perform chase set up if just happened
            if (stateChangedThisFrame)
            {

                assignNewKey(chaserIndex);
                assignNewKey(chaseeIndex);

                playersInPlay[chaserIndex].isChasee = false;
                playersInPlay[chaseeIndex].isChasee = true;
                playersInPlay[chaserIndex].inChase = true;
                playersInPlay[chaseeIndex].inChase = true;
                playersInPlay[chaseeIndex].angleAtHome %= (2 * 3.141f);
                playersInPlay[chaserIndex].angleAtHome %= (2 * 3.141f);

                stateChangedThisFrame = false;
            }

            // Chaser has caught chasee if they are overlapping
            if (playersInPlay[chaserIndex].currentPlacementAngle >= playersInPlay[chaseeIndex].currentPlacementAngle)
            {
                setGameState(GameState.CAUGHT_ANIMATION);
                // Wipe chase keys
                for (int i = 0; i < currentPlayerCount; i++)
                {
                    chaseSceneKeys[i] = KeyCode.None;
                    numberOfConsecutivePresses[i] = 0;
                    Destroy(playerKeyPopups[i]);
                }
            }

            camLookPosition = playersInPlay[chaseeIndex].transform.position;
        }
        Camera.main.transform.LookAt(camLookPosition);
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0, Camera.main.transform.rotation.eulerAngles.y, 0));

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(sceneBuildIndex: 0);
        }
    }

    private void LateUpdate()
    {
        //if (stateChangedThisFrame) stateChangedThisFrame = false;
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
                chaseeIndex = indexOfChasee;
                playerController.playerIndex = indexOfChasee;
                playerController.currentPlacementAngle = 0;
                newPlayer.transform.position = new Vector3(0, 0, chaseRadius);
                newPlayer.transform.LookAt(Camera.main.transform.position);
                playerController.setSprite(PlayerController.SpriteFrameType.PATTING);
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
