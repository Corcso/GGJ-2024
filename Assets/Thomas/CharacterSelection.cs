using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{

    GameObject[] ducks;
    int noOfPlayers = 0;

    // Start is called before the first frame update
    void Start()
    {
        ducks = new GameObject[noOfPlayers];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void buttonPressExample(GameObject prefabToAdd)
    {
        Debug.Log("pressed");

        for (int i = 0; i < noOfPlayers; i++)
        {

            ducks[i].AddComponent<CharacterSelection>();

        }

        
    }
}
