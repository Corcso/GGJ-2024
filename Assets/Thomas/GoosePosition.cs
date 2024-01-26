using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoosePosition : MonoBehaviour
{

    float noOfPlayers = 4;
    float duckPosX = 0;
    float duckPosZ = 0;
    float rotationAmount = 0;
    float rotationDegrees = 0;
    float distanceFromCam = 4;

    // Start is called before the first frame update
    void Start()
    {
        rotationAmount = 360 / noOfPlayers;
        Vector3 finalPos = Camera.main.transform.position + Camera.main.transform.forward * distanceFromCam;
        transform.position = finalPos;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.L))
        {

            rotationDegrees += rotationAmount;

            Camera.main.transform.rotation = Quaternion.Euler(0, rotationDegrees, 0);

            Vector3 finalPos = Camera.main.transform.position + Camera.main.transform.forward * distanceFromCam;
            transform.position = finalPos;

        }

    }
}
