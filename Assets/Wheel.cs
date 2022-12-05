using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Wheel : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject wheelMesh;
    public LayerMask whatIsGround;

    public bool wheelFrontLeft;
    public bool wheelFrontRight;
    public bool wheelRearLeft;
    public bool wheelRearRight;
    [Header("Suspension")]
    public float restLegth;
    public float springTravel;
    public float springStiffness;
    public float damperStiffness;
    public float camberFront;
    public float camberRear;
    public float caster;
    public float backForceMultiplyer;
    

    private float minLength;
    private float maxLength;
    private float lastLength;
    private float springLength;
    private float springVelocity;
    private float springForce;
    private float damperForce;
    [Header("Steering")]
    public float steerAngle;
    public float steerTime;
    private bool isSteeringRight;
    private bool isSteeringLeft;
    private bool isNotSteering;

    private Vector3 suspensionForce;
    private Vector3 wheelVelocityLS; //Local Space
    private float Fx;
    private float Fy;
    private float nFy;

    private float wheelAngle;
    [Header("Wheel")]
    public float wheelRadius;
    public float motorTorque;
    public float accelerationMuliplyer;

    public float frontTrackForceReducer;
    public float minfrontTrackForceReducer;
    public float maxfrontTrackForceReducer;
    
    public float rotationAngle;

    //Brakes
    public float brakeForce;
    public float brakeCoeff;
    public float handBrakeForce;
    public float hamdBrakeCoeff;


    public AnimationCurve torqueGraph;
   [SerializeField] public Engine engine;

    public TextMeshProUGUI mTorque;
    
    public float relaxationLength;

    public float InputX;









   public Vector3 FxForce;
    public Vector3 FyForce;
    public Vector3 Force;
    public float coeff;
    public float CoeffX;
    public float CoeffY;
    public float throttleCoeff=1;
    public float tireCoeff;
    public float throttle;
    float fZ;
    Vector3 fZForce;
    float slipAngle;
    [SerializeField] float slipAnglePeak=8;
    float sX;
    float sY;
    [SerializeField] float tC;
    float targetAngularVelocity;




    void Start()
    {
        rb = transform.root.GetComponent<Rigidbody>();
       
        minLength = restLegth - springTravel;
        maxLength = restLegth + springTravel;
    }

    private void Update()
    {
       
        wheelMesh.transform.position = new Vector3(transform.position.x, transform.position.y - springLength, transform.position.z);
    
        wheelAngle = Mathf.Lerp(wheelAngle, steerAngle, steerTime * Time.deltaTime);

      //  Debug.Log(wheelAngle);
        Debug.DrawRay(transform.position, -transform.up * (springLength), Color.green);
       // Debug.DrawRay(transform.position, Fy * -transform.right, Color.red);
       
        //Debug.DrawRay(transform.position,backForceMultiplyer*  Fx * transform.forward+ Fy * -transform.right, Color.yellow);

        if (wheelFrontLeft)
        {
            transform.localRotation = Quaternion.Euler(caster, wheelAngle, -camberFront);
            wheelMesh.transform.localRotation = Quaternion.Euler(rotationAngle, wheelAngle, -camberFront);
        }
        if (wheelFrontRight)
        {
            transform.localRotation = Quaternion.Euler(caster, wheelAngle, camberFront);
            wheelMesh.transform.localRotation = Quaternion.Euler(rotationAngle, 180+wheelAngle, -camberFront);
        }


        if (wheelRearLeft)
        {
            transform.localRotation = Quaternion.Euler(0, 0, -camberRear);
            wheelMesh.transform.localRotation = Quaternion.Euler(rotationAngle, 0, -camberRear);
        }
        if (wheelRearRight)
        {
            transform.localRotation = Quaternion.Euler(0, 0, camberRear);
            wheelMesh.transform.localRotation = Quaternion.Euler(rotationAngle, 0, 180+camberRear);
        }

        //Debug.Log(Fy);


        Inputs();

        mTorque.text = "Motor torque: " + motorTorque;


    }
    void FixedUpdate()
    {

        //motorTorque = torqueGraph.Evaluate(engine.currentRpm);


        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxLength + wheelRadius, whatIsGround))
        {

            SuspensionForceCount(hit);
            WheelVelocityCount(hit);
            
            RearDrive(hit);
        }
        else springLength = restLegth;
        
    }








public void SuspensionForceCount(RaycastHit hit)
    {
        lastLength = springLength;
        springLength = hit.distance - wheelRadius;
        springLength = Mathf.Clamp(springLength, minLength, maxLength);
        springVelocity = (lastLength - springLength) / Time.fixedDeltaTime;
        springForce = springStiffness * (restLegth - springLength);
        damperForce = damperStiffness * springVelocity;

        suspensionForce = (springForce + damperForce) * transform.up;

        fZ = springForce + damperForce;
        fZForce = fZ * hit.normal.normalized;

    }

    public void ApplySuspensionForce()
    {
        rb.AddForceAtPosition(fZForce, transform.position);
    }

















    public void Inputs()
    {
        InputX = Input.GetAxis("Vertical");
    }
    public void BrakeForceCount()
    {
        brakeForce = InputX * brakeCoeff;
    }
   
    public void WheelVelocityCount(RaycastHit hit)
    {
        wheelVelocityLS = transform.InverseTransformDirection(rb.GetPointVelocity(hit.point));
       // coeff = (Mathf.Abs(wheelVelocityLS.x) / relaxationLength) * Time.fixedDeltaTime;
       // coeff = Mathf.Clamp(coeff, 0, 1);
       // wheelVelocityLS.x = (Mathf.Max(wheelVelocityLS.x) - wheelVelocityLS.x) * coeff;
        //wheelVelocityLS.x = Mathf.Clamp(wheelVelocityLS.x, -1, 1);
    }
    /* public void SlipAngleCount()
     {
         if(wheelVelocityLS.z!=0)
         slipAngle = Mathf.Atan(-wheelVelocityLS.x / Mathf.Abs(wheelVelocityLS.z))*Mathf.Rad2Deg;
         wheelVelocityLS.x = Mathf.Clamp(slipAngle / slipAnglePeak, -1, 1);

     }*/
  

    private void GetSy()
    {
        slipAngle = wheelVelocityLS.z == 0 ? 0 : Mathf.Atan(-wheelVelocityLS.x / Mathf.Abs(wheelVelocityLS.z)) * Mathf.Rad2Deg;
        sY = slipAngle / slipAnglePeak;
    }
    public void RearDrive(RaycastHit hit)
    {
       
        

        wheelVelocityLS.y = Mathf.Clamp(wheelVelocityLS.y, -1, 1);


        FxForce = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized;

        FyForce = Vector3.ProjectOnPlane(transform.right, hit.normal).normalized;


       






        if (wheelFrontLeft)
        {
            Debug.DrawRay(transform.position - transform.up * springLength, FxForce, Color.blue);
            Debug.DrawRay(transform.position - transform.up * springLength, FyForce, Color.red);
            // Debug.Log("front"+wheelVelocityLS.y);
            throttle = 0;
            rotationAngle += Mathf.Rad2Deg * wheelVelocityLS.z / wheelRadius * Time.fixedDeltaTime;
            rb.AddForceAtPosition( Force,transform.position-transform.up*springLength);
        }
        if (wheelFrontRight)
        {
            Debug.DrawRay(transform.position - transform.up * springLength, FxForce, Color.blue);
            Debug.DrawRay(transform.position - transform.up * springLength, FyForce, Color.red);
            throttle = 0;
           
            rotationAngle -= Mathf.Rad2Deg * wheelVelocityLS.z / wheelRadius * Time.fixedDeltaTime;
            rb.AddForceAtPosition(Force, transform.position - transform.up * springLength);
        }
        if (wheelRearLeft)
        {
            throttle = InputX;
            Debug.DrawRay(transform.position - transform.up * springLength, FxForce, Color.blue);
            Debug.DrawRay(transform.position - transform.up * springLength, FyForce, Color.red);
           
            // Debug.Log("rear"+wheelVelocityLS.y);
            rotationAngle += Mathf.Rad2Deg * wheelVelocityLS.z/ wheelRadius * Time.fixedDeltaTime;
            //Debug.DrawRay(transform.position, suspensionForce + Fx * transform.forward, Color.blue);
            rb.AddForceAtPosition( Force, transform.position - transform.up * springLength);
        }
        if (wheelRearRight)
        {
            throttle = InputX ;
            Debug.DrawRay(transform.position - transform.up * springLength, FxForce, Color.blue);
            Debug.DrawRay(transform.position - transform.up * springLength, FyForce, Color.red);

            rotationAngle += Mathf.Rad2Deg * wheelVelocityLS.z / wheelRadius * Time.fixedDeltaTime;
            //Debug.DrawRay(transform.position, suspensionForce + Fx * transform.forward, Color.blue);
            rb.AddForceAtPosition( Force, transform.position - transform.up * springLength);
        }
        Debug.DrawRay(transform.position, Fx * transform.forward + Fy * -transform.right, Color.black);
        //frontTrackForceReducer = Mathf.Clamp(frontTrackForceReducer, minfrontTrackForceReducer, maxfrontTrackForceReducer);
    }






}
