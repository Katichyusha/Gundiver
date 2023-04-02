using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalManager : MonoBehaviour
{
    public static DecalManager Instance;

    public List<GameObject> decalList;
    public List<GameObject> effectsList;
    // 1.bullethole     2. harpoonhole      3. explosionchar    4. blood

    private void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    public void SpawnDecalFromRay(RaycastHit hitInfo, int decalIndex){
        var decal = Instantiate(decalList[decalIndex], hitInfo.point, Quaternion.identity);
        decal.transform.forward = hitInfo.normal * -1f;
        decal.transform.position -= decal.transform.forward * 0.5f;
        Destroy(decal, 10f);
        var effect = Instantiate(effectsList[decalIndex], hitInfo.point, Quaternion.identity);
        effect.transform.forward = hitInfo.normal * -1f;
        //effect.transform.position -= effect.transform.forward * 0.5f;
        //Destroy(effect, 10f);
    }
}
