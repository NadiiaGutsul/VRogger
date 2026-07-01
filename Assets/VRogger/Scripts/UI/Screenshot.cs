using NUnit.Framework;
using UnityEngine;
using RangeAttribute = UnityEngine.RangeAttribute;
using UnityEngine.InputSystem;

public class Screenshot : MonoBehaviour
{
    [SerializeField]
    private string path;
    [SerializeField]
    [Range(1, 5)]
    private int size = 1;

    void Update()
    {
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            path += "screenshot ";
            path += System.Guid.NewGuid().ToString() + ".png";

            ScreenCapture.CaptureScreenshot(path, size);
        }        
    }
}
