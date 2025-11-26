using UnityEngine;

public class Lid : MonoBehaviour, IPickable
{
    public GameObject lidModel;
    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public GameObject OnPick(Transform player, Transform holdPoint)
    {
        transform.SetParent(holdPoint, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        rb.isKinematic = true;
        col.enabled = false;

        return gameObject;
    }

    public void AttachToCup(Cup cup)
    {
        cup.Attachlid(this);
        SetPhysics(false);
    }

    public void SetPhysics(bool active)
    {
        rb.isKinematic = !active;
        col.enabled = active;
    }
}
