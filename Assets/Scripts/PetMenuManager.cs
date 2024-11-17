using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PetMenuManager : MonoBehaviour
{
    public Pet pet;
    public TMP_Text NameText;
    public TMP_Text HealthText;
    public TMP_Text StrengthText;
    public TMP_Text DefenseText;
    public TMP_Text SpeedText;
    
    public void UpdateStatsUI()
    {
        //NameText.text = pet.Name.ToString();
        StrengthText.text = "Strength: " + pet.Strength.ToString();
        DefenseText.text = "Defense: " + pet.Defense.ToString();
        HealthText.text = "Health: " + pet.Health.ToString();
    }
    private void Update()
    {
        UpdateStatsUI();
    }
}
