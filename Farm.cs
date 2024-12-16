using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Group3d.Notifications;

public class Farm : MonoBehaviour, IInteractable
{
    [Header("Crop Settings")]
    [SerializeField] private Crop m_CurrentCrop;
    [SerializeField] private int m_CurrentCapacity = 0;
    [SerializeField] private int m_MaxCapacity = 10;
    [SerializeField] private int m_UpgradeCost = 20;

    [Header("Crop Prefabs and Parents")]
    [SerializeField] private GameObject cropModelPrefab; // Crop model to instantiate
    [SerializeField] private Transform cropParent;       // Parent for all crops

    [Header("UI and Audio Settings")]
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private AudioClip upgradeSound;
    [SerializeField] private AudioClip harvestSound;

    [Header("Spawn Point Grid Settings")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>(); // List to hold spawn points
    [SerializeField] private int rows = 3;    // Number of rows in the grid
    [SerializeField] private int columns = 3; // Number of columns in the grid
    [SerializeField] private float spacing = 2.0f; // Spacing between points

    [Header("Available Crops")]
    public List<Crop> ListOfCrops;

    private PlayerFoodManager m_PlayerFoodManager;
    private UIManager m_UiManager;

    public Crop CurrentCrop { get => m_CurrentCrop; set => m_CurrentCrop = value; }
    public int CurrentCapacity { get => m_CurrentCapacity; set => m_CurrentCapacity = value; }
    public int MaxCapacity { get => m_MaxCapacity; set => m_MaxCapacity = value; }
    public int UpgradeCost { get => m_UpgradeCost; set => m_UpgradeCost = value; }

    private void Start()
    {
        if (CurrentCrop == null)
        {
            Debug.LogError("No crop assigned to the farm!");
        }

        m_PlayerFoodManager = FindObjectOfType<PlayerFoodManager>();
        m_UiManager = FindObjectOfType<UIManager>();

        UpdateStatusText();

        GenerateSpawnPoints(); // Generate spawn points
        StartCoroutine(PassiveFoodGain());
    }

    private void GenerateSpawnPoints()
    {
        spawnPoints.Clear();

        // Get the center position of the farm
        Vector3 startPosition = transform.position;

        // Calculate offsets to center the grid
        float xOffset = (columns - 1) * spacing * 0.5f;
        float zOffset = (rows - 1) * spacing * 0.5f;

        // Generate the grid of spawn points
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 spawnPosition = new Vector3(
                    startPosition.x - xOffset + (j * spacing),
                    startPosition.y,
                    startPosition.z - zOffset + (i * spacing)
                );

                // Create an empty GameObject as a spawn point
                GameObject spawnPoint = new GameObject("SpawnPoint_" + (i * columns + j));
                spawnPoint.transform.position = spawnPosition;
                spawnPoint.transform.SetParent(cropParent);

                // Add the spawn point's transform to the list
                spawnPoints.Add(spawnPoint.transform);
            }
        }

        Debug.Log("Spawn points successfully generated: " + spawnPoints.Count);
    }

    public void OnLookAt()
    {
        Debug.Log("Looking at the farm growing: " + CurrentCrop.CropName);

        if (m_UiManager != null)
        {
            m_UiManager.ShowInteractableInfo(
                "Farm (" + CurrentCrop.CropName.ToString() + ")",
                "Current Capacity: " + CurrentCapacity + "/" + MaxCapacity + "\n" +
                            "Upgrade Cost: " + UpgradeCost,
                "Press 'E' to harvest",
                "Press 'Q' to upgrade",
                "Press 'R' to change crop"
            );
        }
    }

    public void OnInteract()
    {
        HarvestCrop();
    }

    public void OnSecondaryInteract()
    {
        UpgradeFarmCapacity();
    }

    public void OnTertiaryInteract()
    {
        ChangeCrop();
    }

    private void HarvestCrop()
    {
        if (CurrentCapacity > 0)
        {
            m_PlayerFoodManager.AddCrop(CurrentCrop, CurrentCapacity);
            Debug.Log("Harvested " + CurrentCapacity + " units of " + CurrentCrop.CropName);
            CurrentCapacity = 0;

            PlaySound(harvestSound);

            UpdateStatusText();

            StartCoroutine(PassiveFoodGain());
        }
        else
        {
            Debug.Log("The farm is empty, nothing to harvest.");
        }
    }

    private IEnumerator PassiveFoodGain()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.5f);
            CurrentCapacity += 1;
            Debug.Log("Total Amount of food now: " + CurrentCapacity);

            UpdateStatusText();

            if (CurrentCapacity >= MaxCapacity)
            {
                Debug.Log("Max capacity reached, stopping passive food gain.");
                Notifications.Send("Farm (" + m_CurrentCrop.CropName + ") full!", NotificationType.Error, null);
                yield break;
            }

            SpawnCrops(); // Spawn crops when food is gained
        }
    }

    private void UpgradeFarmCapacity()
    {
        if (m_PlayerFoodManager.HasCrop(CurrentCrop, UpgradeCost))
        {
            m_PlayerFoodManager.UseCrop(CurrentCrop, UpgradeCost);
            MaxCapacity += 5;
            UpgradeCost *= 2;
            Debug.Log("Farm upgraded! New max capacity: " + MaxCapacity);

            PlaySound(upgradeSound);
        }
        else
        {
            Debug.Log("Not enough of " + CurrentCrop.CropName + " to upgrade the farm. Upgrade cost: " + UpgradeCost);
        }
    }

    private void ChangeCrop()
    {
        HarvestCrop();

        if (ListOfCrops.Count > 0)
        {
            int currentIndex = ListOfCrops.IndexOf(m_CurrentCrop);
            int nextIndex = (currentIndex + 1) % ListOfCrops.Count;
            m_CurrentCrop = ListOfCrops[nextIndex];
            Debug.Log("Crop changed to: " + m_CurrentCrop.CropName);

            UpdateStatusText();
        }
        else
        {
            Debug.Log("No crops available to change.");
        }
    }

    private void UpdateStatusText()
    {
        _statusText.text = m_CurrentCrop.CropName + ": " + CurrentCapacity + "/" + MaxCapacity;
    }

    private void PlaySound(AudioClip clip)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void SpawnCrops()
    {
        // Ensure cropModelPrefab and spawnPoints are valid
        if (cropModelPrefab == null)
        {
            Debug.LogError("Crop Model Prefab is not assigned.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points found to spawn crops.");
            return;
        }

        // Clear any previous crops (not spawn points)
        foreach (Transform child in cropParent)
        {
            if (!spawnPoints.Contains(child))  // Only destroy non-spawn-point children
            {
                Destroy(child.gameObject);
            }
        }

        // Spawn crops up to the current capacity
        for (int i = 0; i < Mathf.Min(CurrentCapacity, spawnPoints.Count); i++)  // Using the property CurrentCapacity
        {
            Transform spawnPoint = spawnPoints[i];
            if (spawnPoint != null)
            {
                // Spawn a new crop at the spawn point position
                Instantiate(cropModelPrefab, spawnPoint.position, Quaternion.identity, cropParent);
            }
        }

        Debug.Log($"Spawned {Mathf.Min(CurrentCapacity, spawnPoints.Count)} crops.");
    }
}
