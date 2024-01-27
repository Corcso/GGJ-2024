using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{

    GameObject[] ducks;
    int noOfPlayers = 3;
    [SerializeField] int playerSelectingNow = 0;
    [SerializeField] TextMeshProUGUI txt;

    // Start is called before the first frame update
    void Start()
    {
        ducks = new GameObject[noOfPlayers];
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
            selectedNotifier.transform.parent.GetComponent<Button>().interactable = false;

            // Do some gui stuff to say who is selecting
            txt.text = "Player " + playerSelectingNow.ToString() + " has chosen!";

        }
        else
        {

            txt.text = "All players have selected a character!";

        }


    }

    public void buttonPressback()
    {

    }

    public void buttonPressPlay()
    {

    }
}
