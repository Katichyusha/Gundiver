using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIControl : MonoBehaviour
{
    [Header("Settings")]
    public Sprite crosshairImage;
    public Image crosshair;

    [Header("Technical Auto Updating")]
    public float healthValue;
    public TMP_Text healthText;
    [SerializeField] private Health playerHealthScript;
    
    public float oxyValue;
    public TMP_Text oxyText;
    [SerializeField] private PlayerStats oxyAndAmmoScript;

    public void Start(){
        crosshair.sprite = crosshairImage;
    }

    public void Update(){
        UpdateHealthValue();
        UpdateOxyValue();
    }


    public void UpdateHealthValue(){
        healthValue = playerHealthScript.currHP;
        healthText.text = healthValue.ToString();
    }

    public void UpdateOxyValue(){
        oxyValue = oxyAndAmmoScript.oxygen;
        oxyText.text = oxyValue.ToString();
    }
}
