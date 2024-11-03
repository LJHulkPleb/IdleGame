using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodManager : MonoBehaviour
{
    public Text foodCountText;
    public Text upgradeCostText;
    public Crop crop;
    public int totalFood = 0;

    private void Start()
    {
        UpdateFoodUI();
        UpdateUpgradeCostUI();
    }
    private void Update()
    {
        UpdateFoodUI();
    }
    public void produceFood(Crop crop)
    {
        Debug.Log("Button Clicked");
        totalFood += crop.foodPerClick;
        UpdateFoodUI();
        Debug.Log("Total Amount of food now: " + totalFood);
    }
    private void UpdateFoodUI()
    {
        foodCountText.text = "Food: " + totalFood.ToString();
    }
    public void UpgradeCrop()
    {
        if(totalFood >= crop.upgradeCost)
        {
            totalFood -= crop.upgradeCost;
            crop.currentLevel++;
            crop.foodPerClick += 1;
            crop.upgradeCost *= 2;
            UpdateFoodUI();
            UpdateUpgradeCostUI();
        }
    }
    private void UpdateUpgradeCostUI()
    {
        upgradeCostText.text = "Upgrade Cost: " + crop.upgradeCost.ToString();
    }
}
