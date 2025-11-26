using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject heldObject;
    public Transform holdPoint;
    public LayerMask mask;
    public float pickRange;
    private GameObject lookedAtObject;
    private new Camera camera;
    private UIManager uiManager;
    private Ray ray;
    private RaycastHit hit;

    private IPickable lookedAtPickable;
    private CoffeMachine lookedAtMachine;
    private Cup lookedAtCup;
    private Lid heldlid;
    private Cup cup;

    private void Awake()
    {
        camera = Camera.main;
    }

    private void Start()
    {
        uiManager = UIManager.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
                TryPick();
            else
                TryUseOrDrop();
        }
        UpdateLook();

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Raycast hit: " + hit.collider);
        }
    }

    private void TryPick()
    {
        if (!RaycastForward()) return;
        lookedAtPickable = hit.collider.GetComponent<IPickable>();
        if (lookedAtPickable != null)
        {
            heldObject = lookedAtPickable.OnPick(transform, holdPoint);
            CacheHeldComponents();
        }
    }

    private void TryUseOrDrop()
    {
        if (!RaycastForward())
        {
            Drop();
            return;
        }

        lookedAtMachine = hit.collider.GetComponent<CoffeMachine>();
        Cup cup = heldObject != null ? heldObject.GetComponent<Cup>() : null;
        if (lookedAtMachine != null && cup != null && !cup.isFilled)
        {
            SetHeldObject(null);

            bool started = lookedAtMachine.TryStartFill(cup.gameObject, () =>
            {
                SetHeldObject(cup.gameObject);
            });
            lookedAtCup = hit.collider.GetComponent<Cup>();
        }

        Lid lid = heldObject != null ? heldObject.GetComponent<Lid>() : null;
        if (lid != null && lookedAtCup != null)
        {
            SetHeldObject(null);
            lid.AttachToCup(lookedAtCup);
            return;
        }

        Drop();
    }

    private void Drop()
    {
        if (heldObject == null) return;

        heldObject.transform.SetParent(null);
        var rb = heldObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        var col = heldObject.GetComponent<Collider>();
        if (col != null)
            col.enabled = true;
        heldObject = null;
        cup = null;
        heldlid = null;
    }


    private void UpdateLook()
    {
        if (!RaycastForward())
        {
            if (lookedAtObject != null)
            {
                uiManager.HideInteractionHint();
                lookedAtObject = null;
            }
            return;
        }

        GameObject target = hit.collider.gameObject;
        if (target != lookedAtObject)
        {
            lookedAtObject = target;
            lookedAtPickable = target.GetComponent<IPickable>();
            lookedAtCup = target.GetComponent<Cup>();
            lookedAtMachine = target.GetComponent<CoffeMachine>();

            if (lookedAtPickable != null && heldObject == null)
                uiManager.ShowInteraction($"Нажмите [E], чтобы взять {target.name}");
            else if (lookedAtMachine != null && cup != null && !cup.isFilled)
                uiManager.ShowInteraction("Нажмите [E], чтобы налить кофе");
            else if (heldlid != null && lookedAtCup != null)
                uiManager.ShowInteraction("Нажмите [E], чтобы надеть крышку");
            else
                uiManager.HideInteractionHint();
        }
    }

    private bool RaycastForward()
    {
        ray.origin = camera.transform.position;
        ray.direction = camera.transform.forward;
        return Physics.Raycast(ray, out hit, pickRange, mask);
    }

    public void SetHeldObject(GameObject go)
    {
        heldObject = go;
        CacheHeldComponents();
        if (go != null)
        {
            go.transform.SetParent(holdPoint, false);
            var rb = go.GetComponent<Rigidbody>();
            if (rb)
                rb.isKinematic = true;
        }
    }

    private void CacheHeldComponents()
    {
        cup = heldObject ? heldObject.GetComponent<Cup>() : null;
        heldlid = heldObject ? heldObject.GetComponent<Lid>() : null;
    }

    public GameObject GetHeldObject() => heldObject;
}