using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Health : MonoBehaviour
{
    public float maxHP;
    public float currHP;
    public AudioClip[] sounds;
    private float damageBuffer;

    public UnityEvent onDamage;
    public UnityEvent onDeath;
    public UnityEvent onHeal;

    public void Start(){
        currHP = maxHP;
    }

    public void Update(){
        if(damageBuffer!=0){
            currHP += damageBuffer;
            if(damageBuffer > 0)
                onHeal.Invoke();
            else if(damageBuffer < 0){
                onDamage.Invoke();
            }
            if(currHP <= 0)
                onDeath.Invoke();
        }
        damageBuffer = 0;
    }

    public void DAMAGE(float value){
        damageBuffer += value;
    }

    public float InformDamageBuffer(){
        return damageBuffer;
    }
    
    public void RequestSound(int indexOfAudioClip){
        SoundManager.Instance.PlayOneShot(sounds[indexOfAudioClip], SoundManager.sourceTypes.hostile);
    }

}
