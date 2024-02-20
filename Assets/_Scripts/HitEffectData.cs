using UnityEngine;

[CreateAssetMenu(fileName = "HitEffectData", menuName = "New HitEffects Data")]
public class HitEffectData : ScriptableObject
{
    [SerializeField]
    HitEffect metalHitFx, fleshHitFx, stoneHitFx, woodHitFx, sandHitFx;

    public void SpawnHitEffect(SurfaceTypes surfaceType, Vector3 spawnLocation, Quaternion spawnRotation)
    {
        HitEffect effectToInstantiate = null;
        switch (surfaceType)
        {
            case SurfaceTypes.metal:
                effectToInstantiate = metalHitFx;
                break;
            case SurfaceTypes.flesh:
                effectToInstantiate = fleshHitFx;
                break;
            case SurfaceTypes.stone:
                effectToInstantiate = stoneHitFx;
                break;
            case SurfaceTypes.sand:
                effectToInstantiate = sandHitFx;
                break;
            case SurfaceTypes.wood:
                effectToInstantiate = woodHitFx;
                break;
        }
        GameObject clone = Instantiate(effectToInstantiate.hitEffectPrefab, spawnLocation, spawnRotation);
        AudioSource audioSource = clone.GetComponent<AudioSource>();
        PlayRandomSFX(effectToInstantiate, audioSource);
        clone.GetComponent<ParticleSystem>().Emit(1);
    }

    void PlayRandomSFX(HitEffect effect, AudioSource source)
    {
        source.PlayOneShot(effect.hitEffectSfx[UnityEngine.Random.Range(0, effect.hitEffectSfx.Length)]);
    }
}
