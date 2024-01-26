using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ButtonMashScript : MonoBehaviour
{
    // Start is called before the first frame update

    bool clicked = false;
    double timeSinceLastSpeedChange;
    [SerializeField] float speed;
    float maxSpeed = 2.0f;
    float minSpeed = 0.2f;

    [SerializeField] private float dist;
    [SerializeField] private float bobAmp;
    [SerializeField] private float bobFreq;
    [SerializeField] private float debugOffset;
    [SerializeField] private bool isCamLocked;
    private float currentPlacementAngle;
    private float angleAtHome;

    void Start()
    {
        timeSinceLastSpeedChange = 0;

    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastSpeedChange += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && (speed < maxSpeed)) {
            speed += 0.1f;
            timeSinceLastSpeedChange = 0;
        }
        // Exponential rate of decrease since last click. 
        if (timeSinceLastSpeedChange > 0.01 && (speed > minSpeed)) {
            speed -= 0.002f;
            timeSinceLastSpeedChange = 0;
        }

        currentPlacementAngle += speed * Time.deltaTime;

        Vector3 oldPos = transform.position;
        transform.position = new Vector3((dist * Mathf.Sin(currentPlacementAngle)),
                                        bobAmp * Mathf.Sin(currentPlacementAngle / bobFreq) + bobAmp,
                                        (dist * Mathf.Cos(currentPlacementAngle)));
        transform.LookAt(Camera.main.transform.position);

        if (isCamLocked)
        {
            Camera.main.transform.LookAt(transform.position);
        }
    }
}
