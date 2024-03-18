using DG.Tweening;
using System;
using UnityEngine;

public class BulletHole : MonoBehaviour
{
    [SerializeField]
    Material metalMaterial ,stoneMaterial ,woodMaterial ,fleshMaterial ,sandMaterial;
    [SerializeField] float lifetime;
    MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        SetRandomRotation();

        transform.GetComponent<MeshRenderer>().material.DOColor(Color.clear, 5).SetDelay(lifetime).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    void SetRandomRotation()
    {
        transform.Rotate(Vector3.up, UnityEngine.Random.Range(0, 360));
    }

    public void SetMaterialType(SurfaceTypes surfaceType)
    {
        switch (surfaceType)
        {
            case SurfaceTypes.metal:
                meshRenderer.material = metalMaterial;
                break;
            case SurfaceTypes.stone:
                meshRenderer.material = stoneMaterial;
                break;
            case SurfaceTypes.wood:
                meshRenderer.material = woodMaterial;
                break;
            case SurfaceTypes.flesh:
                meshRenderer.material = fleshMaterial;
                break;
            case SurfaceTypes.sand:
                meshRenderer.material = sandMaterial;
                break;
            default:
                meshRenderer.material = stoneMaterial;
                break;
        }
    }
}
