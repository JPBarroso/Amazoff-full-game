using FMOD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class RoombaController : MonoBehaviour
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

    FMOD.Studio.EventInstance driving;
    FMOD.Studio.EventDescription Chase;
    FMOD.Studio.PARAMETER_DESCRIPTION Chs;
    FMOD.Studio.PARAMETER_ID CID;

    FMOD.Studio.EventInstance danger;
    FMOD.Studio.EventDescription safe;
    FMOD.Studio.PARAMETER_DESCRIPTION sfe;
    FMOD.Studio.PARAMETER_ID SID;

    FMOD.Studio.EventInstance speaking;
    FMOD.Studio.EventInstance death;
    FMOD.Studio.EventDescription spotted;
    FMOD.Studio.PARAMETER_DESCRIPTION spt;
    FMOD.Studio.PARAMETER_ID spot;

    FMOD.Studio.PLAYBACK_STATE pbs;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        driving = FMODUnity.RuntimeManager.CreateInstance("event:/Robits/Driving");
        danger = FMODUnity.RuntimeManager.CreateInstance("event:/misc/music/chase");
        death = FMODUnity.RuntimeManager.CreateInstance("event:/player/Death");

        speaking = FMODUnity.RuntimeManager.CreateInstance("event:/Robits/speaking");
        spotted = FMODUnity.RuntimeManager.GetEventDescription("event:/Robits/speaking");
        spotted.getParameterDescriptionByName("Spotted", out spt);
        spot = spt.id;

        safe = FMODUnity.RuntimeManager.GetEventDescription("event:/misc/music/chase");
        safe.getParameterDescriptionByName("safe", out sfe);
        SID = sfe.id;

        Chase = FMODUnity.RuntimeManager.GetEventDescription("event:/Robits/Driving");
        Chase.getParameterDescriptionByName("Chase", out Chs);
        CID = Chs.id;
    }

    // Update is called once per frame
    void Update()
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(driving, GetComponent<Transform>(), GetComponent<Rigidbody>());
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(danger, GetComponent<Transform>(), GetComponent<Rigidbody>());
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(death, GetComponent<Transform>(), GetComponent<Rigidbody>());
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(speaking, GetComponent<Transform>(), GetComponent<Rigidbody>());
        playsound(speed, speedMultiplier);
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
                    danger.setParameterByID(SID, 0f);
                    speaking.setParameterByID(spot, 1f);
                }
                else
                {
                    danger.setParameterByID(SID, 1f);
                    speaking.setParameterByID(spot, 4f);
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
            death.start();
            driving.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            danger.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Scene scene = SceneManager.GetActiveScene();
            print("killed player");
            SceneManager.LoadScene(scene.path);
        }
    }
    void playsound(float speed, float speedMultiplier)
    {
        driving.getPlaybackState(out pbs);
        if (pbs == FMOD.Studio.PLAYBACK_STATE.STOPPED)
        {
            danger.setParameterByID(SID, 1f);
            speaking.setParameterByID(spot, 0f);
            speaking.start();
            danger.start();
            driving.start();
        }

        if (speed == speedMultiplier)
        {
            driving.setParameterByID(CID, 1f);
            print(CID);
        }
        else
        {
            driving.setParameterByID(CID, 0f);
            print(CID);
        }
    }
}

