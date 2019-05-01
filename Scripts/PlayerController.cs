using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 1.5f;
    [SerializeField]
    private float thrusterForce = 1000f;

    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;
    [SerializeField]
    private float thrusterFuelRegenSped = 0.3f;
    private float thrusterFuelAmount = 1f;

    public float getThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }

    [SerializeField]
    private LayerMask environmentMask;

    [Header("Spring settings:")]
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    //Component caching
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    void Start ()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        SetJointSettings(jointSpring);
    }

    void Update()
    {
        //Setting target position for spring
        //this makes the physics right when it comes to
        //applying gravity when flying over objects
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f, environmentMask))
        {
            joint.targetPosition = new Vector3(0f, -hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }

        //Calculate movement velocity as a 3D vector
        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");

        Vector3 movHorizontal = transform.right * xMov;
        Vector3 movVertical = transform.forward * zMov;

        //Final moveement vector
        Vector3 velocity = (movHorizontal + movVertical) * speed;

        //Animate movement

        animator.SetFloat("ForwardVelocity", zMov);

        //Apply movement
        motor.Move(velocity);

        //Calculate rotation as a 3D vector (turning around)
        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;

        //Apply rotation
        motor.Rotate(rotation);

        //Calculate camera rotation as a 3D vector (turning around)
        float xRot = Input.GetAxisRaw("Mouse Y");

        float cameraRotationX = xRot * lookSensitivity;

        //Apply camera rotation
        motor.RotateCamera(cameraRotationX);

        //Calculate the thrusterforce based on player input
        Vector3 _thrusterForce = Vector3.zero;
        if (Input.GetButton("Jump") && thrusterFuelAmount > 0f)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if (thrusterFuelAmount >= 0.01f)
            {             
                _thrusterForce = Vector3.up * thrusterForce;
                SetJointSettings(0f);
            }
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSped * Time.deltaTime;

            SetJointSettings(jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

        //Apply the thruster force
        motor.ApplyThruster(_thrusterForce);
    }

    private void SetJointSettings (float _jointSpring)
    {
        joint.yDrive = new JointDrive {
        positionSpring = _jointSpring, 
        maximumForce = jointMaxForce
        };
    }
}
