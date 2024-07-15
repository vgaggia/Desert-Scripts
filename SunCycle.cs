/*
HOW TO USE THIS SCRIPT:

1. Attach this script to the lever object in your scene.

2. In the Unity Inspector, assign the following:
   - Sun Light: The directional light representing the sun in your scene.
   - Cycle Length: Duration of a full day/night cycle in seconds.
   - Sun Intensity: Maximum intensity of the sun light.
   - Time Slider (optional): UI Slider to display/control the time of day.
   - Lever Audio Source: AudioSource component for lever sound.
   - Lever Sound: AudioClip to play when interacting with the lever.
   - Rotation Speed: Speed multiplier for lever rotation.
   - Toggle Objects: List of GameObjects to toggle on and off when interacting.

3. Lever Setup:
   - Ensure the lever object has a Collider component for interaction.

4. Ensure the GameObject with this script has UdonBehaviour component.

This script will create a day/night cycle with the following features:
- Automatic sun movement and color/intensity changes.
- Manual time control by interacting with and rotating the lever.
- Use mode to start/stop interaction.
- Time moves backwards at -45 degrees and forwards at +45 degrees.
- Audio feedback when interacting with the lever.
- Ability to toggle other objects on/off when interacting.
- Network synchronization for multiplayer VRChat worlds.
*/

using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SunCycle : UdonSharpBehaviour
{
    public Light sunLight;
    public float cycleLength = 120f;
    public float sunIntensity = 1f;
    public Slider timeSlider;
    public AudioSource leverAudioSource;
    public AudioClip leverSound;
    public float rotationSpeed = 1f;

    [Tooltip("List of objects to toggle on and off")]
    public GameObject[] toggleObjects;

    [UdonSynced] private float _syncedTimeElapsed = 0f;
    private float _localTimeElapsed = 0f;
    private bool _isInteracting = false;
    private Vector3 _lastInteractPosition;
    private float _leverAngle = 0f;

    void Start()
    {
        if (sunLight != null)
        {
            sunLight.type = LightType.Directional;
        }
    }

    public override void Interact()
    {
        ToggleInteraction();
    }

    private void ToggleInteraction()
    {
        _isInteracting = !_isInteracting;

        if (_isInteracting)
        {
            StartInteraction();
        }
        else
        {
            StopInteraction();
        }
    }

    private void StartInteraction()
    {
        _lastInteractPosition = GetPlayerInteractPosition();

        if (leverAudioSource != null && leverSound != null)
        {
            leverAudioSource.clip = leverSound;
            leverAudioSource.Play();
        }

        foreach (GameObject toggleObject in toggleObjects)
        {
            if (toggleObject != null)
            {
                toggleObject.SetActive(!toggleObject.activeSelf);
            }
        }

        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
    }

    private void StopInteraction()
    {
        if (leverAudioSource != null)
        {
            leverAudioSource.Stop();
        }
    }

    private void Update()
    {
        if (_isInteracting && Networking.IsOwner(gameObject))
        {
            UpdateLeverRotation();
        }

        if (Networking.IsOwner(gameObject))
        {
            UpdateTime();
            RequestSerialization();
        }

        UpdateSunPosition();

        if (timeSlider != null)
        {
            timeSlider.value = _syncedTimeElapsed / cycleLength;
        }
    }

    private void UpdateLeverRotation()
    {
        Vector3 currentInteractPosition = GetPlayerInteractPosition();
        float delta = (currentInteractPosition - _lastInteractPosition).x * rotationSpeed;
        
        _leverAngle = Mathf.Clamp(_leverAngle + delta, -45f, 45f);
        transform.localRotation = Quaternion.Euler(0f, _leverAngle, 0f);

        _lastInteractPosition = currentInteractPosition;
    }

    private void UpdateTime()
    {
        float timeDirection = Mathf.Sign(_leverAngle);
        float timeSpeed = Mathf.Abs(_leverAngle) / 45f; // Normalized speed based on lever angle
        
        _localTimeElapsed += Time.deltaTime * timeDirection * timeSpeed;
        
        if (_localTimeElapsed < 0)
        {
            _localTimeElapsed += cycleLength;
        }
        else if (_localTimeElapsed >= cycleLength)
        {
            _localTimeElapsed -= cycleLength;
        }
        
        _syncedTimeElapsed = _localTimeElapsed;
    }

    private Vector3 GetPlayerInteractPosition()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (localPlayer == null) return Vector3.zero;

        if (localPlayer.IsUserInVR())
        {
            return localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand).position;
        }
        else
        {
            VRCPlayerApi.TrackingData headData = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            return headData.position + headData.rotation * Vector3.forward;
        }
    }

    private void UpdateSunPosition()
    {
        if (sunLight == null) return;

        float timeOfDay = _syncedTimeElapsed / cycleLength;
        float sunAngle = Mathf.Lerp(40f, 400f, timeOfDay) % 360f;
        sunLight.transform.rotation = Quaternion.Euler(sunAngle, 0f, 0f);

        float intensityMultiplier = Mathf.Clamp01(Mathf.Cos((sunAngle - 90f) * Mathf.Deg2Rad));
        sunLight.intensity = sunIntensity * intensityMultiplier;

        if (sunAngle >= 0f && sunAngle < 180f)
        {
            sunLight.color = Color.white;
        }
        else
        {
            sunLight.color = Color.Lerp(Color.white, new Color(0.6f, 0.6f, 1f), (sunAngle - 180f) / 180f);
        }
    }

    public override void OnDeserialization()
    {
        _localTimeElapsed = _syncedTimeElapsed;
    }
}