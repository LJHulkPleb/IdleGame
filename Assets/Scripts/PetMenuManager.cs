using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetMenuManager : MonoBehaviour
{
    public Pet pet;
    public Text StrengthText;
    public Text DefenseText;
    public Text HealthText;
    public void UpdateStatsUI()
    {
        StrengthText.text = "Strength: " + pet.Strength.ToString();
        DefenseText.text = "Defense: " + pet.Defense.ToString();
        HealthText.text = "Health: " + pet.Health.ToString();
    }
    private void Update()
    {
        UpdateStatsUI();
    }
}
