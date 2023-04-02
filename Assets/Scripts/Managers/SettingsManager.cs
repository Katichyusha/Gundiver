using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public int targetFPS = 120;
    public int vSyncQuality = 0; // 0 = off, theres a few other ones that arent important yet

    private void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }

        QualitySettings.vSyncCount = vSyncQuality;
        Application.targetFrameRate = targetFPS;
    }

    private void Update(){
        if (Application.targetFrameRate != targetFPS){
            Application.targetFrameRate = targetFPS;
        }
    }
}
