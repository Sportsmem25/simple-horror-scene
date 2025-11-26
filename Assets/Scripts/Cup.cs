using UnityEngine;

public class Cup : MonoBehaviour, IPickable
{
    public bool isFilled = false;
    public bool hasLid = false;
    public Transform lidAttachPoint;
    public Renderer liquidRenderer;
    public ParticleSystem fillParticles;
    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        if (liquidRenderer) liquidRenderer.enabled = false;
    }

    public GameObject OnPick(Transform player, Transform holdPoint)
    {
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        rb.isKinematic = true;
        col.enabled = false;
        return gameObject;
    }

    public void Fill()
    {
        if (isFilled) return;
        isFilled = true;
        if (liquidRenderer)
            liquidRenderer.enabled = true;
        Debug.Log("Cup full");
    }

    public void Attachlid(Lid lid)
    {
        if (hasLid) return;

        hasLid = true;
        lid.transform.SetParent(lidAttachPoint != null ? lidAttachPoint : null);
        lid.transform.localPosition = Vector3.zero;
        lid.transform.localRotation = Quaternion.identity;
        lid.SetPhysics(false);
        Debug.Log("Lid on the cup");
    }
}