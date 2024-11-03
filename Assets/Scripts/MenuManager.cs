using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject farmMenu;
    public GameObject petMenu;
    public Camera playerCamera;
    public float detectionRange = 5f;

    private void Start()
    {
        farmMenu.SetActive(false);
        petMenu.SetActive(false);
    }

    private void Update()
    {
        DetectObject();
    }

    void DetectObject()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Farm"))
            {
                ShowFarmMenu();
            }
            else if (hit.collider.CompareTag("Pet")){
                ShowPetMenu();
            }
            else
            {
                HideAllMenus();
            }
        }
        else
        {
            HideAllMenus();
        }
    }
    void ShowFarmMenu()
    {
        farmMenu.SetActive(true);
        petMenu.SetActive(false);
    }
    void ShowPetMenu()
    {
        farmMenu.SetActive(false);
        petMenu.SetActive(true);
    }
    void HideAllMenus()
    {
        farmMenu.SetActive(false);
        petMenu.SetActive(false);
    }
}
