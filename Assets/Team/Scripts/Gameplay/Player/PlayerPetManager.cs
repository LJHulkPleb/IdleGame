using System.Collections.Generic;
using UnityEngine;

public class PlayerPetManager : MonoBehaviour
{
    public Pet CurrentPet { get; private set; }
    public List<Pet> OwnedPets { get; private set; } = new List<Pet>();
    public Transform SpawnPoint;

    public void AddPet(Pet newPet)
    {
        if (!OwnedPets.Contains(newPet))
        {
            OwnedPets.Add(newPet);
            Debug.Log($"Added {newPet.Stats.PetName} to player's pets.");
        }
    }

    public void SetCurrentPet(Pet pet)
    {
        if (OwnedPets.Contains(pet))
        {
            CurrentPet = pet;
            Debug.Log($"Set current pet to {pet.Stats.PetName}.");
        }
        else
        {
            Debug.LogWarning("Attempted to set a pet that isn't owned by the player.");
        }
    }

    public void SpawnAndAssignSelectedPet()
    {
        Pet selectedPetPrefab = SelectedPetManager.Instance.GetSelectedPet();
        string selectedPetName = SelectedPetManager.Instance.GetSelectedPetName();
        PetStats selectedPetStats = SelectedPetManager.Instance.GetSelectedPetStats();

        if (selectedPetPrefab != null)
        {
            Pet spawnedPet = Instantiate(selectedPetPrefab, SpawnPoint.position, SpawnPoint.rotation);
            spawnedPet.transform.SetParent(SpawnPoint);
            spawnedPet.transform.localPosition = Vector3.zero;

            spawnedPet.Stats.PetName = selectedPetName;

            if (selectedPetStats != null)
            {
                spawnedPet.Stats.Strength = selectedPetStats.Strength;
                spawnedPet.Stats.Defense = selectedPetStats.Defense;
                spawnedPet.Stats.Speed = selectedPetStats.Speed;
                spawnedPet.Stats.Health = selectedPetStats.Health;
                Debug.Log($"Assigned stats to {selectedPetName}: Str-{selectedPetStats.Strength}, Def-{selectedPetStats.Defense}, Spd-{selectedPetStats.Speed}, Hp-{selectedPetStats.Health}");
            }
            else
            {
                Debug.LogWarning("No stats found for the selected pet. Default stats will be used.");
            }

            AddPet(spawnedPet);
            SetCurrentPet(spawnedPet);
        }
        else
        {
            Debug.LogError("No pet selected!");
        }
    }
}
