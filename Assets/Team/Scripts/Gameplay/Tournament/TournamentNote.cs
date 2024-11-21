using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentNote : MonoBehaviour, IInteractable
{
    private Tournament Tournament;
    private TournamentManager TournamentManager;
    private MenuManager MenuManager;

    public void Initialize(Tournament tournament, TournamentManager manager)
    {
        Tournament = tournament;
        TournamentManager = manager;
        MenuManager = FindObjectOfType<MenuManager>();
    }

    public void Update()
    {
        if (Tournament.TimeLeft > 0)
        {
            Tournament.TimeLeft -= Time.deltaTime;
        }
        else
        {
            Tournament.TimeLeft = 0;
        }
    }

    public void OnLookAt()
    {
        if (MenuManager != null)
        {
            MenuManager.ShowInteractableInfo(
                "Tournament Rank: " + Tournament.Rank.ToString().Insert(4, " "),
                "\nTime Left: " + Tournament.TimeLeft.ToString("F0") + "s\n" +
                "Press 'E' to sign up for this tournament."
            );
            Highlight(true);
        }
    }

    public void OnInteract()
    {
        if (MenuManager != null)
        {
            MenuManager.ShowInteractableInfo(
                "Tournament Rank: " + Tournament.Rank,
                "Signing up for tournament..."
            );
            TournamentManager.SignUpForTournament(Tournament);
        }
    }
    private void Highlight(bool highlight)
    {
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = highlight ? Color.yellow : Color.white;
        }
    }

    private void OnMouseExit()
    {
        Highlight(false);
    }
}
