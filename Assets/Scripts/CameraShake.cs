using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Coroutine shakeCorutine;
    private Vector3 originalPos;

    private void Awake()
    {
        originalPos = transform.localPosition;
    }

    public void StartShake(float intensity, float duration)
    {
        if (shakeCorutine != null) StopCoroutine(shakeCorutine);
        shakeCorutine = StartCoroutine(Shake(intensity, duration));
    }

    public void StopShake()
    {
        if (shakeCorutine != null) StopCoroutine(shakeCorutine);
        transform.localPosition = originalPos;
    }

    private IEnumerator Shake(float intensity, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            float x = (Random.value * 2 - 1) * intensity * 0.1f;
            float y = (Random.value * 2 - 1) * intensity * 0.1f;
            transform.localPosition = originalPos + new Vector3(x, y, 0);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}