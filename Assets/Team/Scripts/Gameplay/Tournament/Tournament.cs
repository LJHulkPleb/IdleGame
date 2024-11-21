using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tournament
{
    public TournamentRank Rank { get; private set; }
    public bool IsPlayerParticipating { get; set; }
    public OpponentPet OpponentPet { get; set; }
    public float TimeLeft { get; set; }
    public float TimeToJoin { get; set; }

    public Tournament(TournamentRank rank, float timeToJoin)
    {
        Rank = rank;
        TimeToJoin = TimeLeft = timeToJoin;
        OpponentPet = new OpponentPet();
        OpponentPet.Initialize((int)rank);

        IsPlayerParticipating = false;
    }

    public Tournament(TournamentRank rank, Pet opponentPet, float timeToJoin)
    {
        Rank = rank;
        TimeToJoin = TimeLeft = timeToJoin;
        OpponentPet = (OpponentPet)opponentPet;
        OpponentPet.Initialize((int)rank);
        IsPlayerParticipating = false;
    }
}
