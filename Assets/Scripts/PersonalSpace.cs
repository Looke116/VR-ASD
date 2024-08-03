using Normal.Realtime;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public class PersonalSpace : MonoBehaviour
{
    public GameObject controller;

    [Tooltip(
        "If you want to use A/B X/Y buttons on the controllers you might need to add the in your XRI Default Input Actions")]
    public InputActionProperty toggleOnOff;

    private CapsuleCollider _personalSpace;
    private GameObject _personalSpaceDisplay;
    private bool _isEnabled;

    void Start()
    {
        // Get reference to the object's collider
        _personalSpace = GetComponent<CapsuleCollider>();

        // Disable collision between the object's layer and the default layer and the UI layer
        int currentLayer = _personalSpace.gameObject.layer;
        Physics.IgnoreLayerCollision(currentLayer, 0);
        Physics.IgnoreLayerCollision(currentLayer, 5);

        // Check if the game object is owned by the client and disable the collision with the XR Rig
        var realtimeView = GetComponent<RealtimeView>();
        if (realtimeView.isOwnedLocallyInHierarchy)
        {
            realtimeView.RequestOwnershipOfSelfAndChildren();
            var rigCollider = GameObject.Find("XR Origin (XR Rig)").GetComponent<CharacterController>();
            Physics.IgnoreCollision(_personalSpace, rigCollider);
        }

        // // Code to disable collision with all other layers if needed
        // for (int i = 0; i < 32; i++)
        // {
        //     if (i != currentLayer) Physics.IgnoreLayerCollision(currentLayer, i);
        // }

        // Reference to the game object that is used to display the personal space
        _personalSpaceDisplay = gameObject.GetNamedChild("Display Mesh");

        // Whenever the toggle on/off action is performed call the "Toggle" local function 
        if (toggleOnOff.reference != null)
        {
            toggleOnOff.reference.action.performed += Toggle;
        }
        else
        {
            toggleOnOff.action.performed += Toggle;
        }

        return;

        // Declared local function 
        void Toggle(InputAction.CallbackContext obj)
        {
            if (realtimeView.isOwnedLocallyInHierarchy)
            {
                _isEnabled = !_isEnabled;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_isEnabled) return;

        // Get the player's position and normalize the height
        Vector3 playerPos = transform.position;
        playerPos.y = 0;

        // Cast a ray out of the controller to determine where it is pointing
        Physics.Raycast(controller.transform.position, controller.transform.forward, out var hit);

        // Check if the ray cast hit anything and save the result
        if (hit.collider == null) return;
        var hitPos = hit.point;
        hitPos.y = 0;

        // Calculate the distance
        float distance = Vector3.Distance(playerPos, hitPos);

        // Clamp the size
        if (distance > 3.5f)
        {
            distance = 3.5f;
        }

        // Update the personal space
        _personalSpace.radius = distance;
        _personalSpaceDisplay.transform.localScale = new Vector3(distance * 2, 0.5f, distance * 2);
    }
}