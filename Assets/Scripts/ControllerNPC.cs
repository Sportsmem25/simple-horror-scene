using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;

public class ControllerNPC : MonoBehaviour
{
    public Transform sitPoint;
    public NavMeshAgent agent;
    public AudioClip angryClip;
    public Animator animator;
    public float catchDistance = 1.6f;
    public GameObject gameOverPanel;
    public GameObject orderPanel;
    public GameObject safeZonePanel;
    public PostProcessVolume chasePostProcess;

    private float transitionSpeed = 1f;
    private Transform player;
    public bool isChasing = false;
    public bool isGameOver = false;
    private bool hasOrdered = false;
    private Coroutine chaseRoutine;

    private static readonly int IdleHash = Animator.StringToHash("isIdle");
    private static readonly int WalkingHash = Animator.StringToHash("isWalking");
    private static readonly int RunningHash = Animator.StringToHash("isRunning");
    private static readonly int ScreamingHash = Animator.StringToHash("isAngry");

    private void Start()
    {
        player = Camera.main.transform;
        chasePostProcess.weight = 0f;
        StartCoroutine(EnterAndOrder());
    }

    private void Update()
    {
        if(isChasing && agent != null && agent.enabled)
        {
            AvoidanceRaycast();
        } 
    }

    private void AvoidanceRaycast()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = transform.forward;
        float distance = 2f;
        float sideStep = 0.5f;
        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            if (!hit.collider.CompareTag("Player"))
            {
                Vector3 avoidDir = Vector3.Cross(Vector3.up, direction).normalized;
                Vector3 newTarget = transform.position + avoidDir * sideStep;
                NavMeshHit navHit;
                if (NavMesh.SamplePosition(newTarget, out navHit, 2f, NavMesh.AllAreas))
                {
                    agent.SetDestination(navHit.position);
                }
            }
        }
        else
            Debug.DrawRay(origin, direction * distance, Color.red);
    }

    private IEnumerator EnterAndOrder()
    {
        SetAnimationState(idle: false, walking: true, running: false, screaming: false);

        agent.SetDestination(sitPoint.position);
        while (Vector3.Distance(transform.position, sitPoint.position) > 0.5f)
        {
            yield return null;
        }
        SetAnimationState(idle: true, walking: false, running: false, screaming: false);

        if (!hasOrdered)
        {
            hasOrdered = true;
            if (orderPanel)
            {
                orderPanel.SetActive(true);
                yield return new WaitForSeconds(3f);
                orderPanel.SetActive(false);
            }
        }
    }


    public void OnHitByCoffe()
    {
        if (isChasing || isGameOver) return;
        if (chaseRoutine != null) StopCoroutine(chaseRoutine);
        chaseRoutine = StartCoroutine(AngerThenChase());
    }

    private IEnumerator AngerThenChase()
    {
        SetAnimationState(idle: false, walking: false, running: false, screaming: true);

        if (angryClip)
            AudioSource.PlayClipAtPoint(angryClip, transform.position);

        yield return new WaitForSeconds(1.5f);

        StartChasing();
    }

    private void StartChasing()
    {
        isChasing = true;

        SetAnimationState(idle: false, walking: false, running: true, screaming: false);

        ChaseManager.Instance?.StartChase();
        if (chasePostProcess) StartCoroutine(EnablePostProcess(true));
        chaseRoutine = StartCoroutine(ChasePlayer());
    }

    private IEnumerator ChasePlayer()
    {
        while (isChasing && player != null && !isGameOver)
        {
            agent.SetDestination(player.position);

            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= catchDistance)
            {
                OnCatchPlayer();
                yield break;
            }
            yield return null;
        }
    }

    private void OnCatchPlayer()
    {
        isGameOver = true;
        isChasing = false;

        agent.isStopped = true;
        ChaseManager.Instance?.StopChase();
        var playerController = player.GetComponentInParent<PlayerMovement>();
        playerController.UnlockCursor();

        if (gameOverPanel)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;

        Debug.Log("Игрок пойман! Игра остановлена.");
    }

    public void StopChasing()
    {
        if (!isChasing) return;
        isChasing = false;

        SetAnimationState(idle: true, walking: false, running: false, screaming: false);

        if (chaseRoutine != null)
            StopCoroutine(chaseRoutine);

        StartCoroutine(EnableNPC());
        if(safeZonePanel) safeZonePanel.SetActive(true);
        if (chasePostProcess) StartCoroutine(EnablePostProcess(false));
    }

    private IEnumerator EnableNPC()
    {
        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);
        Debug.Log("NPC исчез");
    }

    private IEnumerator EnablePostProcess(bool enable)
    {
        chasePostProcess.priority = 11;
        float target = enable ? 1f : 0f;
        float start = chasePostProcess.weight;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            chasePostProcess.weight = Mathf.Lerp(start, target, t);
            yield return null;
        }
        chasePostProcess.weight = target;
    }


    private void SetAnimationState(bool idle, bool walking, bool running, bool screaming)
    {
        if(!animator) return;

        animator.SetBool(IdleHash, idle);
        animator.SetBool(WalkingHash, walking);
        animator.SetBool(RunningHash, running);
        animator.SetBool(ScreamingHash, screaming);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cup"))
        {
            OnHitByCoffe();
        }
    }
}