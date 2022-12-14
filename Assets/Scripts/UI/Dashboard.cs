using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dashboard : MonoBehaviour
{
    [SerializeField] private bool isEnabled;
    [SerializeField] private Transform needle;
    [SerializeField] private Text speed;
    [SerializeField] private Text gear;
    [SerializeField] private GearBoxComponent gearComponent;
    private float tachoAngle;
    [SerializeField] private float zeroTachoAngle = 36f;
    [SerializeField] private float maxTachoAngle = -130f;
    private float tachoAngleSize;
    private float engineMaxRpm;
    private Rigidbody rb;
    [SerializeField] private Text angle;
    [SerializeField] private WheelControllerTFM wheel;
    public void InitDashboard(Rigidbody _rb, float maxRpm)
    {
        engineMaxRpm = maxRpm;
        rb = _rb;
        tachoAngleSize = zeroTachoAngle - maxTachoAngle;
    }

    // Update is called once per frame
    public void UpdateD(float engineRpm)
    {
        if (isEnabled)
        {
            float rpmNormalized = engineRpm / engineMaxRpm;
            tachoAngle = zeroTachoAngle - rpmNormalized * tachoAngleSize;
            needle.eulerAngles = new Vector3(0, 0, tachoAngle);
            speed.text = Mathf.Round(rb.velocity.magnitude * 3.6f).ToString();
            angle.text = "Angle: "+Mathf.RoundToInt(Mathf.Abs(wheel.slipAngle)).ToString();
            if(gearComponent.GetCurrentGear()==1)
            gear.text = "N";
            else gear.text = (gearComponent.GetCurrentGear() - 1).ToString();
            if (gearComponent.GetCurrentGear() == 0)
                gear.text = "R";
            else gear.text = (gearComponent.GetCurrentGear() - 1).ToString();
            

        }
    }
}
