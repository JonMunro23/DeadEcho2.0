using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilManager : MonoBehaviour
{


    

    //private void OnEnable()
    //{
    //    WeaponShooting.onWeaponFired += ApplyRecoil;
    //}

    //private void OnDisable()
    //{
    //    WeaponShooting.onWeaponFired -= ApplyRecoil;
    //}

    

    //private void Update()
    //{
    //    Recoiling();
    //}

    //public void StartRecoil(float recoilParam, float maxRecoil_xParam, float recoilSpeedParam)
    //{
    //    // in seconds
    //    recoil = recoilParam;
    //    maxRecoil_x = maxRecoil_xParam;
    //    recoilSpeed = recoilSpeedParam;
    //    maxRecoil_y = Random.Range(-maxRecoil_xParam, maxRecoil_xParam);
    //}

    //void Recoiling()
    //{
    //    if (recoil < 0f)
    //    {
    //        Quaternion maxRecoil = Quaternion.Euler(maxRecoil_x, maxRecoil_y, 0f);
    //        // Dampen towards the target rotation
    //        transform.localRotation = Quaternion.Slerp(transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeed);
    //        recoil += Time.deltaTime;
    //    }
    //    else
    //    {
    //        recoil = 0f;
    //        // Dampen towards the target rotation
    //        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * recoilSpeed / 2);
    //    }
    //}
}
