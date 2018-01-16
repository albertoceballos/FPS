using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float cameraRotationLimit = 85f;

    private Vector3 velocity = Vector3.zero;

    private Rigidbody rb;
    private Vector3 rot = Vector3.zero;
    private float camRot = 0;
    private float currentCamRotX = 0;
    private Vector3 tF = Vector3.zero;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    //gets a movement vector from controller
    //sets velocity to that value
    public void Move(Vector3 vel)
    {
        velocity = vel;
    }

    //sets value of rot variable
    public void Rotate(Vector3 rotation)
    {
        rot = rotation;
    }

    //sets value of camRot
    public void RotateCamera(float rotation)
    {
        camRot = rotation;
    }

    //gets force vector for thruster
    public void ApplyThruster(Vector3 th)
    {
        tF = th;
    }

    //Runs every physics iteration
    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }
    //Performs movement based on velocity variable
    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            //stop rigid body from moving there if it collides with somethings on the way
            //easier to control that addForce
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        if(tF != Vector3.zero)
        {
            rb.AddForce(tF * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rot));
        if (cam != null)
        {
            //cam.transform.Rotate(-camRot);
            //Set rotation and clamp it
            currentCamRotX -= camRot;
            currentCamRotX = Mathf.Clamp(currentCamRotX, -cameraRotationLimit, cameraRotationLimit);

            //apply rotation to the transform of the player's camera
            cam.transform.localEulerAngles = new Vector3(currentCamRotX,0f,0f);
        }
    }
}
