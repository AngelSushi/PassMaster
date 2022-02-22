using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour {

    public static CameraEffects Instance { get; private set;}

    void Awake() {
        Instance = this;
    }

    public IEnumerator Shake(float duration, float magnitude) {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)  {

            transform.localPosition = originalPos + Random.insideUnitSphere * magnitude;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
    
}
