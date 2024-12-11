using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartingAnimalSelector : MonoBehaviour
{   
    
    public string sceneToLoad;

    public string monsterName;
    private GameObject objectToSpawn;

    private Vector3 spawnPosition;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    public void OnButtonClicked(GameObject objectToSpawnForButton)
    {
        Debug.Log("Button clicked! Loading scene: " + sceneToLoad);

        objectToSpawn = objectToSpawnForButton;

        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (monsterName == "Bear")
        {
            spawnPosition = new Vector3(16, 1, 5);
        }
        else if (monsterName == "Dragon")
        {
            spawnPosition = new Vector3(16, 1, 5);
        }
        else if (monsterName == "Bunny")
        {
            spawnPosition = new Vector3(16, 0, 5);
        }
       Debug.Log("Scene loaded! Spawning object");
        if (objectToSpawn != null)
        {
            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
            Debug.Log("Object spawned!");
        }
        else
        {
            Debug.LogError("No object to spawn!");
        }

    }

}
