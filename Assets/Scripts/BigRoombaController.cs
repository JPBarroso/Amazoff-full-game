using FMOD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BigRoombaController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Vector3 heading;
    public bool hasHeading = false;
    public float maxSpeed = 1f;
    public float speed = 1f;
    public float speedMultiplier = 2f;
    private Transform target;
    public float killDst = 1f;

    public float viewRadius = 1f;
    [Range(0, 360)]
    public float viewAngle = 1f;

    public float viewRadius2 = 1f;
    [Range(0, 360)]
    public float viewAngle2 = 1f;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    FMOD.Studio.EventInstance stomping;

    FMOD.Studio.PLAYBACK_STATE pbs;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        stomping = FMODUnity.RuntimeManager.CreateInstance("event:/Robits/Driving");

    }

    // Update is called once per frame
    void Update()
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(stomping, GetComponent<Transform>(), GetComponent<Rigidbody>());
        playsound();
        killPlayer();
        if (!hasHeading)
            GetHeading();
        else
        {
            FindVisibleTargets();
            FindVisibleTargets2();
            agent.SetDestination(heading);
            agent.speed = speed;
            if (distance(heading) < 0.1)
                hasHeading = false;
        }
    }

    float distance(Vector3 vector3)
    {
        return Mathf.Sqrt(Mathf.Pow(transform.position.x - vector3.x, 2f) + Mathf.Pow(transform.position.z - vector3.z, 2f));
    }
    void GetHeading()
    {
        heading = RandomNavmeshLocation(120);
        speed = maxSpeed;
        hasHeading = true;
    }

    Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));

    }

    void FindVisibleTargets()
    {
        Collider[] targetPlayer = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        for(int i = 0; i < targetPlayer.Length; i++)
        {
            Transform player = targetPlayer[i].transform;
            Vector3 dirToTarget = (player.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if(!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    heading = target.position;
                    speed = speedMultiplier;
                }
            }
        }
    }

    void FindVisibleTargets2()
    {
        Collider[] targetPlayer = Physics.OverlapSphere(transform.position, viewRadius2, targetMask);
        for (int i = 0; i < targetPlayer.Length; i++)
        {
            Transform player = targetPlayer[i].transform;
            Vector3 dirToTarget = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle2 / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    heading = target.position;
                    speed = speedMultiplier;
                }
            }
        }
    }

    void killPlayer()
    {
        Vector3 p = new Vector3(target.position.x, 0, target.position.z);
        if(distance(p) < killDst)
        {
            stomping.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Scene scene = SceneManager.GetActiveScene();
            print("killed player");
            SceneManager.LoadScene(scene.path);
        }
    }
    void playsound()
    {
        stomping.getPlaybackState(out pbs);
        if (pbs == FMOD.Studio.PLAYBACK_STATE.STOPPED)
        {
            stomping.start();
        }
    }
}

