using UnityEngine;
using System.Collections;

public class Guard : MonoBehaviour {
	public static event System.Action OnGuardHasSpottedPlayer;

    #region localVariable
    [Header("BasicStat")]
    [SerializeField]
    private float speed = 5;
    [SerializeField]
    private float waitTime = .3f;
    [SerializeField]
    private float turnSpeed = 90;
    [SerializeField]
    private float timeToSpotPlayer = .5f;
	[SerializeField]
	private LayerMask viewMask;
	[SerializeField]
	private float viewDistance;
	[SerializeField]
	private Transform pathHolder;
    #endregion

    #region CacheReference
    float viewAngle;
    float playerVisibleTimer;

    Light spotlight;
    Transform player;
    Color originalSpotlightColour;
    #endregion

    private void Awake() { Init(); }
    

    void Start() {
		StartCoroutine (FollowPath (CalculateWayPoints()));
	}

    void Update()
    {
        if (CanSeePlayer())
        {
            playerVisibleTimer += Time.deltaTime;
        }
        else
        {
            playerVisibleTimer -= Time.deltaTime;
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
        spotlight.color = Color.Lerp(originalSpotlightColour, Color.red, playerVisibleTimer / timeToSpotPlayer);

        if (playerVisibleTimer >= timeToSpotPlayer)
        {
            if (OnGuardHasSpottedPlayer != null)
            {
                OnGuardHasSpottedPlayer();
            }
        }
    }

    void Init()
    {
        spotlight = GetComponentInChildren<Light>();
        player = GameObject.FindGameObjectWithTag("Detective").transform;

        viewAngle = spotlight.spotAngle;
        originalSpotlightColour = spotlight.color;

    }

    Vector3[] CalculateWayPoints()
	{
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        return waypoints;
	}

	bool CanSeePlayer() {
		if (Vector3.Distance(transform.position,player.position) < viewDistance) {
			Vector3 dirToPlayer = (player.position - transform.position).normalized;
			float angleBetweenGuardAndPlayer = Vector3.Angle (transform.forward, dirToPlayer);
			if (angleBetweenGuardAndPlayer < viewAngle / 2f) {
				if (!Physics.Linecast (transform.position, player.position, viewMask)) {
                    Debug.Log("Dead");
					return true;
				}
			}
		}
		return false;
	}

    #region Following Path or waypoints
    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];

        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }
            yield return null;
        }
    }
    #endregion


    #region RotationWhileTuringTowardsangles
    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }
    #endregion

    #region TestPurpose
    void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;

        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
    #endregion
}
