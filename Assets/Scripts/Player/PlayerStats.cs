using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int[] maxAmmo; //set values in editor
    public int[] ammo; // global ammo counter for all weapons, gotten and incremented from gun scripts

    public Health playerHealth;

    public float oxygen = 100f;
    public float oxyReductionAmt = 1f;
    public float oxyReductionInterval = 0.2f;

    public void OxyReduce(){
        if(oxygen > 0)
            oxygen -= oxyReductionAmt;
        else if(oxygen <= 0)
            oxygen = 0;
    }

    public void OxyReduceInvoke(){
        InvokeRepeating(nameof(OxyReduce), 0f, oxyReductionInterval);
    }
    public void CancelOxyReduceInvoke(){
        CancelInvoke(nameof(OxyReduce));
    }
}
