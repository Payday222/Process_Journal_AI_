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
    public float lastJumpTime;

  public override void Initialize()
    {
        Target.transform.localPosition = new Vector3(Random.Range(-2.15f, 2.0f), 0.41f, 6);
        transform.localPosition = new Vector3(Random.Range(-1.76f, 1.70f), 1, -0.8f);
        rb = GetComponent<Rigidbody>();
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
    { rb.AddForce((Vector3.up * jumpForce), ForceMode.Impulse);
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