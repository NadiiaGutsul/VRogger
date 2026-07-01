using System;
using Unity.VisualScripting;
using UnityEngine;

public class DeleteOutOfBounds : MonoBehaviour
{
    private float timeBeforeDestroy = 0.5f;
    private float currentTime = 0f;
    private bool canDestroy = false;
    private void Update()
    {
        if (!canDestroy)
        {
            Timer(); 
        };
    
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Border" && canDestroy)
        {
            Destroy(gameObject);
            
        }
    }
    
    private void Timer()
    {
        currentTime += Time.deltaTime;
    
        if (currentTime >= timeBeforeDestroy)
        {
            canDestroy = true;
        }
    }
}
