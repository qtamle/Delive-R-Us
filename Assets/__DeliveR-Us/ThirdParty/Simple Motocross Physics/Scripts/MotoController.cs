using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMPScripts
{
    // Cycle Geometry Class - Holds Gameobjects pertaining to the specific bicycle
    [System.Serializable]
    public class MotoGeometry
    {
        public GameObject handles,
            fVisualWheel,
            fPhysicsWheel,
            rPhysicsWheel;

        [Space]
        [Header("Optional")]
        public GameObject secondaryFVisualWheel;
        public GameObject secondaryFPhysicsWheel;
    }

    [System.Serializable]
    public class EngineSettings
    {
        public int numOfGears;
        public float torque;
        public AnimationCurve accelerationCurve;
        public float topSpeed;
        public float reversingSpeed;

        [Space]
        [Header("Engine Info")]
        public int currentGear;

        [Range(0, 1)]
        public float gearRatio;
    }

    // Wheel Friction Settings Class - Uses Physics Materials and Physics functions to control the
    // static / dynamic slipping of the wheels
    [System.Serializable]
    public class WheelFrictionSettings
    {
        public PhysicsMaterial fPhysicMaterial,
            rPhysicMaterial;
        public Vector2 fFriction,
            rFriction;
    }

    [System.Serializable]
    public class AirTimeSettings
    {
        public bool freestyle;
        public float airTimeRotationSensitivity;

        [Range(0.5f, 10)]
        public float heightThreshold;
        public float groundSnapSensitivity;
    }

    public class MotoController : MonoBehaviour
    {
        public EngineSettings engineSettings;
        public MotoGeometry motoGeometry;
        public WheelFrictionSettings wheelFrictionSettings;

        [Tooltip("Steer Angle over Speed")]
        public AnimationCurve steerAngle;
        public Vector2 steerControls;

        // Defines the leaning curve of the bicycle
        public AnimationCurve leanCurve;

        [Tooltip("X: Steer Sensitivity, Y: Steer Gravity")]
        public float axisAngle;

        // The slider refers to the ratio of Relaxed mode to Top Speed.
        // Torque is a physics based function which acts as the actual wheel driving force.
        public Vector3 centerOfMassOffset;

        [HideInInspector]
        public bool isReversing,
            isAirborne,
            stuntMode;

        [HideInInspector]
        public Rigidbody rb,
            fWheelRb,
            secondaryFWheelRb,
            rWheelRb;
        float turnAngle;
        float xQuat,
            zQuat;

        [HideInInspector]
        public float turnLeanAmount;
        RaycastHit hit;

        [HideInInspector]
        public float customSteerAxis,
            customLeanAxis,
            customAccelerationAxis,
            rawCustomAccelerationAxis;
        bool isRaw;

        [HideInInspector]
        public float currentTopSpeed,
            pickUpSpeed;
        Quaternion initialLowerForkLocalRotaion,
            initialHandlesRotation;
        ConfigurableJoint fPhysicsWheelConfigJoint,
            secondaryFPhysicsWheelConfigJoint,
            rPhysicsWheelConfigJoint;
        public bool groundConformity;
        RaycastHit hitGround;
        Vector3 theRay;
        float groundZ;
        JointDrive fDrive,
            rYDrive,
            rZDrive;

        [HideInInspector]
        public Vector3 lastVelocity,
            deceleration,
            lastDeceleration;
        public AirTimeSettings airTimeSettings;
        int currGear,
            prevGear;
        bool changeGear;

        [HideInInspector]
        public bool wheelieInput;

        [HideInInspector]
        public float wheeliePower;
        public float emergencyBrakeStrength = 5f;

        public Transform GetCameraLook;
        public Transform GetCameraFollow;

        [SerializeField]
        private GasSystem gas;

        [SerializeField]
        private float gasUsePerSecMoving = 0.7f;

        [SerializeField]
        private float gasUsePerSecIdle = 0.6f;

        [SerializeField]
        private float coastingDrag = 0.8f;
        private float _origDrag;
        private bool _outOfGas;

        void Awake()
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.maxAngularVelocity = Mathf.Infinity;

            fWheelRb = motoGeometry.fPhysicsWheel.GetComponent<Rigidbody>();
            fWheelRb.maxAngularVelocity = Mathf.Infinity;

            if (motoGeometry.secondaryFVisualWheel && motoGeometry.secondaryFPhysicsWheel)
            {
                secondaryFWheelRb = motoGeometry.secondaryFPhysicsWheel.GetComponent<Rigidbody>();
                secondaryFWheelRb.maxAngularVelocity = Mathf.Infinity;
            }

            rWheelRb = motoGeometry.rPhysicsWheel.GetComponent<Rigidbody>();
            rWheelRb.maxAngularVelocity = Mathf.Infinity;

            currentTopSpeed = engineSettings.topSpeed;

            initialHandlesRotation = motoGeometry.handles.transform.localRotation;

            fPhysicsWheelConfigJoint = motoGeometry.fPhysicsWheel.GetComponent<ConfigurableJoint>();

            if (motoGeometry.secondaryFVisualWheel && motoGeometry.secondaryFPhysicsWheel)
                secondaryFPhysicsWheelConfigJoint =
                    motoGeometry.secondaryFPhysicsWheel.GetComponent<ConfigurableJoint>();

            rPhysicsWheelConfigJoint = motoGeometry.rPhysicsWheel.GetComponent<ConfigurableJoint>();

            _origDrag = rb.linearDamping;
            if (!gas)
                gas = GasSystem.Instance; // kéo bằng Inspector là tốt nhất; fallback dùng Singleton
        }

        private void HandleGas()
        {
            if (gas == null)
                return;

            // Lấy tốc độ hiện tại
            float speed = rb.linearVelocity.magnitude; // NOTE: nếu code của bạn đang dùng linearVelocity, thay bằng rb.linearVelocity.magnitude

            // Có coi là di chuyển/đạp ga?
            bool movingOrThrottle = speed > 0.1f || Mathf.Abs(rawCustomAccelerationAxis) > 0.01f;

            // Trừ xăng theo trạng thái
            float useRate = movingOrThrottle ? gasUsePerSecMoving : gasUsePerSecIdle;
            if (!gas.IsOutOfGas())
                gas.ConsumeGas(useRate * Time.fixedDeltaTime);

            // Cập nhật trạng thái hết xăng
            _outOfGas = gas.IsOutOfGas();

            // Khi hết xăng: tăng drag để xe “tà” lại; còn xăng: trả về drag gốc
            rb.linearDamping = _outOfGas ? coastingDrag : _origDrag;
        }

        void FixedUpdate()
        {
            if (_stopped)
                return;
            HandleGas();

            // Detect reversing
            isReversing = transform.InverseTransformDirection(rb.linearVelocity).z < 0;

            // FORWARD: Apply traditional front wheel rotation (physical steer)
            if (!isReversing)
            {
                float steerValue =
                    customSteerAxis * steerAngle.Evaluate(rb.linearVelocity.magnitude);

                motoGeometry.fPhysicsWheel.transform.rotation = Quaternion.Euler(
                    transform.rotation.eulerAngles.x,
                    transform.rotation.eulerAngles.y + steerValue,
                    0f
                );
                fPhysicsWheelConfigJoint.axis = new Vector3(1, 0, 0);

                if (motoGeometry.secondaryFVisualWheel && motoGeometry.secondaryFPhysicsWheel)
                {
                    motoGeometry.secondaryFPhysicsWheel.transform.rotation = Quaternion.Euler(
                        transform.rotation.eulerAngles.x,
                        transform.rotation.eulerAngles.y + steerValue,
                        0f
                    );
                    secondaryFPhysicsWheelConfigJoint.axis = new Vector3(1, 0, 0);
                }
            }
            else // REVERSE: Use torque-based turning and update visuals only
            {
                float turnStrength =
                    steerAngle.Evaluate(rb.linearVelocity.magnitude) * customSteerAxis * 0.05f;
                rb.AddTorque(Vector3.up * turnStrength, ForceMode.Acceleration);
                fPhysicsWheelConfigJoint.axis = new Vector3(0, 1, 0);

                // Visual only
                float steerValue =
                    customSteerAxis * steerAngle.Evaluate(rb.linearVelocity.magnitude);
                motoGeometry.fPhysicsWheel.transform.localRotation = Quaternion.Euler(
                    0f,
                    steerValue,
                    0f
                );

                if (motoGeometry.secondaryFVisualWheel && motoGeometry.secondaryFPhysicsWheel)
                {
                    motoGeometry.secondaryFPhysicsWheel.transform.localRotation = Quaternion.Euler(
                        0f,
                        steerValue,
                        0f
                    );
                }
            }

            //cache rb velocity
            float currentSpeed = rb.linearVelocity.magnitude;

            // Clamp max speed
            if (rb.linearVelocity.magnitude > engineSettings.topSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * engineSettings.topSpeed;

            // Emergency Brake
            if (Input.GetKey(KeyCode.Space))
            {
                rb.linearVelocity = Vector3.Lerp(
                    rb.linearVelocity,
                    Vector3.zero,
                    emergencyBrakeStrength * Time.fixedDeltaTime
                );
                rb.angularVelocity = Vector3.zero;
            }

            // Center of Mass handling
            if (stuntMode)
                rb.centerOfMass = GetComponent<BoxCollider>().center;
            else
                rb.centerOfMass =
                    centerOfMassOffset
                    + new Vector3(
                        0,
                        0,
                        rawCustomAccelerationAxis > 0 && customSteerAxis == 0
                            ? 0.5f - (engineSettings.gearRatio - 1) * 0.5f
                            : 0
                    );

            //Gear Chnaging
            if (currGear != prevGear)
                StartCoroutine(GearChange(0.15f));

            prevGear = currGear;
            currGear = Mathf.Clamp(
                Mathf.FloorToInt(
                    (currentSpeed + (engineSettings.numOfGears)) / engineSettings.numOfGears
                ),
                0,
                engineSettings.numOfGears
            );

            if (changeGear)
                engineSettings.currentGear = currGear;

            engineSettings.gearRatio =
                ((currentSpeed + (engineSettings.numOfGears)) / engineSettings.numOfGears)
                - currGear;
            engineSettings.gearRatio = Mathf.Clamp01(engineSettings.gearRatio);

            //Power Control. Wheel Torque + Acceleration curves
            if (rawCustomAccelerationAxis > 0 && !_outOfGas)
                rWheelRb.AddTorque(
                    transform.right
                        * engineSettings.torque
                        * customAccelerationAxis
                        * Mathf.Clamp01(engineSettings.currentGear)
                );

            if (rawCustomAccelerationAxis > 0 && !isAirborne && !_outOfGas)
                rb.AddForce(
                    transform.forward
                        * engineSettings.accelerationCurve.Evaluate(engineSettings.gearRatio)
                );

            if (
                currentSpeed < engineSettings.reversingSpeed
                && rawCustomAccelerationAxis < 0
                && !isAirborne
                && !_outOfGas
            )
                rb.AddForce(
                    -transform.forward
                        * engineSettings.accelerationCurve.Evaluate(customAccelerationAxis)
                        * 0.5f
                );

            if (transform.InverseTransformDirection(rb.linearVelocity).z < 0)
                isReversing = true;
            else
                isReversing = false;

            if (rawCustomAccelerationAxis < 0 && isReversing == false && !isAirborne && !_outOfGas)
                rb.AddForce(
                    -transform.forward
                        * engineSettings.accelerationCurve.Evaluate(customAccelerationAxis)
                        * 2
                );

            pickUpSpeed = Mathf.Clamp(currentSpeed * 0.2f, 0, 1);

            //Handles
            motoGeometry.handles.transform.localRotation =
                Quaternion.Euler(
                    0,
                    customSteerAxis * steerAngle.Evaluate(currentSpeed),
                    -customSteerAxis * axisAngle
                ) * initialHandlesRotation;

            //FWheelVisual
            motoGeometry.fVisualWheel.transform.position = new Vector3(
                motoGeometry.fPhysicsWheel.transform.position.x,
                motoGeometry.fPhysicsWheel.transform.position.y,
                motoGeometry.fPhysicsWheel.transform.position.z
            );
            xQuat = Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y));
            zQuat = Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.y));
            motoGeometry.fVisualWheel.transform.rotation = Quaternion.Euler(
                xQuat * (customSteerAxis * -axisAngle),
                customSteerAxis * steerAngle.Evaluate(currentSpeed),
                zQuat * (customSteerAxis * -axisAngle)
            );
            motoGeometry.fVisualWheel.transform.GetChild(0).transform.localRotation = motoGeometry
                .rPhysicsWheel
                .transform
                .rotation;

            //Secondary FWheelVisual
            if (motoGeometry.secondaryFVisualWheel && motoGeometry.secondaryFPhysicsWheel)
            {
                motoGeometry.secondaryFVisualWheel.transform.position = new Vector3(
                    motoGeometry.secondaryFPhysicsWheel.transform.position.x,
                    motoGeometry.secondaryFPhysicsWheel.transform.position.y,
                    motoGeometry.secondaryFPhysicsWheel.transform.position.z
                );
                motoGeometry.secondaryFVisualWheel.transform.rotation = Quaternion.Euler(
                    xQuat * (customSteerAxis * -axisAngle),
                    customSteerAxis * steerAngle.Evaluate(currentSpeed),
                    zQuat * (customSteerAxis * -axisAngle)
                );
                motoGeometry.secondaryFVisualWheel.transform.GetChild(0).transform.localRotation =
                    motoGeometry.rPhysicsWheel.transform.rotation;
            }

            turnLeanAmount =
                -leanCurve.Evaluate(customLeanAxis) * Mathf.Clamp(currentSpeed * 0.1f, 0, 1);

            //FrictionSettings
            wheelFrictionSettings.fPhysicMaterial.staticFriction = wheelFrictionSettings
                .fFriction
                .x;
            wheelFrictionSettings.fPhysicMaterial.dynamicFriction = wheelFrictionSettings
                .fFriction
                .y;
            wheelFrictionSettings.rPhysicMaterial.staticFriction = wheelFrictionSettings
                .rFriction
                .x;
            wheelFrictionSettings.rPhysicMaterial.dynamicFriction = wheelFrictionSettings
                .rFriction
                .y;

            if (
                Physics.Raycast(
                    motoGeometry.fPhysicsWheel.transform.position,
                    Vector3.down,
                    out hit,
                    Mathf.Infinity
                )
            )
                if (hit.distance < 0.5f)
                {
                    Vector3 velf = motoGeometry.fPhysicsWheel.transform.InverseTransformDirection(
                        fWheelRb.linearVelocity
                    );
                    velf.x *= Mathf.Clamp01(
                        1 / (wheelFrictionSettings.fFriction.x + wheelFrictionSettings.fFriction.y)
                    );
                    fWheelRb.linearVelocity =
                        motoGeometry.fPhysicsWheel.transform.TransformDirection(velf);
                }
            if (
                Physics.Raycast(
                    motoGeometry.rPhysicsWheel.transform.position,
                    Vector3.down,
                    out hit,
                    Mathf.Infinity
                )
            )
                if (hit.distance < 0.5f)
                {
                    Vector3 velr = motoGeometry.rPhysicsWheel.transform.InverseTransformDirection(
                        rWheelRb.linearVelocity
                    );
                    velr.x *= Mathf.Clamp01(
                        1 / (wheelFrictionSettings.rFriction.x + wheelFrictionSettings.rFriction.y)
                    );
                    rWheelRb.linearVelocity =
                        motoGeometry.rPhysicsWheel.transform.TransformDirection(velr);
                }

            //Secondary Wheel Friction Settings
            if (motoGeometry.secondaryFVisualWheel && motoGeometry.secondaryFPhysicsWheel)
            {
                if (
                    Physics.Raycast(
                        motoGeometry.secondaryFPhysicsWheel.transform.position,
                        Vector3.down,
                        out hit,
                        Mathf.Infinity
                    )
                )
                    if (hit.distance < 0.5f)
                    {
                        Vector3 velsf =
                            motoGeometry.secondaryFPhysicsWheel.transform.InverseTransformDirection(
                                secondaryFWheelRb.linearVelocity
                            );
                        velsf.x *= Mathf.Clamp01(
                            1
                                / (
                                    wheelFrictionSettings.fFriction.x
                                    + wheelFrictionSettings.fFriction.y
                                )
                        );
                        secondaryFWheelRb.linearVelocity =
                            motoGeometry.fPhysicsWheel.transform.TransformDirection(velsf);
                    }
            }

            //AirControl
            if (
                Physics.Raycast(
                    transform.position + new Vector3(0, 1f, 0),
                    Vector3.down,
                    out hit,
                    Mathf.Infinity
                )
            )
            {
                if (hit.distance > 1.5f)
                    isAirborne = true;
                else
                    isAirborne = false;
                // For stunts
                // 5f is the snap to ground distance
                if (hit.distance > airTimeSettings.heightThreshold && airTimeSettings.freestyle)
                {
                    stuntMode = true;
                    // Stunt + flips controls
                    // You may use Numpad Inputs as well.
                    rb.AddTorque(
                        transform.right
                            * rawCustomAccelerationAxis
                            * -3
                            * airTimeSettings.airTimeRotationSensitivity,
                        ForceMode.Impulse
                    );
                }
                else
                    stuntMode = false;
            }

            // Setting the Main Rotational movements of the bicycle
            if (airTimeSettings.freestyle)
            {
                if (!stuntMode && isAirborne)
                    transform.rotation = Quaternion.Lerp(
                        transform.rotation,
                        Quaternion.Euler(
                            0,
                            transform.rotation.eulerAngles.y,
                            turnLeanAmount + GroundConformity(groundConformity)
                        ),
                        Time.deltaTime * airTimeSettings.groundSnapSensitivity
                    );
                else if (!stuntMode && !isAirborne)
                    transform.rotation = Quaternion.Lerp(
                        transform.rotation,
                        Quaternion.Euler(
                            transform.rotation.eulerAngles.x,
                            transform.rotation.eulerAngles.y,
                            turnLeanAmount + GroundConformity(groundConformity)
                        ),
                        Time.deltaTime * 10 * airTimeSettings.groundSnapSensitivity
                    );
            }
            else
            {
                transform.rotation = Quaternion.Euler(
                    transform.rotation.eulerAngles.x,
                    transform.rotation.eulerAngles.y,
                    turnLeanAmount + GroundConformity(groundConformity)
                );
            }

            //Impact sensing
            deceleration = (fWheelRb.linearVelocity - lastVelocity) / Time.fixedDeltaTime;
            lastVelocity = fWheelRb.linearVelocity;
            lastDeceleration = deceleration;

            //Wheelie
            //rb.centerOfMass = centerOfMassOffset + new Vector3(0,-wheelieFactor,-wheelieFactor*2);
            if (!isAirborne && wheelieInput && rawCustomAccelerationAxis > 0)
            {
                rb.angularDamping = 15;
                wheeliePower = customAccelerationAxis * 150;
                var rot = Quaternion.FromToRotation(
                    transform.forward,
                    new Vector3(transform.forward.x, 0.6f, transform.forward.z)
                );
                rb.AddTorque(
                    new Vector3(rot.x, rot.y, rot.z) * wheeliePower,
                    ForceMode.Acceleration
                );
            }
            else
            {
                rb.angularDamping = 1;
            }

            if (rb.linearVelocity.magnitude < 0.001f)
            {
                xQuat = 0f;
                zQuat = 0f;
                turnLeanAmount = 0f;
            }

            Vector3 origin = transform.position + transform.forward * 0.5f + Vector3.up * 0.1f;
            if (Physics.Raycast(origin, Vector3.forward, out RaycastHit SLOPhit, 0.2f))
            {
                if (SLOPhit.normal.y < 0.5f) // facing a slope wall
                {
                    rb.AddForce(Vector3.up * 200f); // small hop to climb
                }
            }
        }

        void Update()
        {
            if (_stopped)
                return;

            ApplyCustomInput();
        }

        float GroundConformity(bool toggle)
        {
            if (toggle)
            {
                groundZ = transform.rotation.eulerAngles.z;
            }
            return groundZ;
        }

        void ApplyCustomInput()
        {
            if (GetComponent<MobileController>() == null)
            {
                CustomInput(
                    "Horizontal",
                    ref customSteerAxis,
                    steerControls.x,
                    steerControls.y,
                    false
                );
                CustomInput("Vertical", ref customAccelerationAxis, 1, 1, false);
                CustomInput(
                    "Horizontal",
                    ref customLeanAxis,
                    steerControls.x,
                    steerControls.y,
                    false
                );
                CustomInput("Vertical", ref rawCustomAccelerationAxis, 1, 1, true);
                wheelieInput = Input.GetKey(KeyCode.LeftShift);
            }
        }

        //Input Manager Controls
        float CustomInput(string name, ref float axis, float sensitivity, float gravity, bool isRaw)
        {
            var r = Input.GetAxisRaw(name);
            var s = sensitivity;
            var g = gravity;
            var t = Time.unscaledDeltaTime;

            if (isRaw)
                axis = r;
            else
            {
                if (r != 0)
                    axis = Mathf.Clamp(axis + r * s * t, -1f, 1f);
                else
                    axis = Mathf.Clamp01(Mathf.Abs(axis) - g * t) * Mathf.Sign(axis);
            }

            return axis;
        }

        IEnumerator GearChange(float time)
        {
            changeGear = false;
            engineSettings.currentGear = 0;
            yield return new WaitForSeconds(time);
            changeGear = true;
        }

        private bool _stopped = false;

        public void StopScooter()
        {
            _stopped = true;

            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            rb.Sleep();
        }

        public void ResumeScooter()
        {
            _stopped = false;

            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
        }
    }
}
