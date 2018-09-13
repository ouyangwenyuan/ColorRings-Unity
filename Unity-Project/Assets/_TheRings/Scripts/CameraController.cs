using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    // How long the camera shaking.
    public float shakeDuration = 0.1f;
    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.2f;
    public float decreaseFactor = 0.3f;

    private Vector3 originalPos;
    private float currentShakeDuration;

    void Start()
    {
        originalPos = transform.localPosition;        
    }
    public void ShakeCamera()
    {
        StartCoroutine(Shake());
    }
    IEnumerator Shake()
    {
        currentShakeDuration = shakeDuration;
        while (currentShakeDuration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            currentShakeDuration -= Time.deltaTime * decreaseFactor;
            yield return null;
        }
        transform.localPosition = originalPos;
    }

}
