using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ButtonMashScript : MonoBehaviour
{
    // Start is called before the first frame update

    bool clicked = false;
    double timeWithoutClicking;
    [SerializeField] float speed = 5.0f;
    float maxSpeed = 10.0f;
    float minSpeed = 1.0f;

    void Start()
    {
        timeWithoutClicking = Time.timeAsDouble * 1000;

    }

    // Update is called once per frame
    void Update()
    {
       
        double currentTime = Time.timeAsDouble * 1000;

        if (Input.GetMouseButtonDown(0) && (speed < maxSpeed)) {
            speed += 0.5f;
            timeWithoutClicking = Time.timeAsDouble * 1000;
        }

        if (!Input.GetMouseButtonDown (0) && (currentTime - timeWithoutClicking > 30) && (speed > minSpeed)) {
            speed -= 0.1f;
            timeWithoutClicking = Time.timeAsDouble * 1000;
        }
    }
}
