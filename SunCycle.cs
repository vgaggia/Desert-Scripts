using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SunCycle : UdonSharpBehaviour
{
    public Light sunLight; // Assign the sun Light component in the inspector
    public float cycleLength = 120f; // Length of the day/night cycle in seconds
    public float sunIntensity = 1f;
    public Slider timeSlider; // Assign the UI Slider component in the inspector
    
    [UdonSynced] private float _syncedTimeElapsed = 0f;
    private float _localTimeElapsed = 0f;
    private bool _isTimeLocked = false;

    private void Start()
    {
        // Ensure the sun light is set to directional
        sunLight.type = LightType.Directional;
    }

    public override void Interact()
    {
        _isTimeLocked = !_isTimeLocked;
        timeSlider.interactable = _isTimeLocked;
    }

    private void Update()
    {
        if (Networking.IsOwner(gameObject))
        {
            if (!_isTimeLocked)
            {
                _localTimeElapsed += Time.deltaTime;
                _syncedTimeElapsed = _localTimeElapsed;
                RequestSerialization();
            }
            else
            {
                _syncedTimeElapsed = timeSlider.value * cycleLength;
                RequestSerialization();
            }
        }

        // Normalize the time elapsed to the cycle length
        float timeOfDay = _syncedTimeElapsed / cycleLength;

        // Calculate the sun's rotation based on the time of day
        float sunAngle = Mathf.Lerp(40f, 400f, timeOfDay) % 360f;
        Quaternion sunRotation = Quaternion.Euler(sunAngle, 0f, 0f);

        // Adjust the sun light rotation
        sunLight.transform.rotation = sunRotation;

        // Adjust the sun light intensity based on the sun's angle
        float intensityMultiplier = Mathf.Clamp01(Mathf.Cos((sunAngle - 90f) * Mathf.Deg2Rad));
        sunLight.intensity = sunIntensity * intensityMultiplier;

        // Transition through different color stages for the sun light
        if (sunAngle >= 0f && sunAngle < 180f)
        {
            // Daytime (0 <= sunAngle < 180)
            sunLight.color = Color.white;
        }
        else
        {
            // Nighttime (180 <= sunAngle < 360)
            sunLight.color = Color.Lerp(Color.white, new Color(0.6f, 0.6f, 1f), (sunAngle - 180f) / 180f);
        }

        // Check if the cycle needs to be reset
        if (_localTimeElapsed >= cycleLength && Networking.IsOwner(gameObject) && !_isTimeLocked)
        {
            _localTimeElapsed = 0f;
            _syncedTimeElapsed = 0f;
            RequestSerialization();
        }
    }
}