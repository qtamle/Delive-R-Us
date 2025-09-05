using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SMPScripts
{
    public class MobileController : MonoBehaviour
    {
        MotoController motoController;
        public MobileButtonHandler forward, backward, left, right, wheelie;
        void Start()
        {
            motoController = GetComponent<MotoController>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            MobileInput(left.buttonPressed+right.buttonPressed, ref motoController.customSteerAxis, motoController.steerControls.x, motoController.steerControls.y, false);
            MobileInput(forward.buttonPressed + backward.buttonPressed, ref motoController.customAccelerationAxis, 1, 1, false);
            MobileInput(left.buttonPressed+right.buttonPressed, ref motoController.customLeanAxis, motoController.steerControls.x, motoController.steerControls.y, false);
            MobileInput(forward.buttonPressed + backward.buttonPressed, ref motoController.rawCustomAccelerationAxis, 1, 1, true);
            motoController.wheelieInput = System.Convert.ToBoolean(wheelie.buttonPressed);
        }
        float MobileInput(int instruction, ref float axis, float sensitivity, float gravity, bool isRaw)
        {
            var r = instruction*2;
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
    }
}
