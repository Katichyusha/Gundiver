using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int[] maxAmmo; //set values in editor
    public int[] ammo; // global ammo counter for all weapons, gotten and incremented from gun scripts

    public float oxygen = 1f;
}
