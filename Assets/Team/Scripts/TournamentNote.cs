using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentNote : MonoBehaviour, IInteractable
{
    private Tournament tournament;
    private TournamentManager tournamentManager;
    private MenuManager menuManager;

    public void Initialize(Tournament tournament, TournamentManager manager)
    {
        this.tournament = tournament;
        tournamentManager = manager;
        menuManager = FindObjectOfType<MenuManager>();
    }

    public void OnLookAt()
    {
        if (menuManager != null)
        {
            menuManager.ShowInteractableInfo(
                "Tournament Rank: " + tournament.Rank,
                "Press 'E' to sign up for this tournament."
            );
            Highlight(true);
        }
    }

    public void OnInteract()
    {
        if (menuManager != null)
        {
            menuManager.ShowInteractableInfo(
                "Tournament Rank: " + tournament.Rank,
                "Signing up for tournament..."
            );
            tournamentManager.SignUpForTournament(tournament);
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
