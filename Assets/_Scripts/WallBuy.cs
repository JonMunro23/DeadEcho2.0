using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBuy : MonoBehaviour
{
    [SerializeField] Weapon weaponToShow;
    Transform weaponObjSpawnHolder;

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
        Instantiate(weaponToShow.weaponWallBuyObj, weaponObjSpawnHolder);
        weaponCost = weaponToShow.cost;
    }
}
