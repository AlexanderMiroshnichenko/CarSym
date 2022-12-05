using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
   [SerializeField] private WheelControllerTFM[] wheel;
    [SerializeField] private ParticleSystem[] smoke;
    

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (ParticleSystem p in smoke)
        {
            foreach (WheelControllerTFM w in wheel)
                if (Mathf.Abs(w.slipAngle) > 5)
                {
                    p.maxParticles = 10000;
                }
                else p.maxParticles = 0;
        }
    }
}
