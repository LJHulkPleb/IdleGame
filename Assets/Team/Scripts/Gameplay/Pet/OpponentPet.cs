using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentPet : Pet
{
    public void Initialize(int rank)
    {
        Speed = Random.Range(rank * 5, rank * 10);
        Strength = Random.Range(rank * 5, rank * 10);
        Defense = Random.Range(rank * 3, rank * 7);
        Health = Random.Range(rank * 10, rank * 20);
    }
}
