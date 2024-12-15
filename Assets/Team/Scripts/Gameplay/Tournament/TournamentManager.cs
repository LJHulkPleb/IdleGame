using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Group3d.Notifications;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TournamentManager : MonoBehaviour
{
    public static TournamentManager Instance { get; private set; }

    public GameObject TournamentBoardPrefab;
    private GameObject tournamentBoardInstance;

    public List<Tournament> UpcomingTournaments { get; private set; }
    public int NumOfTournaments = 5;
    public TournamentRank PlayerRank = TournamentRank.RankE;
    public List<GameObject> OpponentPetPrefabs;
    public GameObject NotePrefab;
    public Transform[] NoteSpawnPositions;
    public string[] TournamentSceneNames;

    private Tournament m_ActiveTournament;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("TournamentManager initialized and marked as persistent.");
            UpcomingTournaments = new List<Tournament>();
            StartCoroutine(GenerateTournaments());
        }
        else if (Instance != this)
        {
            if (SceneManager.GetActiveScene().name == "FarmScene")
            {
                Debug.Log("Using existing TournamentManager in FarmScene.");
                Instance.InitializeFromPreviousInstance(this);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("A new scene loaded with a different TournamentManager. Persisting the original.");
                Destroy(Instance.gameObject);
                Instance = this;
                DontDestroyOnLoad(gameObject);
                UpcomingTournaments = new List<Tournament>();
                StartCoroutine(GenerateTournaments());
            }
        }

        if (NoteSpawnPositions != null && NoteSpawnPositions.Length != 0) return;
        NoteSpawnPositions = FindObjectsOfType<Transform>()
            .Where(t => t.CompareTag("NoteSpawnMarker"))
            .ToArray();
        Debug.Log($"Found and assigned {NoteSpawnPositions.Length} spawn positions dynamically.");

    }

    public void InitializeFromPreviousInstance(TournamentManager previousManager)
    {
        m_ActiveTournament = previousManager.m_ActiveTournament;
        PlayerRank = previousManager.PlayerRank;
        UpcomingTournaments = previousManager.UpcomingTournaments;
    }

    private void Start()
    {
        if (UpcomingTournaments == null)
        {
            UpcomingTournaments = new List<Tournament>();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        
        if (NoteSpawnPositions != null && NoteSpawnPositions.Length > 0)
        {
            StartCoroutine(GenerateTournaments());
        }
        else
        {
            Debug.LogWarning("NoteSpawnPositions is not ready. Tournaments will not be generated.");
        }
    }


    private IEnumerator GenerateTournaments()
    {
        Debug.Log("Generating tournaments");
        while (true)
        {
            if (NoteSpawnPositions == null || NoteSpawnPositions.Length == 0)
            {
                Debug.LogWarning("NoteSpawnPositions is not assigned or empty. Stopping tournament generation.");
                yield break;
            }

            if (UpcomingTournaments.Count < NumOfTournaments)
            {
                foreach (Transform position in NoteSpawnPositions)
                {
                    if (position != null && position.childCount == 0)
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
    private void AddNewTournament(Transform position)
    {
        if (UpcomingTournaments == null)
        {
            Debug.LogError("UpcomingTournaments is null!");
            return;
        }

        TournamentRank rank = (TournamentRank)Random.Range((int)PlayerRank, Mathf.Clamp((int)PlayerRank + 1, 1, System.Enum.GetValues(typeof(TournamentRank)).Length));
        float timeToJoin = Random.Range(30, 60);

        if (OpponentPetPrefabs == null || OpponentPetPrefabs.Count == 0)
        {
            Debug.LogError("OpponentPetPrefabs is null or empty!");
            return;
        }

        int randomIndex = Random.Range(0, OpponentPetPrefabs.Count);
        GameObject selectedOpponentPrefab = OpponentPetPrefabs[randomIndex];

        Tournament newTournament = new Tournament(rank, selectedOpponentPrefab, timeToJoin);

        if (newTournament == null)
        {
            Debug.LogError("newTournament is null!");
            return;
        }

        Debug.Log($"Adding Tournament: {newTournament.Rank}, TimeToJoin: {newTournament.TimeToJoin}");
        UpcomingTournaments.Add(newTournament);
        DisplayTournamentNote(newTournament, position);
    }


    private void DisplayTournamentNote(Tournament tournament, Transform position)
    {
        GameObject note = Instantiate(NotePrefab, position.position, Quaternion.Euler(0, -90, 0), position);
        note.GetComponent<TournamentNote>().Initialize(tournament, this);
        StartCoroutine(RemoveTournamentAfterTimeout(note, tournament));
    }

    private IEnumerator RemoveTournamentAfterTimeout(GameObject note, Tournament tournament)
    {
        yield return new WaitForSeconds(tournament.TimeToJoin);

        if (UpcomingTournaments.Contains(tournament))
        {
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
        m_ActiveTournament = tournament;

        if (TournamentSceneNames == null || TournamentSceneNames.Length == 0)
        {
            Debug.LogError("TournamentSceneNames is not populated. Please add tournament scenes.");
            return;
        }

        string randomSceneName = TournamentSceneNames[Random.Range(0, TournamentSceneNames.Length)];
        Debug.Log($"Signed up for tournament: {tournament.Rank}. Loading scene: {randomSceneName}");

        SceneManager.LoadScene(randomSceneName);
    }


    public Tournament GetActiveTournament() => m_ActiveTournament;

    public void RewardPlayer() => Debug.Log("Player won the tournament! Reward given.");
    public void NotifyPlayerLoss() => Debug.Log("Player lost the tournament.");

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "FarmScene")
        {
            Debug.Log("Loading FarmScene...");
            gameObject.SetActive(true);

            StartCoroutine(DelayedLinkSceneObjects());
            StartCoroutine(DelayedGenerateTournaments());
        }
    }

    private IEnumerator DelayedGenerateTournaments()
    {
        yield return new WaitForSeconds(0.1f);
        if (NoteSpawnPositions != null && NoteSpawnPositions.Length > 0)
        {
            StartCoroutine(GenerateTournaments());
        }
        else
        {
            Debug.LogWarning("NoteSpawnPositions is not ready even after delay.");
        }
    }

    private IEnumerator DelayedLinkSceneObjects()
    {
        yield return new WaitForSeconds(0.1f);
        LinkSceneObjects();
    }

    public void LinkSceneObjects()
    {
        if (SceneManager.GetActiveScene().name != "FarmScene")
        {
            Debug.Log("Not in FarmScene. Skipping TournamentBoard setup.");
            return;
        }

        GameObject tournamentBoard = GameObject.Find("TournamentBoard");

        if (tournamentBoard != null)
        {
            Debug.Log("Found existing TournamentBoard in FarmScene.");
            tournamentBoardInstance = tournamentBoard;

            NoteSpawnPositions = tournamentBoard.GetComponentsInChildren<NoteSpawnMarker>()
                .Select(marker => marker.transform)
                .ToArray();

            NumOfTournaments = NoteSpawnPositions.Length;
        }
        else
        {
            Debug.LogError("TournamentBoard not found in FarmScene.");
            NoteSpawnPositions = new Transform[0];
            NumOfTournaments = 0;
        }

        Debug.Log($"Assigned {NoteSpawnPositions.Length} note spawn positions. NumOfTournaments set to {NumOfTournaments}.");
    }
    
    private void OnDestroy()
    {
        if (tournamentBoardInstance != null)
        {
            Destroy(tournamentBoardInstance);
            tournamentBoardInstance = null;
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log($"TournamentManager instance {this.gameObject.name} destroyed.");
    }
}
