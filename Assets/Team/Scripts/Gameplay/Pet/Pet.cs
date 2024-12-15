using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pet : MonoBehaviour, IInteractable
{
    [SerializeField] private PetStats m_Stats;

    public PetStats Stats { get => m_Stats; set => m_Stats = value; }

    private UIManager m_UiManager;
    private PlayerFoodManager m_FoodManager;
    private PetController m_PetController;
    private Crop m_CurrentCrop;
    private Animator m_PetAnimator;

    private void Start()
    {
        m_UiManager = FindObjectOfType<UIManager>();
        m_FoodManager = FindObjectOfType<PlayerFoodManager>();
        m_PetController = GetComponent<PetController>();

        if (m_FoodManager.Crops.Count > 0)
        {
            m_CurrentCrop = m_FoodManager.Crops[0].Crop;
        }

    }

    public void Initialize(PetStats stats)
    {
        m_Stats = stats;
    }

    public void RandomInitialize(int rank)
    {
        m_Stats = new (
             "Opponent",
            Random.Range(rank * 5, rank * 10),
            Random.Range(rank * 4, rank * 8),
            Random.Range(rank * 20, rank * 40),
            Random.Range(rank * 3, rank * 7),
            "None"
        );

        Debug.Log($"Initialized Pet: Strength={m_Stats.Strength}, Defense={m_Stats.Defense}, Speed={m_Stats.Speed}, Health={m_Stats.Health}");
    }

    public void OnLookAt()
    {
        if (m_PetController != null)
        {
            m_PetController.enabled = false;
        }

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("forward", 0);
            animator.SetBool("isIdle", true);
        }

        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
        }

        Vector3 directionToPlayer = (m_UiManager.PlayerCamera.transform.position - transform.position).normalized;
        directionToPlayer.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);
        m_UiManager.ShowInteractableInfo(
            Stats.PetName.ToString(),
            $"Strength: {Stats.Strength}\nDefense: {Stats.Defense}\nSpeed: {Stats.Speed} \nHealth {Stats.Health}",
                "Press 'E' to feed" + m_CurrentCrop,
                  "Press 'Q' to change crop",
                  ""
                  );
        Highlight(true);
    }

    public void OnInteract()
    {
        if (m_FoodManager == null || m_CurrentCrop == null) return;
        
        Feed(m_CurrentCrop);
    }

    public void OnSecondaryInteract()
    {
        if (m_FoodManager.Crops.Count > 0)
        {
            int currentIndex = m_FoodManager.Crops.FindIndex(c => c.Crop == m_CurrentCrop);
            int nextIndex = (currentIndex + 1) % m_FoodManager.Crops.Count;
            m_CurrentCrop = m_FoodManager.Crops[nextIndex].Crop;
        }
        else
        {
        }
    }
    public void OnTertiaryInteract(){} //Maybe pet name change idk

    public void Feed(Crop crop)
    {
        CropInfo cropInfo = m_FoodManager.Crops.Find(c => c.Crop == crop);
        if (cropInfo == null || cropInfo.Amount < crop.UpgradeCost)
        {
            return;
        }

        m_FoodManager.UseCrop(crop, crop.UpgradeCost);

        Stats.Strength += crop.AttributeBoosts[0].BoostAmount;
        Stats.Defense += crop.AttributeBoosts[1].BoostAmount;
        Stats.Speed += crop.AttributeBoosts[2].BoostAmount;
        Stats.Health += crop.AttributeBoosts[3].BoostAmount;

    }

    public void Attack(Pet defender)
    {
        int damage = Stats.Strength - defender.Stats.Defense;
        damage = damage > 0 ? damage : 1;
        defender.Stats.Health -= damage;

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

        if (m_PetController != null)
        {
            m_PetController.enabled = true;
        }

        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("forward", .1f);
            animator.SetBool("isIdle", false);
        }

        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = false;
        }
    }
}