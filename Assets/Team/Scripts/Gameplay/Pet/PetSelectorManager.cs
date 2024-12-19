using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class PetSelectorManager : MonoBehaviour
{
    [Header("Pet Display")]
    public Transform PetDisplayPoint;
    public List<GameObject> PetPrefabs;

    [Header("Name Input")]
    public TMP_InputField NameInputField;

    [Header("Stat Adjustment UI")]
    [Space(5)]
    [Header("Strength")]
    public Slider StrengthBar;
    public TMP_Text StrengthText;
    public Button StrengthPlusButton;
    public Button StrengthMinusButton;

    [Space(5)]
    [Header("Defense")]
    public Slider DefenseBar;
    public TMP_Text DefenseText;
    public Button DefensePlusButton;
    public Button DefenseMinusButton;

    [Space(5)]
    [Header("Speed")]
    public Slider SpeedBar;
    public TMP_Text SpeedText;
    public Button SpeedPlusButton;
    public Button SpeedMinusButton;

    [Space(5)]
    [Header("Health")]
    public Slider HealthBar;
    public TMP_Text HealthText;
    public Button HealthPlusButton;
    public Button HealthMinusButton;

    [Space(10)]
    [Header("Available Points")]
    public TMP_Text AvailablePointsText;
    public int MaxBonusPoints = 5;
    public int StatBonus = 3;
    private int m_AvailablePoints;

    [Header("Navigation Buttons")]
    public Button LeftArrowButton;
    public Button RightArrowButton;
    public Button ConfirmButton;

    private int m_CurrentPetIndex = 0;
    private GameObject m_CurrentPetInstance;
    private PetStats[] m_PetStats;

    private void Start()
    {
        InitializePetStats();
        m_AvailablePoints = MaxBonusPoints;
        DisplayPet(m_CurrentPetIndex);
        UpdateStatBars();
        SetupButtonListeners();
    }

    private void InitializePetStats()
    {
        m_PetStats = new PetStats[]
        {
            new("Bear", 1, 1, 1, 1, "Defense"),
            new("Dragon", 1, 1, 1, 1, "Strength"),
            new("Bunny", 1, 1, 1, 1, "Speed")
        };

        foreach (var pet in m_PetStats)
        {
            ApplyBonusStat(pet);
        }
    }

    private void DisplayPet(int index)
    {
        if (m_CurrentPetInstance != null)
            Destroy(m_CurrentPetInstance);

        m_CurrentPetInstance = Instantiate(PetPrefabs[index], PetDisplayPoint.position, Quaternion.identity);

        m_CurrentPetInstance.transform.SetParent(PetDisplayPoint);
        m_CurrentPetInstance.transform.localPosition = Vector3.zero;
        m_CurrentPetInstance.transform.localRotation = Quaternion.Euler(0, 180, 0);

        NavMeshAgent navMeshAgent = m_CurrentPetInstance.GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
            navMeshAgent.enabled = false;

        PetController petController = m_CurrentPetInstance.GetComponent<PetController>();
        if (petController != null)
            petController.enabled = false;

        Pet petScript = m_CurrentPetInstance.GetComponent<Pet>();
        if (petScript != null)
            petScript.enabled = false;

        UpdateStatBars();
    }

    private void UpdateStatBars()
    {
        PetStats selectedPet = m_PetStats[m_CurrentPetIndex];

        StrengthBar.value = selectedPet.Strength;
        DefenseBar.value = selectedPet.Defense;
        SpeedBar.value = selectedPet.Speed;
        HealthBar.value = selectedPet.Health;

        StrengthText.text = selectedPet.Strength.ToString();
        DefenseText.text = selectedPet.Defense.ToString();
        SpeedText.text = selectedPet.Speed.ToString();
        HealthText.text = selectedPet.Health.ToString();

        AvailablePointsText.text = $"Available Points: {m_AvailablePoints}";
    }

    private void SetupButtonListeners()
    {
        LeftArrowButton.onClick.AddListener(() => CyclePet(-1));
        RightArrowButton.onClick.AddListener(() => CyclePet(1));
        ConfirmButton.onClick.AddListener(ConfirmSelection);

        StrengthPlusButton.onClick.AddListener(() => AllocateStat("Strength"));
        StrengthMinusButton.onClick.AddListener(() => DeallocateStat("Strength"));

        DefensePlusButton.onClick.AddListener(() => AllocateStat("Defense"));
        DefenseMinusButton.onClick.AddListener(() => DeallocateStat("Defense"));

        SpeedPlusButton.onClick.AddListener(() => AllocateStat("Speed"));
        SpeedMinusButton.onClick.AddListener(() => DeallocateStat("Speed"));

        HealthPlusButton.onClick.AddListener(() => AllocateStat("Health"));
        HealthMinusButton.onClick.AddListener(() => DeallocateStat("Health"));
    }

    private void CyclePet(int direction)
    {
        m_CurrentPetIndex = (m_CurrentPetIndex + direction) % PetPrefabs.Count;
        if (m_CurrentPetIndex < 0)
            m_CurrentPetIndex = PetPrefabs.Count - 1;

        AudioSource arrowButton = RightArrowButton.GetComponent<AudioSource>();

        arrowButton.Play();

        DisplayPet(m_CurrentPetIndex);
    }

    private void AllocateStat(string statType)
    {
        AudioSource allocateSound = StrengthPlusButton.GetComponent<AudioSource>();

        allocateSound.Play();

        if (m_AvailablePoints <= 0) return;

        PetStats selectedPet = m_PetStats[m_CurrentPetIndex];
        switch (statType)
        {
            case "Strength":
                selectedPet.Strength++;
                break;
            case "Defense":
                selectedPet.Defense++;
                break;
            case "Speed":
                selectedPet.Speed++;
                break;
            case "Health":
                selectedPet.Health++;
                break;
        }

        m_AvailablePoints--;
        UpdateStatBars();
    }

    private void DeallocateStat(string statType)
    {
        AudioSource deallocateSound = StrengthMinusButton.GetComponent<AudioSource>();

        deallocateSound.Play();

        PetStats selectedPet = m_PetStats[m_CurrentPetIndex];
        switch (statType)
        {
            case "Strength" when selectedPet.Strength > 1:
                selectedPet.Strength--;
                m_AvailablePoints++;
                break;
            case "Defense" when selectedPet.Defense > 1:
                selectedPet.Defense--;
                m_AvailablePoints++;
                break;
            case "Speed" when selectedPet.Speed > 1:
                selectedPet.Speed--;
                m_AvailablePoints++;
                break;
            case "Health" when selectedPet.Health > 1:
                selectedPet.Health--;
                m_AvailablePoints++;
                break;
        }

        UpdateStatBars();
    }

    private void ApplyBonusStat(PetStats petStats)
    {
        switch (petStats.BonusStat)
        {
            case "Strength":
                petStats.Strength += StatBonus;
                break;
            case "Defense":
                petStats.Defense += StatBonus;
                break;
            case "Speed":
                petStats.Speed += StatBonus;
                break;
            case "Health":
                petStats.Health += StatBonus;
                break;
        }
    }

    private void ConfirmSelection()
    {
        AudioSource confirmSound = ConfirmButton.GetComponent<AudioSource>();

        float loadDelay = confirmSound.clip.length;

        confirmSound.Play();

        PetStats selectedPetStats = m_PetStats[m_CurrentPetIndex];
        string chosenName = NameInputField.text;
        if (string.IsNullOrEmpty(chosenName))
        {
            chosenName = selectedPetStats.PetName;
        }

        GameObject selectedPetPrefab = PetPrefabs[m_CurrentPetIndex];

        if (SelectedPetManager.Instance == null)
        {
            Debug.LogError("SelectedPetManager.Instance is null! Ensure the SelectedPetManager exists in the scene.");
            return;
        }

        if (selectedPetPrefab == null)
        {
            Debug.LogError("selectedPetPrefab is null! Ensure a prefab is selected.");
            return;
        }

        Pet selectedPet = selectedPetPrefab.GetComponent<Pet>();
        if (selectedPet == null)
        {
            Debug.LogError("The selected prefab does not have a Pet component attached!");
            return;
        }

        SelectedPetManager.Instance.SetSelectedPet(selectedPet, chosenName, selectedPetStats);

        Debug.Log($"Selected Pet: {chosenName}, Stats: Str-{selectedPetStats.Strength}, Def-{selectedPetStats.Defense}, Spd-{selectedPetStats.Speed}, Hp-{selectedPetStats.Health}");
        


        Invoke(nameof(LoadFarmScene), loadDelay);
    }

    private void LoadFarmScene()
    {
        SceneManager.LoadScene("FarmScene");
    }
}
