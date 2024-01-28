using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalSettingsManager : MonoBehaviour
{
    public GameObject[] playersToSpawnPrefabs;
    public int currentPlayerCount;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
