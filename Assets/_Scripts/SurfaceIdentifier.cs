using UnityEngine;

public enum SurfaceTypes
{
    metal,
    stone,
    wood,
    flesh,
    sand,
    glass
}

public class SurfaceIdentifier : MonoBehaviour
{
    public SurfaceTypes surfaceType;
}
