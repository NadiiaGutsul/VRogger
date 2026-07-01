using System;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class ShopItem : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject spawnObject;

    [Header("Stacks & Cooldown")]
    private int maxStacks = 5;
    private float cooldown = 10f;
    private int stacks;
    private float cooldownTimer;
    [SerializeField] private PlayerMovement player;

    [Header("Visuals")]
    [SerializeField] private Material baseMat;
    [SerializeField] private Material cooldownMat;
    [SerializeField] private string description;
    private Renderer render;
    private TextMeshPro timerText;
    [SerializeField] private GameObject textObject;
    
    [Header("VR Components")]
    private XRBaseInteractable interactable;
    private XRInteractionManager interactionManager;


    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(OnSelected);

        interactionManager = FindFirstObjectByType<XRInteractionManager>();

        render = GetComponentInChildren<Renderer>();
        timerText = textObject.GetComponent<TextMeshPro>();

        stacks = maxStacks;
    }

    private void Start()
    {
        player.goalReached.AddListener(OnLevelSwitch);
    }

    private void Update()
    {
        HandleCooldown();
        UpdateVisuals();
    }
    
    // Public so TimerCollectible can call it on pickup
    public void ReduceCooldown(float  amount)
    {
        cooldownTimer -= amount;
    }

    private void OnLevelSwitch(GameObject goal)
    {
        if (stacks < maxStacks)
        {
            stacks++;
        }
    }

    private void HandleCooldown()
    {
        if (stacks >= maxStacks)
            return;

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            stacks++;
            cooldownTimer = cooldown;
        }
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        if (stacks <= 0 || interactionManager == null || GameManager.instance.isPaused)
            return;

        stacks--;

        if (stacks < maxStacks)
            cooldownTimer = cooldown;

        GameObject spawned = Instantiate(
            spawnObject,
            transform.position,
            Quaternion.identity
        );

        // Force grab the spawned item
        XRGrabInteractable grab = spawned.GetComponent<XRGrabInteractable>();
        if (!grab)
        {
            
            return;
        }
        
        interactionManager.SelectEnter(
            args.interactorObject,
            grab
        );
    }

    private void SetGrabbed()
    {
        
    }

    private void UpdateVisuals()
    {
        bool available = stacks > 0;

        render.material = available ? baseMat : cooldownMat;

        if (timerText)
        {
            string timer = stacks < maxStacks
                ? Mathf.CeilToInt(cooldownTimer).ToString()
                : "Ready";

            timerText.text =
                // $"{description}\nStacks: {stacks}/{maxStacks}\nNext: {timer}";
                $"Stacks: {stacks}/{maxStacks}\nNext: {timer}";
        }
    }
}
