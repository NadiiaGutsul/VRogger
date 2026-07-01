using System;
using Unity.VisualScripting;
using UnityEngine;

public class SnapVisuals : MonoBehaviour
{
    [SerializeField] private MeshRenderer render;
    private VRInteractableManager vrInteractableManager;
    private string snapTag;


    private void Awake()
    {
        snapTag = gameObject.tag;
        
    }

    private void Start()
    {
        vrInteractableManager = GameObject.FindWithTag("VRInteractableManager").GetComponent<VRInteractableManager>();
        render.enabled = false;
    }

    private void Update()
    {

        if (snapTag == "StreetSnap")
        {
            if (vrInteractableManager.trafficIsGrabbed && gameObject.CompareTag(snapTag))
            {
                
                render.enabled = true;
            }
            else
            {
                render.enabled = false;
            }
        }
        else if (snapTag == "WaterSnap")
        {
            if (vrInteractableManager.lilypadIsGrabbed && gameObject.CompareTag(snapTag))
            {
                
                render.enabled = true;
            }
            else
            {
                render.enabled = false;
            }
        }
        
        if (gameObject.CompareTag("Untagged"))
        {
            render.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") || (other.CompareTag("MovingPlatform")))
        {
            gameObject.tag = "Untagged";
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle") || (other.CompareTag("MovingPlatform")))
        {
            gameObject.tag = snapTag;
        }
    }

}
