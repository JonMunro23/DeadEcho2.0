using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBuy : MonoBehaviour
{
    public Weapon weapon;
    Transform weaponObjSpawnHolder;

    [HideInInspector]
    public string weaponName;
    [HideInInspector]
    public int weaponCost;

    private void Awake()
    {
        weaponObjSpawnHolder = transform.GetChild(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitialiseWallBuy();
    }

    void InitialiseWallBuy()
    {
        Instantiate(weapon.weaponWallBuyObj, weaponObjSpawnHolder);
        weaponCost = weapon.cost;
        weaponName = weapon.name;
    }
}
