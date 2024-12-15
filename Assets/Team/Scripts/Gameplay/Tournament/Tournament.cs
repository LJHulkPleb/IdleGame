using UnityEngine;

public class Tournament
{
    public TournamentRank Rank { get; private set; }
    public bool IsPlayerParticipating { get; set; }
    public GameObject OpponentPetPrefab { get; private set; }
    public float TimeLeft { get; set; }
    public float TimeToJoin { get; private set; }

    public Tournament(TournamentRank rank, GameObject opponentPrefab, float timeToJoin)
    {
        Rank = rank;
        OpponentPetPrefab = opponentPrefab;
        TimeToJoin = TimeLeft = timeToJoin;
        IsPlayerParticipating = false;
    }
}