using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TournamentSceneManager : MonoBehaviour
{
    public Transform PlayerSpawnPoint;
    public Transform OpponentSpawnPoint;
    public Slider PlayerHealthBar;
    public Slider OpponentHealthBar;

    private void Start()
    {
        Tournament currentTournament = TournamentManager.Instance.GetActiveTournament();

        if (currentTournament == null || currentTournament.OpponentPetPrefab == null)
        {
            Debug.LogError("No active tournament or opponent pet!");
            return;
        }

        Pet playerPet = Instantiate(
            SelectedPetManager.Instance.GetSelectedPet(),
            PlayerSpawnPoint.position,
            PlayerSpawnPoint.rotation
        ).GetComponent<Pet>();
        playerPet.transform.SetParent(PlayerSpawnPoint);
        SetUpPet(playerPet);

        Pet opponentPet = Instantiate(
            currentTournament.OpponentPetPrefab,
            OpponentSpawnPoint.position,
            OpponentSpawnPoint.rotation
        ).GetComponent<Pet>();
        opponentPet.transform.SetParent(OpponentSpawnPoint);
        opponentPet.RandomInitialize((int)currentTournament.Rank);
        SetUpPet(opponentPet);

        InitializeHealthBars(playerPet, opponentPet);

        StartCoroutine(StartBattle(playerPet, opponentPet));
    }

    private void SetUpPet(Pet pet)
    {
        if (pet.TryGetComponent(out NavMeshAgent navMeshAgent))
        {
            navMeshAgent.enabled = false;
        }

        if (pet.TryGetComponent(out PetController petController))
        {
            petController.enabled = false;
        }
    }

    private void InitializeHealthBars(Pet playerPet, Pet opponentPet)
    {
        PlayerHealthBar.maxValue = playerPet.Stats.Health;
        PlayerHealthBar.value = playerPet.Stats.Health;

        OpponentHealthBar.maxValue = opponentPet.Stats.Health;
        OpponentHealthBar.value = opponentPet.Stats.Health;
    }

    private IEnumerator StartBattle(Pet playerPet, Pet opponentPet)
    {
        yield return new WaitForSeconds(2f);

        bool playerGoesFirst = playerPet.Stats.Speed >= opponentPet.Stats.Speed;
        Pet attacker = playerPet.Stats.Speed >= opponentPet.Stats.Speed ? playerPet : opponentPet;
        Pet defender = attacker == playerPet ? opponentPet : playerPet;

        while (playerPet.Stats.Health > 0 && opponentPet.Stats.Health > 0)
        {
            Debug.Log($"{attacker.Stats.PetName} is attacking {defender.Stats.PetName}");
            attacker.Attack(defender);
            UpdateHealthBars(playerPet, opponentPet);
            yield return new WaitForSeconds(3f);
            (attacker, defender) = (defender, attacker);
        }

        if (playerPet.Stats.Health > 0)
        {
            Debug.Log($"{playerPet.Stats.PetName} wins the tournament!");
            TournamentManager.Instance.RewardPlayer();
        }
        else
        {
            Debug.Log($"{opponentPet.Stats.PetName} wins. Better luck next time!");
            TournamentManager.Instance.NotifyPlayerLoss();
            SceneManager.LoadScene("FarmScene");
        }
    }


    private void UpdateHealthBars(Pet playerPet, Pet opponentPet)
    {
        PlayerHealthBar.value = Mathf.Max(playerPet.Stats.Health, 0);
        OpponentHealthBar.value = Mathf.Max(opponentPet.Stats.Health, 0);
    }
}
