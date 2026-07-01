using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public struct TrackingOrigin
{
    public Vector3 position;
    public Quaternion rotation;

    public TrackingOrigin(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}

/// <summary>
/// This script must be put on the XR CameraRig gameobject.
/// For user-defined rig orientation (from HMD view direction), call <see cref="SetOrigin(UnityEngine.InputSystem.InputAction.CallbackContext)"/>. 
/// </summary>
public class XRRigOriginController : MonoBehaviour
{
    [Header("Scene settings")]

    /// <summary>
    /// Reference to the user-defined origin of the VR experience.
    /// </summary>
    [SerializeField] private Transform virtualSceneOrigin;

    /// <summary>
    /// Consider the Y coordinate of the virtual scene origin for the camera rig.
    /// </summary>
    [SerializeField] private bool useYOfVirtualSceneOrigin;

    /// <summary>
    /// XR Origin
    /// </summary>
    [SerializeField] private XROrigin xrOrigin;

    /// <summary>
    /// Input Action to reset the rig.
    /// </summary>
    [SerializeField] private InputActionReference inputActionReference;

    /// <summary>
    /// Struct for saving rig transform in the PlayerPrefs
    /// </summary>
    protected TrackingOrigin trackingOrigin;

    /// <summary>
    /// True, when the XR subsystem is running.
    /// </summary>
    internal bool XRisRunning = false;
    
    private Transform xrOriginTransform;
    
    /// <summary>
    /// HMD pose reference transform
    /// </summary>
    private Transform hmdTransform;

    /// <summary>
    /// At startup, set last saved hmd origin.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator Start()
    {
        xrOriginTransform = xrOrigin.Origin.transform;
        hmdTransform = xrOrigin.Camera.transform;

        yield return WaitForXR();
        ResetTrackingOriginMode();
        LoadTrackingOrigin();
        inputActionReference.action.performed += SetOrigin;

        yield return new WaitForSeconds(10f);
        ResetTrackingOriginMode();
    }


    /// <summary>
    /// Set virtual scene origin.
    /// Available input:
    /// Transform of <see cref="XRRigOriginController.xrOriginTransform"/>
    /// Transform of <see cref="XRRigOriginController.hmdTransform"/>
    /// Transform of <see cref="virtualSceneOrigin"/>
    /// </summary>
    /// <param name="inputContext"></param>
    public void SetOrigin(InputAction.CallbackContext inputContext)
    {
        if (!inputContext.performed)
        {
            return;
        }

        ResetTrackingOriginMode();

        xrOriginTransform.rotation *= Quaternion.FromToRotation(Vector3ProjectXZ(hmdTransform.forward).normalized, Vector3ProjectXZ(virtualSceneOrigin.forward).normalized);
        xrOriginTransform.position -= Vector3ProjectXZ(hmdTransform.position - virtualSceneOrigin.position);

        if (useYOfVirtualSceneOrigin)
        {
            xrOriginTransform.position = new Vector3(xrOriginTransform.position.x, virtualSceneOrigin.position.y, xrOriginTransform.position.z);
        }

        SaveTrackingOrigin(new TrackingOrigin(xrOriginTransform.position, xrOriginTransform.rotation));
    }

    /// <summary>
    /// Load last saved tracking origin.
    /// </summary>
    protected void LoadTrackingOrigin()
    {
        if (PlayerPrefs.GetString("OriginJSON", "") == "")
        {
            return;
        }

        trackingOrigin = JsonUtility.FromJson<TrackingOrigin>(PlayerPrefs.GetString("OriginJSON"));

        xrOriginTransform.SetPositionAndRotation(trackingOrigin.position, trackingOrigin.rotation);
    }

    /// <summary>
    /// Save tracking origin to hmd prefs.
    /// </summary>
    /// <param name="trackingOrigin"></param>
    protected void SaveTrackingOrigin(TrackingOrigin trackingOrigin)
    {
        PlayerPrefs.SetString("OriginJSON", JsonUtility.ToJson(trackingOrigin));
    }

    /// <summary>
    /// Remove y component from Vector3.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    protected static Vector3 Vector3ProjectXZ(Vector3 v) => new(v.x, 0, v.z);

    /// <summary>
    /// Wait for XR subsystem to start
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForXR()
    {
        // wait untit XR has started 
        while (!XRisRunning)
        {
            if (UnityEngine.XR.Management.XRGeneralSettings.Instance != null)
            {
                XRisRunning = UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.isInitializationComplete;
            }

            yield return null;
        }
    }

    private void ResetTrackingOriginMode()
    {
        XROrigin.TrackingOriginMode trackingOriginMode = xrOrigin.RequestedTrackingOriginMode;
        xrOrigin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Device;
        xrOrigin.RequestedTrackingOriginMode = trackingOriginMode;
    }
}

