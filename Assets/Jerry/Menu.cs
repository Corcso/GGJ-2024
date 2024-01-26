using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayButton()
    {
       // SceneManager.LoadScene(1);

        //Uncomment this to load game scene
    }

    public void OnHowToPlayButton()
    {
        // SceneManager.LoadScene(1);

        //Uncomment this to load how to play scene
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
