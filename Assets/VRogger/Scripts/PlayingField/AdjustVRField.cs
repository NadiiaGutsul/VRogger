using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AdjustVRField : MonoBehaviour
{
    [SerializeField] private GameObject pcEnvironment;
    private XRBaseInteractable interactable;
    private bool isGrabbed = false;
    private GameObject controller;
    private float sensitivity = 1f;
    private float lastY;
    private Collider col;
    private Renderer render;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
        col = GetComponent<Collider>();
        render = GetComponent<Renderer>();
        
        interactable.selectEntered.AddListener(EnableGrabState);
        interactable.selectExited.AddListener(DisableGrabState);
    }

    private void Update()
    {
        MovePlayingField();
        EnableOnlyDuringPause();
    }

    private void MovePlayingField()
    {
        
        if (!isGrabbed) return;
        
        float currentY = controller.transform.position.y;
        float deltaY = currentY - lastY;

        // Move object only in WORLD Y
        pcEnvironment.transform.position += new Vector3(0f, deltaY * sensitivity, 0f);

        lastY = currentY;
    }

    private void EnableGrabState(SelectEnterEventArgs eventArgs)
    {
        isGrabbed = true;
        controller = eventArgs.interactorObject.transform.gameObject;
        lastY = controller.transform.position.y;
    }
    
    private void DisableGrabState(SelectExitEventArgs eventArgs)
    {
        isGrabbed = false;
        controller = null;
    }

    private void EnableOnlyDuringPause()
    {
        col.enabled = GameManager.instance.isPaused;
        render.enabled = GameManager.instance.isPaused;
    }
}
