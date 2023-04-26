using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireHarpoon : MonoBehaviour
{
    private int currAmmo = 100; // temporary
    public GameObject projectileToFire; // some kind of spear/harpoon thing
    [SerializeField] private bool canShoot;
    [Header("Stats")]
    public float damage;
    public float maxRange;
    public float ROF;
    public float shotForce;
    [Header("Settings")]
    [SerializeField] private AudioClip shotSound;
    [SerializeField] private Transform shotOrigin;
    [SerializeField] private Vector3 shotPosAdjust;
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private PlayerStats stats;

    public void Shoot(){
        Primary();
    }


    public void Primary(){
        if(currAmmo != 0 && canShoot){

            canShoot = false;
            Invoke(nameof(ShootTimeout), 1/ROF);

            anim.SetBool("shot", true);

            SoundManager.Instance.PlayOneShot(shotSound, SoundManager.sourceTypes.wep);

            //RaycastHit hitInfo;
            //Rigidbody hitRb;  i dont know if these are needed but they stay just in case
            //Vector3 forceDir;

            currAmmo--; //to be linked with playerstats

            var firedProjectile = Instantiate(projectileToFire, shotOrigin.position, Quaternion.LookRotation(Camera.main.transform.forward));
            var FPScript = firedProjectile.GetComponent<Projectile>();

            FPScript.damage = this.damage;
            FPScript.lifeTime = (FPScript.speed/this.maxRange) * 4f;
        }
    }

    public void ShootTimeout(){
        canShoot = true;
        anim.SetBool("shot", false);
        anim.speed = 1f;
    }
}
