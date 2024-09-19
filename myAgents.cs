using System.Data.SqlTypes;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
public class myAgent : Agent
{
    public Transform Target;
    private Rigidbody rb;
    public float jumpForce = 5f;
    public float moveSpeed = 3f; // Horizontal movement speed during jump
    public float fallforce = 1f;
    private bool isGrounded;
    public const float jumpcooldown = 6;
    [Header("RESULTS")]
    public float lastJumpTime;
    public int targets_reached = 0;

    float currentPos;
private void  Start()
{
    float x  = transform.localPosition.x - Target.position.x;
    float y  = transform.localPosition.y - Target.position.y;
    currentPos = Mathf.Sqrt(x + y);
}

void Update()
{
    
}
    public override void Initialize()
    {
        Target.transform.localPosition = new Vector3(Random.Range(-2.15f, 2.0f), 0.41f, 6);
        transform.localPosition = new Vector3(Random.Range(-1.76f, 1.70f), 1, -0.8f);
        rb = GetComponent<Rigidbody>();
        lastJumpTime -= jumpcooldown;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition); // Agent's position
        sensor.AddObservation(Target.transform.localPosition); // Target's position
        sensor.AddObservation(isGrounded); // Ground state
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int jumpAction = actions.DiscreteActions[0]; 
        int directionAction = actions.DiscreteActions[1]; // Direction action (0 = no movement, 1 = forward, 2 = backward, 3 = left, 4 = right)

        if (jumpAction == 1 && isGrounded && Time.time - lastJumpTime > jumpcooldown)
        {
            Jump(directionAction); 
        }
        //!Reward logic
        float distanceToTarget = Vector3.Distance(transform.localPosition, Target.localPosition);
        // if(distanceToTarget < currentPod.magnitude)
        // {
        //         SetReward(0.5f);
        //         currentPod = transform.localPosition;

        // }
        if(currentPos > distanceToTarget) {
            SetReward(0.1f);
            float x = transform.localPosition.x - Target.position.x;
            float y  = transform.localPosition.y - Target.position.y;
            currentPos = Mathf.Sqrt(x + y);
        }
        

        if (distanceToTarget < 2.0f)
        {
            SetReward(1.0f);
            EndEpisode();
            Initialize();
        }
        if (distanceToTarget > 50.0f)
        {
            SetReward(-1.0f);
             EndEpisode();
             Initialize();
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
