using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPos : MonoBehaviour
{

    float noOfPlayers = 3;
    float playerPos = 0;
    float rotationDegrees = 0;
    float distanceFromCam = 3;

    // Start is called before the first frame update
    void Start()
    {
        playerPos = 360 / noOfPlayers;
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < noOfPlayers; i++)
        {

            rotationDegrees += playerPos;

            Vector3 finalPos = Camera.main.transform.position + Camera.main.transform.forward * distanceFromCam;
            transform.position = finalPos;
            transform.rotation = Quaternion.Euler(0, rotationDegrees, 0);

        }
        

    }
}
