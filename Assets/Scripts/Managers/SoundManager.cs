using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //still requires behaviour for changing the volume of certain channels, specifically the larger ones (enviro, hostile and weapon)
    //music channel and behaviour still needed (whenever that happens its easy as fuck)
    public static SoundManager Instance;
    public AudioSource masterSource;
    [Header("Environment Channel")]
    [SerializeField] private AudioSource _ambientSource;
    [SerializeField] private AudioSource _propSource;
    [SerializeField] private AudioSource _effectSource;
    [Header("Hostiles Channel")]
    [SerializeField] private AudioSource _hostileSource;
    [Header("Weapons Channel")]
    [SerializeField] private AudioSource _weaponSource;
        //all sources above are mixer channels, stupid variable formatting just so it looks nice in editor
    public enum sourceTypes{master, ambient, prop, hostile, wep, effect};

    void Awake(){   // ensure theres only one instance of the sound manager
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    public void PlayOneShot(AudioClip clip, sourceTypes typeOfAudioSource){ // give the clip and the audiosource that needs to play it and it does
        switch(typeOfAudioSource){
            case sourceTypes.master:
                masterSource.PlayOneShot(clip);
                break;
            case sourceTypes.ambient:
                _ambientSource.PlayOneShot(clip);
                break;
            case sourceTypes.prop:
                _propSource.PlayOneShot(clip);
                break;
            case sourceTypes.hostile:
                _hostileSource.PlayOneShot(clip);
                break;
            case sourceTypes.wep:
                _weaponSource.PlayOneShot(clip);
                break;
            case sourceTypes.effect:
                _effectSource.PlayOneShot(clip);
                break;
            default:
                Debug.LogError("Audio Source of type " + typeOfAudioSource.ToString() + "does not exist");
                break;
        }
    }
}
