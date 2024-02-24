using UnityEngine;

[CreateAssetMenu(fileName = "HitEffect", menuName = "New HitEffect")]
public class HitEffect : ScriptableObject
{
    public GameObject hitEffectPrefab;
    public AudioClip[] hitEffectSfx;
}
