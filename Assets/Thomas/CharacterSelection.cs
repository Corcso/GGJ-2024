using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{

    GameObject[] ducks;
    int noOfPlayers;
    [SerializeField] int playerSelectingNow = 0;
    globalSettingsManager settingsManager;

    // Start is called before the first frame update
    void Start()
    {
        settingsManager = GameObject.Find("GLOBAL_SETTINGS").GetComponent<globalSettingsManager>();
        noOfPlayers = settingsManager.currentPlayerCount;
        ducks = new GameObject[noOfPlayers];
        settingsManager.playerColors = new Color[noOfPlayers];


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buttonPressDucks(GameObject prefabToAdd)
    {

        if (playerSelectingNow < noOfPlayers)
        {

            ducks[playerSelectingNow] = prefabToAdd;


        }


    }

    public void playerPressed(TextMeshProUGUI selectedNotifier)
    {

        if (playerSelectingNow < noOfPlayers)
        {

            playerSelectingNow++;
            selectedNotifier.text = playerSelectingNow.ToString();
            settingsManager.playerColors[playerSelectingNow - 1] = selectedNotifier.color;
            selectedNotifier.transform.parent.GetComponent<Button>().interactable = false;
        }


    }

    public void buttonPressback()
    {
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }

    public void buttonPressPlay()
    {
        settingsManager.playersToSpawnPrefabs = ducks;
        SceneManager.LoadScene(sceneBuildIndex: 2);
    }
}
