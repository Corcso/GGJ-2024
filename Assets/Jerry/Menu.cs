using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI txt;
    [SerializeField] public int noOfPlayers = 3;
    [SerializeField] GameObject tutorialBox;
    globalSettingsManager settingsManager;

    // Start is called before the first frame update
    void Start()
    {
        settingsManager = GameObject.Find("GLOBAL_SETTINGS").GetComponent<globalSettingsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void lessPlayers()
    {
        if (noOfPlayers >= 4 && noOfPlayers <= 6)
        {
            noOfPlayers--;
        }

        txt.text = noOfPlayers.ToString();
       
    }

    public void playButton()
    {

        settingsManager.currentPlayerCount = noOfPlayers;
        SceneManager.LoadScene(sceneBuildIndex: 1);

    }

    public void morePlayers()
    {

        if (noOfPlayers >= 3 && noOfPlayers <= 5)
        {
            noOfPlayers++;
        }
        txt.text = noOfPlayers.ToString();


    }

    public void OnHowToPlayButton()
    {
        // SceneManager.LoadScene(1);
        tutorialBox.SetActive(true);
        //Uncomment this to load how to play scene
    }

    public void CloseHowToPlayButton()
    {
        // SceneManager.LoadScene(1);
        tutorialBox.SetActive(false);
        //Uncomment this to load how to play scene
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
