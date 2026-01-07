using UnityEngine;

public class LampSwing : MonoBehaviour
{
    [Header("Swing Settings")]
    [Tooltip("The maximum angle the lamp will swing to the left and right.")]
    private float angleLimit = 30f;

    [Tooltip("How fast the lamp swings.")]
    private float speed = 1.5f;

    [Header("Axis")]
    [Tooltip("The axis to rotate around. Set Z to 1 for 2D style swinging.")]
    private Vector3 swingAxis = new Vector3(0, 0, 1);
    
    private Quaternion _startRotation;
    private float _randomOffset;

    void Start()
    {
        // Save the initial rotation so the swing is relative to how you placed it
        _startRotation = transform.localRotation;
        _randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // Calculate the smooth sine wave value (-1 to 1)
        float sineValue = Mathf.Sin((Time.time + _randomOffset) * speed);

        // Calculate the angle based on the limit
        float angle = sineValue * angleLimit;

        // Apply rotation relative to the starting position
        transform.localRotation = _startRotation * Quaternion.AngleAxis(angle, swingAxis);
    }
}