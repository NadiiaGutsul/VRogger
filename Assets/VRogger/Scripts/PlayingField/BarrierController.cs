using System;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{

    private void OnTriggerExit(Collider other) {
        if (other.tag.Equals("Obstacle") || other.tag.Equals("MovingPlatform")) {
            
            Destroy(other.gameObject);
        }

        if (other.tag.Equals("Tile"))
        {
            Destroy(other.gameObject.transform.parent.gameObject);
        }
    }

    // Enable renderers for moving objects and pre-loaded tiles
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Tile") || other.CompareTag("Obstacle") || other.CompareTag("MovingPlatform"))
        {
            foreach (Renderer renderer in other.GetComponentsInChildren<Renderer>())
            {
                    renderer.enabled = true;

                    if(renderer.gameObject.CompareTag("StreetSnap") || renderer.gameObject.CompareTag("WaterSnap"))
                    {
                        renderer.enabled = false;
                    }
            }
        }
    }
}
