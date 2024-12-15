using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FarmSceneManager : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "FarmScene")
        {
            PlayerPetManager playerPetManager = FindObjectOfType<PlayerPetManager>();
            if (playerPetManager != null)
            {
                playerPetManager.SpawnAndAssignSelectedPet();
            }
        }
    }
}
