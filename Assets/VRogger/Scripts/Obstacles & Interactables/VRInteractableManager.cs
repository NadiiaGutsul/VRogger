using UnityEngine;
using UnityEngine.Events;

public class VRInteractableManager : MonoBehaviour
{
    public bool trafficIsGrabbed { get; private set; }
    public bool lilypadIsGrabbed { get; private set; }
    
    public void SetTrafficGrabbed(bool value)
    {
        trafficIsGrabbed = value;
        
        Debug.Log(trafficIsGrabbed);
    }
    
    public void SetLilypadGrabbed(bool value)
    {
        lilypadIsGrabbed = value;
        
        Debug.Log(lilypadIsGrabbed);
    }


}
