using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Crop", menuName = "IdleGame/Crop")]
public class Crop : ScriptableObject
{
    public string cropName;
    public int foodPerClick;
    public int upgradeCost;
    public int currentLevel = 1;

    private void OnEnable()
    {
        ResetValues();
    }

    public void ResetValues()
    {
        foodPerClick = 1;
        upgradeCost = 5;
    }
}
