using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tournament
{
    public int Rank { get; private set; }
    public bool IsPlayerParticipating { get; set; }
    public OpponentPet OpponentPet { get; set; }

    public Tournament(int rank)
    {
        Rank = rank;
        OpponentPet = new OpponentPet();
        OpponentPet.Initialize(rank);

        IsPlayerParticipating = false;
    }

    public Tournament(int rank, Pet opponentPet)
    {
        Rank = rank;
        OpponentPet = (OpponentPet)opponentPet;
        OpponentPet.Initialize(rank);
        IsPlayerParticipating = false;

    }
}
