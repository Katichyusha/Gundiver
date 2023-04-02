using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireDB : MonoBehaviour
{
    private int currAmmo = 100; // temporary
    [SerializeField] private bool canShoot;
    [Header("Stats")]
    public int numOfPellets;
    public float damage;
    public float maxRange;
    public float maxAccuracy;
    public float ROF; //counts as rechamber time for the shotty
    public float shotForce;
    [Header("Settings")]
    [SerializeField] private AudioClip shotSound;
    [SerializeField] private Transform shotOrigin;
    [SerializeField] private Vector3 shotPosAdjust;
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private PlayerStats stats;

    public void Shoot(){
        if(currAmmo != 0 && canShoot){
            canShoot = false;
            Invoke(nameof(ShootTimeout), 1/ROF);
            anim.SetBool("shot", true);
            SoundManager.Instance.PlayOneShot(shotSound, SoundManager.sourceTypes.wep);
            RaycastHit hitInfo;
            Rigidbody hitRb;
            Vector3 forceDir;

            currAmmo--;

            for(int i=0; i<numOfPellets; i++){
                Vector3 accModifier = new Vector3(Random.Range(-maxAccuracy, maxAccuracy), Random.Range(-maxAccuracy, maxAccuracy), Random.Range(-maxAccuracy, maxAccuracy));
                Debug.DrawRay(shotOrigin.transform.position + shotPosAdjust, (Camera.main.transform.forward + accModifier) * maxRange, Color.red, 1f);

                if(Physics.Raycast(shotOrigin.transform.position + shotPosAdjust, Camera.main.transform.forward + accModifier, out hitInfo, maxRange, whatIsEnemy)){
                    forceDir = hitInfo.point - shotOrigin.transform.position;
                    hitInfo.collider.SendMessage("DAMAGE", -damage, SendMessageOptions.DontRequireReceiver);
                    if(hitInfo.collider.gameObject.TryGetComponent<Rigidbody>(out hitRb))
                        hitRb.AddForce(forceDir.normalized * shotForce, ForceMode.Impulse);
                    else
                        hitInfo.collider.gameObject.SendMessage("Knockback", forceDir.normalized, SendMessageOptions.DontRequireReceiver);
                    if(hitInfo.collider.CompareTag("WORLD"))
                        DecalManager.Instance.SpawnDecalFromRay(hitInfo, 0);
                }
            }
            }
            else if(currAmmo == 0){
                //make empty sound
            }
    }
    
    public void ShootTimeout(){
        canShoot = true;
        anim.SetBool("shot", false);
    }

}
