using UnityEngine;

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

    [Header("Spring settings:")]
    [SerializeField]
    private JointDriveMode jointMode = JointDriveMode.Position;
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    private PlayerMotor motor;
    private ConfigurableJoint joint;

    void Start ()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();

        SetJointSettings(jointSpring);
    }

    void Update()
    {
        //Calculate movement velocity as a 3D vector
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        Vector3 movHorizontal = transform.right * xMov;
        Vector3 movVertical = transform.forward * zMov;

        //Final moveement vector
        Vector3 velocity = (movHorizontal + movVertical).normalized * speed;

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
        if (Input.GetButton("Jump"))
        {
            _thrusterForce = Vector3.up * thrusterForce;
            SetJointSettings(0f);
        }
        else
        {
            SetJointSettings(jointSpring);
        }

        //Apply the thruster force
        motor.ApplyThruster(_thrusterForce);
    }

    private void SetJointSettings (float _jointSpring)
    {
        joint.yDrive = new JointDrive {
        mode = jointMode, 
        positionSpring = _jointSpring, 
        maximumForce = jointMaxForce
        };
    }
}
