using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zombieOffNavMeshLink : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer[] memes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meme in memes)
        {
            meme.enabled = false;
        }
    }
}
