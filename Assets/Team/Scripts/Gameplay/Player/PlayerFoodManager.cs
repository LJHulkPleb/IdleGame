using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerFoodManager : MonoBehaviour
{
    public List<CropInfo> Crops = new List<CropInfo>();

    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _defenseText;
    [SerializeField] private TMP_Text _strengthText;
    [SerializeField] private TMP_Text _speedText;
    [SerializeField] private TMP_Text _vitalisText;

    private Dictionary<string, (TMP_Text textField, string prefix)> cropTextFields;
    private bool isFarmScene;

    private void Start()
    {
        isFarmScene = SceneManager.GetActiveScene().name == "FarmScene";

        if (!isFarmScene)
        {
            Debug.Log("Not in FarmScene. Skipping crop text updates.");
            return;
        }

        cropTextFields = new Dictionary<string, (TMP_Text, string)>
        {
            { "Health", (_healthText, "Health: ") },
            { "Defense", (_defenseText, "Defense: ") },
            { "Strength", (_strengthText, "Strength: ") },
            { "Speed", (_speedText, "Speed: ") },
            { "Vitalis", (_vitalisText, "Vitalis: ") }
        };

        UpdateCropTexts();
    }

    public void AddCrop(Crop crop, int amount)
    {
        CropInfo existingCrop = Crops.Find(c => c.Crop == crop);
        if (existingCrop != null)
        {
            existingCrop.Amount += amount;
        }
        else
        {
            Crops.Add(new CropInfo { Crop = crop, Amount = amount });
        }
        Debug.Log("Added " + amount + " units of " + crop.CropName + ". Current amount: " + Crops.Find(c => c.Crop == crop).Amount);

        if (isFarmScene)
        {
            UpdateCropTexts();
        }
    }

    public bool HasCrop(Crop crop, int requiredAmount)
    {
        CropInfo existingCrop = Crops.Find(c => c.Crop == crop);
        return existingCrop != null && existingCrop.Amount >= requiredAmount;
    }

    public void UseCrop(Crop crop, int amount)
    {
        CropInfo existingCrop = Crops.Find(c => c.Crop == crop);
        if (existingCrop != null && existingCrop.Amount >= amount)
        {
            existingCrop.Amount -= amount;
            Debug.Log("Used " + amount + " units of " + crop.CropName + ". Remaining: " + existingCrop.Amount);

            if (existingCrop.Amount <= 0)
            {
                Crops.Remove(existingCrop);
                Debug.Log("Removed " + crop.CropName + " from inventory as the amount is zero.");
            }
        }
        else
        {
            Debug.Log("Not enough " + crop.CropName + " to use.");
        }

        if (isFarmScene)
        {
            UpdateCropTexts();
        }
    }

    private void UpdateCropTexts()
    {
        if (!isFarmScene) return;

        foreach (var entry in cropTextFields.Values)
        {
            entry.textField.text = entry.prefix + "0";
        }

        foreach (var cropInfo in Crops)
        {
            if (cropTextFields.ContainsKey(cropInfo.Crop.CropName))
            {
                var (textField, prefix) = cropTextFields[cropInfo.Crop.CropName];
                textField.text = prefix + cropInfo.Amount.ToString();
            }
        }
    }
}
