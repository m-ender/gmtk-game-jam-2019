using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Transform targetTransform;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.7f;
    private float dampingSpeed = 1.0f;
    Vector3 initialPosition;

    public void TriggerShake()
    {
        shakeDuration = 0.25f;
    }

    private void Awake()
    {
        targetTransform = GetComponent<Transform>();
    }

    private void Start()
    {
        initialPosition = targetTransform.localPosition;
    }

    private void Update()
    {
        if (shakeDuration > 0)
        {
            targetTransform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            targetTransform.localPosition = initialPosition;
        }
    }
}
