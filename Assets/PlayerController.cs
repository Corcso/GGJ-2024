using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // Store game manager reference
    GameManager gameManager;

    double timeSinceLastSpeedChange;
    [SerializeField] float chaseAngularSpeed;

    [SerializeField] private float debugOffset;
    [SerializeField] private float currentPlacementAngle;

    private float angleAtHome;

    private bool inChase;
    [SerializeField] private bool isCamLocked;

    void Start()
    {
        timeSinceLastSpeedChange = 0;
        angleAtHome = 0;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.currentGameState == GameManager.GameState.CHASING && currentPlacementAngle < angleAtHome + (2 * 3.141f))
        {
            timeSinceLastSpeedChange += Time.deltaTime;

            if (Input.GetMouseButtonDown(0) && (chaseAngularSpeed < gameManager.chaseMaxAngSpeed))
            {
                chaseAngularSpeed += 0.1f;
                timeSinceLastSpeedChange = 0;
            }
            // Exponential rate of decrease since last click. 
            if (timeSinceLastSpeedChange > 0.01 && (chaseAngularSpeed > gameManager.chaseMinAngSpeed))
            {
                chaseAngularSpeed -= 0.002f;
                timeSinceLastSpeedChange = 0;
            }

            currentPlacementAngle += chaseAngularSpeed * Time.deltaTime;

            Vector3 oldPos = transform.position;
            transform.position = new Vector3((gameManager.chaseRadius * Mathf.Sin(currentPlacementAngle)),
                                            gameManager.chaseBobAmp * Mathf.Sin(currentPlacementAngle / gameManager.chaseBobFreq) + gameManager.chaseBobAmp,
                                            (gameManager.chaseRadius * Mathf.Cos(currentPlacementAngle)));
            transform.LookAt(Camera.main.transform.position);

            if (isCamLocked)
            {
                Camera.main.transform.LookAt(transform.position);
            }
        }

    }
}
