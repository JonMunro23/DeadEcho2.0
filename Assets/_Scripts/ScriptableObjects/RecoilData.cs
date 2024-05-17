using UnityEngine;

[CreateAssetMenu(fileName ="Recoil Data", menuName = "Weapons/Recoil Data")]
public class RecoilData : ScriptableObject
{
    [Header("Hip Recoil")]
    public Vector3 posRecoil;
    public Vector3 rotRecoil;
    [Header("ADS Recoil")]
    public Vector3 aimingPosRecoil;
    public Vector3 aimingRotRecoil;
    [Header("Camera Recoil")]
    public Vector3 cameraPosRecoil;
    public Vector3 cameraRotRecoil;
    public Vector3 cameraADSPosRecoil;
    public Vector3 cameraADSRotRecoil;
}
