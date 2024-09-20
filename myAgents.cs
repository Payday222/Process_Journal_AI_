using System;
using System.Data.SqlTypes;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.IO;
public class myAgent : Agent
{
        [Header("RESULTS")]
    public float lastJumpTime;
    public int targets_reached = 0;
    public int targets_lost = 0;
    public int total_runs = 0;
    public float accuracy;
    public string run_ID;

        [Header("VARIABLES")]
    public Transform Target;
    private Rigidbody rb;
    public float jumpForce = 4f;
    public float moveSpeed = 2f;
    public float fallforce = 1f;
    private bool isGrounded;
    public float jumpcooldown = 1;
    float currentPos;
private void  Start()
{
    float x  = Target.localPosition.x - transform.localPosition.x;
    float y  = Target.localPosition.y - transform.position.y;
    currentPos = Mathf.Sqrt((x*x) + (y*y));
    run_ID = GetRunIDFromArgs();
}

void Update()
{
    if(total_runs != 0) {
    accuracy = targets_reached / total_runs;
    }
}
 private string GetRunIDFromArgs()
    {
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--run-id" && i + 1 < args.Length)
            {
                return args[i + 1];
            }
        }
        return null;
    }
    public override void Initialize()
    {
        Target.transform.localPosition = new Vector3(UnityEngine.Random.Range(-2.15f, 2.0f), 0.41f, 6);
        transform.localPosition = new Vector3(UnityEngine.Random.Range(-1.76f, 1.70f), 1, -0.8f);
        rb = GetComponent<Rigidbody>();
        lastJumpTime -= jumpcooldown;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(Target.transform.localPosition); 
        sensor.AddObservation(isGrounded); 
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int jumpAction = actions.DiscreteActions[0]; 
        int directionAction = actions.DiscreteActions[1]; 

        if (jumpAction == 1 && isGrounded && Time.time - lastJumpTime > jumpcooldown)
        {
            Jump(directionAction); 
        }
        //!Reward logic
        float distanceToTarget = Vector3.Distance(transform.localPosition, Target.localPosition);
        if(currentPos > distanceToTarget) {
            SetReward(0.1f);
            float x  = Target.localPosition.x - transform.localPosition.x;
            float y  = Target.localPosition.y - transform.position.y;
            currentPos = Mathf.Sqrt((x*x) + (y*y));
        }
        

        if (distanceToTarget < 2.0f)
        {
            SetReward(1.0f);
            EndEpisode();
            Initialize();
            targets_reached++;
            total_runs++;
        }
        if (distanceToTarget > 50.0f)
        {
            SetReward(-1.0f);
             EndEpisode();
             Initialize();
             targets_lost++;
             total_runs++;

        }
    }

        private void Jump(int directionAction)
        {
        Vector3 moveDirection = Vector3.zero; 

        
        switch (directionAction)
        {
            case 1:
            Debug.Log($"f");
                moveDirection = Vector3.forward; 
                break;
            case 2:
            Debug.Log($"b");
                moveDirection = Vector3.back; 
                break;
            case 3:
            Debug.Log($"l");
                moveDirection = Vector3.left; 
                break;
            case 4:
            Debug.Log($"r");
                moveDirection = Vector3.right; 
                break;
            default:
            Debug.Log($"0");
                moveDirection = Vector3.zero; 
                break;
        }

        rb.AddForce((Vector3.up * jumpForce) + (moveDirection * moveSpeed), ForceMode.Impulse);
        isGrounded = false;
        lastJumpTime = Time.time;
    }

    private void FixedUpdate()
    {
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                Debug.Log($"grounded");
                isGrounded = true;
            }
        }
        else
        {
            isGrounded = false;
        }
    }
}
