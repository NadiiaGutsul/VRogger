using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRInteractable : MonoBehaviour
{
    private int duration = 5;

    [SerializeField] private string snapTag;
    [SerializeField] private GameObject[] snapCubes;
    private VRInteractableManager manager;

    private XRGrabInteractable interactable;

    // Listener have to be in Awake for it to show the place indicators properly on first grab
    private void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
        
        manager = GameObject.FindGameObjectWithTag("VRInteractableManager").GetComponent<VRInteractableManager>();
        
        interactable.selectExited.AddListener(SelectedExit);
        interactable.selectEntered.AddListener(SelectedEnter);
    }
    

    private void SelectedEnter(SelectEnterEventArgs args)
    {

        if (snapTag == "StreetSnap")
        {
            manager.SetTrafficGrabbed(true);
        }
        else if (snapTag == "WaterSnap")
        {
            manager.SetLilypadGrabbed(true);
        }
    }

    private void SelectedExit(SelectExitEventArgs args)
    {
        
        if (snapTag == "StreetSnap")
        {
            manager.SetTrafficGrabbed(false);
        }
        else if (snapTag == "WaterSnap")
        {
            manager.SetLilypadGrabbed(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Prevent snapping until it has been dropped properly
        if (interactable.isSelected) return;
        
        if (other.CompareTag(snapTag))
        {
            SnapToTarget(other);
        }
    }

    private void SnapToTarget(Collider other)
    {
        Vector3 cubePos = other.transform.position;
        float offset = other.bounds.extents.y;

        transform.position = new Vector3(
            cubePos.x,
            cubePos.y - offset,
            cubePos.z
        );

        transform.parent = other.transform.parent;
        transform.rotation = other.transform.rotation;

        // Set layer after snapping so moving objects don't stop until it's snapped in place --> only cars can interact with cone, sealily with trees etc.
        gameObject.layer = LayerMask.NameToLayer("VRInteractables");

        interactable.enabled = false;

        Destroy(gameObject, duration);
    }
}