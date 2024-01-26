using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerControllerChase : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float speed;
    [SerializeField] private float dist;
    [SerializeField] private float bobAmp;
    [SerializeField] private float bobFreq;
    [SerializeField] private float debugOffset;
    [SerializeField] private bool isCamLocked;
    private float currentPlacementAngle; 
    private float angleAtHome;
    void Start()
    {
        currentPlacementAngle = 0 - debugOffset;
    }

    // Update is called once per frame
    void Update()
    {
        currentPlacementAngle += speed * Time.deltaTime;

        Vector3 oldPos = transform.position;
        transform.position = new Vector3((dist * Mathf.Sin(currentPlacementAngle)),
                                        bobAmp * Mathf.Sin(currentPlacementAngle / bobFreq) + bobAmp,
                                        (dist * Mathf.Cos(currentPlacementAngle)));
        transform.LookAt(Camera.main.transform.position);   

        if(isCamLocked) {
            Camera.main.transform.LookAt(transform.position);
        }
    }


}
