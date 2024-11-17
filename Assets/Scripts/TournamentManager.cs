using System.Collections;
using System.Collections.Generic;
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
    public GameObject ButtonPrefab;
    public Transform ButtonParent;

    private GameObject m_CurrentOpponentPet;

    // Start is called before the first frame update
    void Start()
    {
        GenerateUpcomingTournaments();
        DisplayTournamentOptions();
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

    void DisplayTournamentOptions()
    {
        foreach (Tournament tournament in UpcomingTournaments)
        {
            GameObject button = Instantiate(ButtonPrefab, ButtonParent);
            button.GetComponentInChildren<Text>().text = "Tournament Rank: " + tournament.Rank;
            int index = UpcomingTournaments.IndexOf(tournament);
            button.GetComponent<Button>().onClick.AddListener(() => SignUpForTournament(index));
        }
    }

    public void SignUpForTournament(int tournamentIndex)
    {
        if (tournamentIndex < 0 || tournamentIndex >= UpcomingTournaments.Count)
        {
            Debug.LogError("Invalid tournament index");
            return;
        }

        UpcomingTournaments[tournamentIndex].IsPlayerParticipating = true;
        SetUpOpponent(tournamentIndex);
        SceneManager.LoadScene("TournamentScene");
    }

    IEnumerator ExecuteTournament(int tournamentIndex)
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

    void SetUpOpponent(int tournamentIndex)
    {
        int opponentRank = UpcomingTournaments[tournamentIndex].Rank;
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
