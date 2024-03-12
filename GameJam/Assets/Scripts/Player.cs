using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class Player : MonoBehaviour
{

    public event System.Action OnReachedEndOfLevel;
    #region Local Variable
    [Header("BasicStat")]
    [SerializeField]
    private float moveSpeed = 3.5f;
    [SerializeField]
    private float smoothMoveTime = .1f;
    [SerializeField]
    private float turnSpeed = 8;
    [SerializeField]
    private GameObject particle;
    #endregion

    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;
    Vector3 velocity;
    new Rigidbody rigidbody;
    bool disabled;
    NavMeshAgent agent;
    RaycastHit[] hits=new RaycastHit[1];


    private void OnEnable()
    {
        GetComponent<Renderer>().enabled = true;
        rigidbody = GetComponent<Rigidbody>();
        agent=GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        Guard.OnGuardHasSpottedPlayer += Disable;
    }

    void Update()
    {
        OnMouseClick();
    }

    public void ResetSpeed() { agent.speed = moveSpeed; }
    public void SpeedUp(float boost) { agent.speed += boost; }

    void OnMouseClick()
    {
        if (Input.GetMouseButtonDown(0)&&!disabled)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.RaycastNonAlloc(ray, hits) > 0)
            {
                agent.SetDestination(hits[0].point);
            }
        }
    }

    void Move()
    {
        Vector3 inputDirection = Vector3.zero;
        if (!disabled)
        {
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
    }

    void OnTriggerEnter(Collider hitCollider)
    {
        if (hitCollider.tag == "Finish")
        {
            Disable();
            if (OnReachedEndOfLevel != null)
            {
                OnReachedEndOfLevel();
            }
        }
    }

    void Disable()
    {
        if (!disabled)
        {
            GetComponent<Renderer>().enabled = false;
            GameObject vfx = Instantiate(particle, transform.position, Quaternion.identity);
            Destroy(vfx, 3);
            disabled = true;
        }

    }

    void FixedUpdate()
    {
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);
    }

    void OnDestroy()
    {
        Guard.OnGuardHasSpottedPlayer -= Disable;
    }
}
