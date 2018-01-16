using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {
    
    private PlayerWeapon currentWeapon;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private WeaponManager weaponManager;
	// Use this for initialization
	void Start () {
        if (cam == null)
        {
            Debug.LogError("PlayerShoo: No camera referenced");
            this.enabled = false;
        }
        weaponManager = GetComponent<WeaponManager>();
	}

    // Update is called once per frame
    void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();
        if (currentWeapon.fireRate <= 0.0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }

    }

    //Is called on server on Shoot
    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    //is called on server when hit something
    //takes in hit points and normal of surface
    [Command]
    void CmdOnHit(Vector3 pos, Vector3 normal)
    {
        RpcDoHitEffect(pos, normal);
    }

    //is called on all clients when a shoot effect is required
    [ClientRpc]
    void RpcDoShootEffect()
    {
        weaponManager.GetCurrentWeaponGraphics().muzzleFlash.Play();
    }
    
    //is called on all clients
    //is used to spawn hit effects
    [ClientRpc]
    void RpcDoHitEffect(Vector3 pos, Vector3 normal)
    {
        GameObject hitEffect = (GameObject) Instantiate(weaponManager.GetCurrentWeaponGraphics().hitEffectPrefab, pos, Quaternion.LookRotation(normal));
        Destroy(hitEffect, 2f);
    }

    [Client]
    void Shoot()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //Shooting call onShoot
        CmdOnShoot();

        //cast a ray
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position,cam.transform.forward,out hit, currentWeapon.range,mask))
        {
            //hit something
            Debug.Log("We hit " + hit.collider.name);
            if (hit.collider.tag == "Player")
            {
                CmdPlayerShot(hit.collider.name,currentWeapon.damage);
            }

            //hit something, call the OnHit method on server
            CmdOnHit(hit.point, hit.normal);
        }
    }

    [Command]
    void CmdPlayerShot(string ID,int damage) {
        Debug.Log(ID + " has been shot.");

        Player player = GameManager.GetPlayer(ID);
        player.RpcTakeDamage(damage);
    }
}
