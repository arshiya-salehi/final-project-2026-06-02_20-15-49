using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private float dayDurationSeconds = 180f; // measured in seconds, so 3 minutes per day
    [Range(0f, 1f)] //range so time of day only goes from 0 to 1 (used in UpdateSun)
    [SerializeField] private float timeOfDay = 0.25f; // 0=midnight, 0.25=sunrise, 0.5=noon

    [SerializeField] private Light sunLight;

    void Update()
    {
        timeOfDay += Time.deltaTime / dayDurationSeconds;
        if (timeOfDay >= 1f) timeOfDay -= 1f; // loop for continuous time

        UpdateSun();
    }

    void UpdateSun()
    {
        // 0 to 360 degrees as time of day goes from 0 to 1
        float degrees = timeOfDay * 360f;

        // offset so noon is at the top instead of sunrise
        float xRotation = degrees - 90f;

        // rotate
        sunLight.transform.eulerAngles = new Vector3(xRotation, 170f, 0f);

        // no light when sun below horizon
        sunLight.intensity = Mathf.Clamp01(
            Vector3.Dot(sunLight.transform.forward, Vector3.down)
        );
    }
}

