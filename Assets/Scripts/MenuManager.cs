using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MenuManager : MonoBehaviour
{
    public GameObject farmMenu;
    public GameObject petMenu;
    public GameObject TournamentMenu;
    public GameObject FpsController;
    public Camera playerCamera;
    public float detectionRange = 5f;
    public TMP_Text ActionText;

    private bool m_IsMenuOpen = false;
    private string m_CurrentTag;
    private FirstPersonController m_FpsControllerScript;


    private void Start()
    {
        farmMenu.SetActive(false);
        petMenu.SetActive(false);
        TournamentMenu.SetActive(false);
        ActionText.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_FpsControllerScript = FpsController.GetComponent<FirstPersonController>();
    }

    private void Update()
    {
        if (!m_IsMenuOpen)
            DetectObject();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (m_IsMenuOpen)
                CloseMenu();
            else
                OpenMenuForCurrentTag();
        }
    }

    private void DetectObject()
    {
        var ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionRange))
        {
            var tag = hit.collider.tag;

            if (tag is "Farm" or "Pet" or "Tournament")
            {
                m_CurrentTag = tag;
                ActionText.text = "Press E to interact with " + tag;
                ActionText.gameObject.SetActive(true);
            }
        }
        else
        {
            ActionText.gameObject.SetActive(false);
            m_CurrentTag = "";
            ActionText.text = "";
        }
    }

    private void OpenMenuForCurrentTag()
    {
        if (m_CurrentTag == "Farm")
        {
            ShowFarmMenu();
        }
        else if (m_CurrentTag == "Pet")
        {
            ShowPetMenu();
        }
        else if (m_CurrentTag == "Tournament")
        {
            ShowTournamentMenu();
        }

        if (!string.IsNullOrEmpty(m_CurrentTag))
        {
            m_IsMenuOpen = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            m_FpsControllerScript.enabled = false;
        }
    }


    void ShowFarmMenu()
    {
        farmMenu.SetActive(true);
        ActionText.gameObject.SetActive(false);
        petMenu.SetActive(false);
        TournamentMenu.SetActive(false);
    }

    void ShowPetMenu()
    {
        farmMenu.SetActive(false);
        ActionText.gameObject.SetActive(false);
        petMenu.SetActive(true);
        TournamentMenu.SetActive(false);
    }

    void ShowTournamentMenu()
    {
        farmMenu.SetActive(false);
        ActionText.gameObject.SetActive(false);
        petMenu.SetActive(false);
        TournamentMenu.SetActive(true);
    }

    void CloseMenu()
    {
        HideAllMenus();
        m_IsMenuOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_FpsControllerScript.enabled = true;
    }

    void HideAllMenus()
    {
        farmMenu.SetActive(false);
        petMenu.SetActive(false);
        TournamentMenu.SetActive(false);
    }
}