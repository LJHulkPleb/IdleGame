using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TournamentManager : MonoBehaviour
{
    public List<Tournament> UpcomingTournaments { get; private set; }
    public int NumOfTournaments = 5;
    public int PlayerRank;
    public PlayerPet PlayerPet;
    public OpponentPet OpponentPet;
    public GameObject OpponentPetPrefab;
    public GameObject NotePrefab;
    public Transform[] NoteSpawnPositions;

    private GameObject m_CurrentOpponentPet;

    // Start is called before the first frame update
    void Start()
    {
        GenerateUpcomingTournaments();
        DisplayTournamentNotes();
    }
   
    void GenerateUpcomingTournaments()
    {
        UpcomingTournaments = new List<Tournament>();
        for (int i = 0; i < NumOfTournaments; i++)
        {
            int randomRank = Random.Range(PlayerRank - 1, PlayerRank + 2);
            randomRank = Mathf.Clamp(randomRank, 1, int.MaxValue);
            Tournament newTournament = new Tournament(randomRank);//Change to random pet
            UpcomingTournaments.Add(newTournament);
        }
    }

    void DisplayTournamentNotes()
    {
        for (int i = 0; i < UpcomingTournaments.Count && i < NoteSpawnPositions.Length; i++)
        {
            GameObject note = Instantiate(NotePrefab, NoteSpawnPositions[i].position, Quaternion.Euler(0, -90, 0), NoteSpawnPositions[i]);
            note.GetComponent<TournamentNote>().Initialize(UpcomingTournaments[i], this);
        }
    }

    public void SignUpForTournament(Tournament tournament)
    {
        if (!UpcomingTournaments.Contains(tournament))
        {
            Debug.LogError("Invalid tournament");
            return;
        }

        tournament.IsPlayerParticipating = true;
        SetUpOpponent(tournament);
        SceneManager.LoadScene("TournamentScene");
    }

    IEnumerator ExecuteTournament(Tournament tournamentIndex)
    {
        yield return new WaitForSeconds(2f); 

        SetUpOpponent(tournamentIndex);

        bool playerGoesFirst = PlayerPet.Speed >= OpponentPet.Speed;
        Pet attacker = playerGoesFirst ? PlayerPet : OpponentPet;
        Pet defender = playerGoesFirst ? OpponentPet : PlayerPet;

        while (PlayerPet.Health > 0 && OpponentPet.Health > 0)
        {
            ExecuteAttack(attacker, defender);
            yield return new WaitForSeconds(1f);

            // Swap attacker and defender for next turn
            //(attacker, defender) = (defender, attacker);

            Pet temp = attacker;
            attacker = defender;
            defender = temp;
        }

        if (PlayerPet.Health > 0)
        {
            RewardPlayer();
        }
        else
        {
            NotifyPlayerLoss();
        }
    }

    void SetUpOpponent(Tournament tournament)
    {
        int opponentRank = tournament.Rank;
        OpponentPet = new OpponentPet();
        OpponentPet.Initialize(opponentRank);
    }

    void ExecuteAttack(Pet attacker, Pet defender)
    {
        int damage = attacker.Strength - defender.Defense;
        if (damage > 0)
        {
            defender.Health -= damage;
        }
        else
        {
            damage = 1;
            defender.Health -= damage;
        }
    }

    void RewardPlayer()
    {
        Debug.Log("Player won the tournament! Reward given.");
    }

    void NotifyPlayerLoss()
    {
        Debug.Log("Player lost the tournament.");
    }
}
