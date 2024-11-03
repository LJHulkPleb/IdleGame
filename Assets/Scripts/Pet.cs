using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    public int Strength;
    public int Defense;
    public int Health;
    public int StrengthCost;
    public int DefenseCost;
    public int HealthCost;
    public FoodManager foodmanager;

    private void Start()
    {
        foodmanager = FindObjectOfType<FoodManager>();
    }
    public void feedStrength()
    {
        if (foodmanager.totalFood >= StrengthCost)
        {
            foodmanager.totalFood -= StrengthCost;
            Strength += 1;
            StrengthCost *= 2;
            Debug.Log("Strength fed: Now" + Strength);
        }

    }
    public void feedDefense()
    {
        if (foodmanager.totalFood >= DefenseCost)
        {
            foodmanager.totalFood -= DefenseCost;
            Defense += 1;
            DefenseCost *= 2;
            Debug.Log("Defense fed: Now" + Defense);
        }
    }
    public void feedHealth()
    {
        if (foodmanager.totalFood >= HealthCost)
        {
            foodmanager.totalFood -= HealthCost;
            Health += 1;
            HealthCost *= 2;
            Debug.Log("Health fed: Now" + Health);
        }
    }
}
