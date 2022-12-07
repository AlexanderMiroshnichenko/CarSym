using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
   [SerializeField] private WheelControllerTFM[] wheel;
    [SerializeField] private ParticleSystem[] smoke;
    [SerializeField] private float angleToDrift;
    [SerializeField] private Rigidbody rb;

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (ParticleSystem p in smoke)
        {
            foreach (WheelControllerTFM w in wheel)
                if (Mathf.Abs(w.slipAngle) > angleToDrift)
                {
                    p.maxParticles = (int)Mathf.Abs(w.slipAngle) * (int)rb.velocity.magnitude;

                }
                else if (w.maximumFrictionTorque < w.targetFrictionTorque)
                {
                    p.maxParticles = (int)w.targetFrictionTorque;
                }
                else p.maxParticles = 0;
        }
    }
}
