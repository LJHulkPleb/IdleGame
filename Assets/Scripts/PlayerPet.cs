using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPet : Pet
{
    public int StrengthCost = 1;
    public int DefenseCost = 1;
    public int HealthCost = 1;
    public FoodManager FoodManager;

    private void Start()
    {
        FoodManager = FindObjectOfType<FoodManager>();
    }

    public void FeedStrength()
    {
        if (FoodManager.totalFood >= StrengthCost)
        {
            FoodManager.totalFood -= StrengthCost;
            Strength += 1;
            StrengthCost *= 2;
            Debug.Log("Strength fed: Now" + Strength);
        }

    }
    public void FeedDefense()
    {
        if (FoodManager.totalFood >= DefenseCost)
        {
            FoodManager.totalFood -= DefenseCost;
            Defense += 1;
            DefenseCost *= 2;
            Debug.Log("Defense fed: Now" + Defense);
        }
    }
    public void FeedHealth()
    {
        if (FoodManager.totalFood >= HealthCost)
        {
            FoodManager.totalFood -= HealthCost;
            Health += 1;
            HealthCost *= 2;
            Debug.Log("Health fed: Now" + Health);
        }
    }
}
