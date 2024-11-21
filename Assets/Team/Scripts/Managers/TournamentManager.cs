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
    public int PlayerRank = 1;
    public PlayerPet PlayerPet;
    public OpponentPet OpponentPet;
    public GameObject OpponentPetPrefab;
    public GameObject NotePrefab;
    public Transform[] NoteSpawnPositions;

    private GameObject m_CurrentOpponentPet;

    // Start is called before the first frame update
    void Start()
    {
        UpcomingTournaments = new List<Tournament>();
        StartCoroutine(GenerateTournaments());
    }

    IEnumerator GenerateTournaments()
    {
        while (true)
        {
            if (UpcomingTournaments.Count < NumOfTournaments)
            {
                foreach (Transform position in NoteSpawnPositions)
                {
                    if (position.childCount == 0)
                    {
                        yield return new WaitForSeconds(Random.Range(10, 20));
                        AddNewTournament(position);
                        break;
                    }
                }
            }
            yield return null;
        }
    }

    void AddNewTournament(Transform position)
    {
        TournamentRank rank = (TournamentRank)Random.Range(PlayerRank, Mathf.Clamp(PlayerRank + 1, 1, System.Enum.GetValues(typeof(TournamentRank)).Length));
        float timeToJoin = Random.Range(30, 60);
        Tournament newTournament = new Tournament(rank, timeToJoin);
        UpcomingTournaments.Add(newTournament);

        DisplayTournamentNote(newTournament, position);

        Debug.Log("New Tournament Added: " + newTournament.Rank + " with time left: " + newTournament.TimeLeft + "s");
    }


    void DisplayTournamentNote(Tournament tournament, Transform position)
    {
        GameObject note = Instantiate(NotePrefab, position.position, Quaternion.Euler(0, -90, 0), position);
        note.GetComponent<TournamentNote>().Initialize(tournament, this);
        StartCoroutine(RemoveTournamentAfterTimeout(note, tournament));
    }

    IEnumerator RemoveTournamentAfterTimeout(GameObject note, Tournament tournament)
    {
        yield return new WaitForSeconds(tournament.TimeToJoin);

        if (UpcomingTournaments.Contains(tournament))
        {
            Debug.Log("Tournament Expired: " + tournament.Rank);
            UpcomingTournaments.Remove(tournament);
            Destroy(note);
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
        if (m_CurrentOpponentPet != null)
        {
            Destroy(m_CurrentOpponentPet);
        }
        int opponentRank = (int)tournament.Rank;
        m_CurrentOpponentPet = Instantiate(OpponentPetPrefab);
        OpponentPet opponentPetComponent = m_CurrentOpponentPet.GetComponent<OpponentPet>();
        opponentPetComponent.Initialize(opponentRank);
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
