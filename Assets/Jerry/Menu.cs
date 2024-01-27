using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI txt;
    [SerializeField] public int noOfPlayers = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void display(TextMeshProUGUI selectModifier)
    {


    }

    public void lessPlayers()
    {
        if (noOfPlayers >= 4 && noOfPlayers <= 6)
        {
            noOfPlayers--;
        }

        txt.text = "Amount of players: " + noOfPlayers.ToString();
       
    }

    public void morePlayers()
    {

        if (noOfPlayers >= 3 && noOfPlayers <= 5)
        {
            noOfPlayers++;
        }
        txt.text = "Amount of players: " + noOfPlayers.ToString();


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
