using System.Collections;
using UnityEngine;

public class CoffeMachine : MonoBehaviour
{
    public Transform cupSlot;
    public AudioClip fillSound;
    public ParticleSystem psCoffe;
    public float fillTime = 2f;
    private bool busy = false;
    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public bool TryStartFill(GameObject cupObj, System.Action onFinished)
    {
        if (busy) return false;

        var cup = cupObj.GetComponent<Cup>();
        if (cup == null || cup.isFilled) return false;
        StartCoroutine(FillCoroutine(cup, onFinished));
        return true;
    }

    private IEnumerator FillCoroutine(Cup cup, System.Action onFinished)
    {
        busy = true;
        if (rb) rb.isKinematic = true;
        if (col) col.enabled = false;

        cup.transform.SetParent(cupSlot, false);
        cup.transform.localPosition = Vector3.zero;
        cup.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

        // Play sound
        if (fillSound) AudioSource.PlayClipAtPoint(fillSound, transform.position);
        if (psCoffe != null)
        {
            psCoffe.Play();
        }

        yield return new WaitForSeconds(fillTime);

        cup.Fill();
        psCoffe.Stop();

        if (col) col.enabled = true;

        busy = false;
        onFinished?.Invoke();
    }
}
