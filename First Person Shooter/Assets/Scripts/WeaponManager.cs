using UnityEngine.Networking;
using UnityEngine;

public class WeaponManager : NetworkBehaviour{

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private PlayerWeapon primaryWeapon;

    [SerializeField]
    private Transform weaponHolder;

    private PlayerWeapon currentWeapon;

    private WeaponGraphics currentGraphics;
	// Use this for initialization
	void Start () {
        EquipWeapon(primaryWeapon);
	}

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentWeaponGraphics()
    {
        return currentGraphics;
    }

    void EquipWeapon(PlayerWeapon weapon)
    {
        currentWeapon = weapon;

        GameObject weaponInst = (GameObject) Instantiate(weapon.graphics, weaponHolder.position, weaponHolder.rotation);

        weaponInst.transform.SetParent(weaponHolder);

        currentGraphics = weaponInst.GetComponent<WeaponGraphics>();
        if (currentGraphics == null)
        {
            Debug.LogError("No WeaponGraphics component on the weapon object: " + weaponInst.name);
        }
        if (isLocalPlayer)
        {
            Utils.SetLayerRecursively(weaponInst, LayerMask.NameToLayer(weaponLayerName));
        }
    }
}
