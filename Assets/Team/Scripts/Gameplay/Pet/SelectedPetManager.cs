using UnityEngine;

public class SelectedPetManager : MonoBehaviour
{
    public static SelectedPetManager Instance { get; private set; }
    private Pet m_SelectedPetPrefab;
    private string m_SelectedPetName;
    private PetStats m_SelectedPetStats;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSelectedPet(Pet petPrefab, string name, PetStats stats)
    {
        m_SelectedPetPrefab = petPrefab;
        m_SelectedPetName = name;
        m_SelectedPetStats = stats;
    }

    public Pet GetSelectedPet()
    {
        return m_SelectedPetPrefab;
    }

    public string GetSelectedPetName()
    {
        return m_SelectedPetName;
    }

    public PetStats GetSelectedPetStats()
    {
        return m_SelectedPetStats;
    }
}
