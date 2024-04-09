using UnityEngine;
using UnityEngine.Splines;

public class zombieOffNavMeshLink : MonoBehaviour
{
    [SerializeField]
    SplineContainer navLinkSpline;


    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer[] memes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meme in memes)
        {
            meme.enabled = false;
        }
    }

    void GenerateSplinePath()
    {
        Spline linkSpline = new Spline();
       // linkSpline.Add();
        navLinkSpline.AddSpline(linkSpline);
    }
}
