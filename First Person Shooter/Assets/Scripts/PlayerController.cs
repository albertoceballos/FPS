using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    [SerializeField] //This makes it show in the inspector
    private float speed = 5f;
    [SerializeField]
    private float lookingSensitivity = 3f;
    [SerializeField]
    private float thrusterForce = 1000f;

    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;
    [SerializeField]
    private float thrusterFuelReplenishSpeed = 0.3f;

    private float thrusterFuelAmount = 1f;

    [SerializeField]
    LayerMask environment;

    [Header("Spring settings:")]
    [SerializeField]
    private float jointSpring=20f;
    [SerializeField]
    private float jointMaxForce = 40f;
    

    private Animator animator;
    private PlayerMotor motor;
    private ConfigurableJoint joint;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();
        SetJointSettings(jointSpring);
    }

    private void Update()
    {
        //Setting target position for spring
        //this makes the physics act right when it comes to 
        //applying gravity when flying over objects
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down,out hit, 100f,environment))
        {
            joint.targetPosition = new Vector3(0f,-hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }

        //Calculate movement velocity as a 3D vector
        //get axis raw is necessary to avoid extra computations
        float xMove = Input.GetAxis("Horizontal");
        //the values are between -1 and 1
        float zMove = Input.GetAxis("Vertical");

        //Animation movement
        animator.SetFloat("ForwardVelocity", zMove);
        //transform.right takes into consideration current position
        Vector3 movHorizontal = transform.right * xMove;
        Vector3 movVertical = transform.forward * zMove;
        //calculate velocity
        //final movement vector
        Vector3 velocity = (movHorizontal + movVertical).normalized * speed;

        //apply movement
        motor.Move(velocity);

        //calculate rotation as 3D vector
        // turning around the x-axis in the game which is the y-axis in the calculations
        //the y-axis is right and left which will enable to look round
        float yRot = Input.GetAxisRaw("Mouse X");
        //creates vector to rotate to
        Vector3 rotation = new Vector3(0f, yRot, 0f) * lookingSensitivity;

        //Apply rotation
        motor.Rotate(rotation);

        //The x-axis is up and down which will enable to aim
        //calculate camera rotation as a 3D vector
        float xRot = Input.GetAxisRaw("Mouse Y");

        //creates a vector the camera will rotate to
        float cameraRotation = xRot * lookingSensitivity;

        //Apply camera rotation
        motor.RotateCamera(cameraRotation);

        //initialize a value for thruster force
        //default is zero so it doesn't add the force
        Vector3 _thrusterForce = Vector3.zero;

        //If Jump button is pressed then change the value of thrusterforce
        //the value is using vector3 up
        if (Input.GetButton("Jump") && thrusterFuelAmount >0)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if(thrusterFuelAmount >= 0.01f)
            {
                _thrusterForce = Vector3.up * thrusterForce;
                SetJointSettings(0f);
            }
            
        }
        else
        {
            thrusterFuelAmount += thrusterFuelReplenishSpeed * Time.deltaTime;
            SetJointSettings(jointSpring);
        }
        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);
        //Apply Thruster Force
        motor.ApplyThruster(_thrusterForce);

    }

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive {
            positionSpring = _jointSpring,
            maximumForce= jointMaxForce};
    }
}
