using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // My player index
    public int playerIndex;
    // Store game manager reference
    GameManager gameManager;

    double timeSinceLastSpeedChange;
    double timeSinceLastSpeedIncrease;
    [SerializeField] float chaseAngularSpeed;

    [SerializeField] private float debugOffset;
    public float currentPlacementAngle;

    public float angleAtHome;

    public bool inChase;
    public bool isChasee;

    private int goHomeDirection;

    void Start()
    {
        timeSinceLastSpeedIncrease = 0;
        timeSinceLastSpeedChange = 0;
        //currentPlacementAngle = -debugOffset;
        //angleAtHome = -debugOffset;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //// Chase set up
        //if (gameManager.currentGameState == GameManager.GameState.CHASING && gameManager.stateChangedThisFrame && inChase)
        //{
        //    angleAtHome = angleAtHome % (2 * 3.141f);
        //}
        // If currently in chase
        if (gameManager.currentGameState == GameManager.GameState.CHASING && currentPlacementAngle < angleAtHome + (6 * 3.141f) && inChase)
        {
            // Incrase both the time since last change and the time since last speed increase
            timeSinceLastSpeedChange += Time.deltaTime;
            timeSinceLastSpeedIncrease += Time.deltaTime;

            // If button pressed increase speed
            if (Input.GetKeyDown(gameManager.chaseSceneKeys[playerIndex]) && (chaseAngularSpeed < gameManager.chaseMaxAngSpeed))
            {
                chaseAngularSpeed += 0.1f;
                timeSinceLastSpeedChange = 0;
                timeSinceLastSpeedIncrease = 0;
                gameManager.countKeyPress(playerIndex);
            }
            // Exponential rate of decrease since last click. 
            if (timeSinceLastSpeedChange > 0.01 && (chaseAngularSpeed > gameManager.chaseMinAngSpeed))
            {
                chaseAngularSpeed -= (gameManager.chaseMinDeceleration - 1 + Mathf.Exp(gameManager.chaseDecelerationGradient * ((float)timeSinceLastSpeedIncrease - gameManager.chaseMinAngSpeed)));
                timeSinceLastSpeedChange = 0;
            }

            // Apply speed
            currentPlacementAngle += chaseAngularSpeed * Time.deltaTime;

            // Change player position and rotation based on angle
            Vector3 oldPos = transform.position;
            transform.position = new Vector3((gameManager.chaseRadius * Mathf.Sin(currentPlacementAngle)),
                                            gameManager.chaseBobAmp * Mathf.Sin(currentPlacementAngle / gameManager.chaseBobFreq) + gameManager.chaseBobAmp,
                                            (gameManager.chaseRadius * Mathf.Cos(currentPlacementAngle)));
            transform.LookAt(Camera.main.transform.position);
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));

            if (currentPlacementAngle > angleAtHome + (6 * 3.141f)) {
                gameManager.setGameState(GameManager.GameState.ESCAPE_ANIMATION);
            }
        }
        //if (gameManager.currentGameState == GameManager.GameState.CAUGHT_ANIMATION && inChase && gameManager.stateChangedThisFrame)
        //{
        //    currentPlacementAngle = currentPlacementAngle % (2 * 3.141f);
        //    if (isChasee) goHomeDirection = (currentPlacementAngle < angleAtHome) ? 1 : -1;
        //}
        //if (gameManager.currentGameState == GameManager.GameState.CAUGHT_ANIMATION && inChase) {

        //    if (isChasee) {
        //        if (!(currentPlacementAngle > 2 * 3.141f)) {
        //            currentPlacementAngle += 0.3f * Time.deltaTime;
        //            transform.position = new Vector3((gameManager.chaseRadius * Mathf.Sin(currentPlacementAngle)),
        //                                    gameManager.chaseBobAmp * Mathf.Sin(currentPlacementAngle / gameManager.chaseBobFreq) + gameManager.chaseBobAmp,
        //                                    (gameManager.chaseRadius * Mathf.Cos(currentPlacementAngle)));
        //            transform.LookAt(Camera.main.transform.position);
        //            transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
        //        }
        //    }
        //    else
        //    { // IF LOGIC WRONG
        //        if (!((currentPlacementAngle < angleAtHome && goHomeDirection == -1) || (currentPlacementAngle > angleAtHome && goHomeDirection == 1)))
        //        {
        //            currentPlacementAngle += 0.3f * Time.deltaTime * goHomeDirection;
        //            transform.position = new Vector3((gameManager.chaseRadius * Mathf.Sin(currentPlacementAngle)),
        //                                    gameManager.chaseBobAmp * Mathf.Sin(currentPlacementAngle / gameManager.chaseBobFreq) + gameManager.chaseBobAmp,
        //                                    (gameManager.chaseRadius * Mathf.Cos(currentPlacementAngle)));
        //            transform.LookAt(Camera.main.transform.position);
        //            transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
        //        }
        //    }
        //}

    }

    public void setAsChasee() {
        currentPlacementAngle = 0;
        timeSinceLastSpeedIncrease = 0;
        timeSinceLastSpeedChange = 0;
        chaseAngularSpeed = gameManager.chaseMinAngSpeed;

        isChasee = true;
        inChase = false;

        transform.position = new Vector3((gameManager.chaseRadius * Mathf.Sin(currentPlacementAngle)),
                                        0,
                                        (gameManager.chaseRadius * Mathf.Cos(currentPlacementAngle)));
        transform.LookAt(Camera.main.transform.position);
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
    }

    public void evadedCapture() {
        isChasee = false;
        inChase = false;

        currentPlacementAngle = angleAtHome;
        timeSinceLastSpeedIncrease = 0;
        timeSinceLastSpeedChange = 0;
        chaseAngularSpeed = gameManager.chaseMinAngSpeed;
        transform.position = new Vector3((gameManager.sitRadius * Mathf.Sin(currentPlacementAngle)),
                                        0,
                                        (gameManager.sitRadius * Mathf.Cos(currentPlacementAngle)));
        transform.LookAt(Camera.main.transform.position);
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
    }

    [SerializeField] Sprite[] spriteFrames;
    public enum SpriteFrameType { WALKING, RUNNING, STANDING, SITTING, PATTED};

    public void setSprite(SpriteFrameType newSprite) { 
        GetComponent<SpriteRenderer>().sprite = spriteFrames[(int)newSprite];
    }

}
