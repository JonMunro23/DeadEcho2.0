using UnityEngine;

public enum PowerUpType
{
    MaxAmmo,
    InstantKill,
    DoublePoints,
    BottomlessClip,
    Devastator
}
[CreateAssetMenu(fileName = "PowerUpData", menuName = "New PowerUp")]
public class PowerUpData : ScriptableObject
{
    public PowerUpType PowerUpType;
    public AudioClip pickupSFx;
    public GameObject prefab;
    public Sprite UIIcon;
    public int litetime = 30;
    public int powerUpDuration = 30;
    public WeaponData powerUpWeapon;
}
