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

    [Header("UI and Audio Settings")]
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private AudioClip upgradeSound;
    [SerializeField] private AudioClip harvestSound;

    [Header("Spawn Point Grid Settings")]
    [Tooltip("Manually assign spawn points for crops.")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("Available Crops")]
    public List<Crop> ListOfCrops;

    private PlayerFoodManager m_PlayerFoodManager;
    private UIManager m_UiManager;

    private Dictionary<int, GameObject> activeCrops = new Dictionary<int, GameObject>();

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
        StartCoroutine(PassiveFoodGain());
    }

    public void OnLookAt()
    {
        Debug.Log("Looking at the farm growing: " + CurrentCrop.CropName);

        if (m_UiManager != null)
        {
            m_UiManager.ShowInteractableInfo(
                "Farm (" + CurrentCrop.CropName + ")",
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

            // Clear all active crops
            foreach (GameObject crop in activeCrops.Values)
            {
                Destroy(crop);
            }
            activeCrops.Clear();

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
            if (CurrentCapacity < MaxCapacity)
            {
                CurrentCapacity++;
                SpawnCrops();
                UpdateStatusText();
                Debug.Log("Current Capacity: " + CurrentCapacity);
            }
            else
            {
                Notifications.Send($"Farm ({CurrentCrop.CropName}) is full!", NotificationType.Error, null);
                yield break;
            }
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
        if (CurrentCrop.CropModelPrefab == null)
        {
            Debug.LogError("Crop Model Prefab is not assigned.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points have been assigned. Please assign spawn points manually in the editor.");
            return;
        }

        // Generate a list of available indices
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (!activeCrops.ContainsKey(i)) // Exclude already used positions
            {
                availableIndices.Add(i);
            }
        }

        // Randomly select positions for new crops
        for (int i = 0; i < Mathf.Min(CurrentCapacity - activeCrops.Count, availableIndices.Count); i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            int spawnIndex = availableIndices[randomIndex];

            // Remove the chosen index to prevent re-selection
            availableIndices.RemoveAt(randomIndex);

            Transform spawnPoint = spawnPoints[spawnIndex];
            GameObject cropInstance = Instantiate(CurrentCrop.CropModelPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
            activeCrops[spawnIndex] = cropInstance; // Track the active crop at this position
        }

        Debug.Log($"Spawned crops. Active crops: {activeCrops.Count}/{spawnPoints.Count}");
    }

}
