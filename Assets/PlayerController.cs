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

    private float angleAtHome;

    private bool inChase;
    [SerializeField] private bool isCamLocked;

    void Start()
    {
        timeSinceLastSpeedIncrease = 0;
        timeSinceLastSpeedChange = 0;
        currentPlacementAngle = -debugOffset;
        angleAtHome = 0;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // If currently in chase
        if (gameManager.currentGameState == GameManager.GameState.CHASING && currentPlacementAngle < angleAtHome + (200 * 3.141f))
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

            // If the player is selected as the camera target move the cam to them, maybe do this in game manager
            if (isCamLocked)
            {
                Camera.main.transform.LookAt(transform.position);
                Camera.main.transform.rotation = Quaternion.Euler(new Vector3(0, Camera.main.transform.rotation.eulerAngles.y, 0));
            }
        }
    }
}
