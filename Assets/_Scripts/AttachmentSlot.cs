using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachmentSlot : MonoBehaviour
{
    public enum AttachmentType
    { 
        sight,
        muzzle,
        stock,
        magazine
    }
    public AttachmentType attachmentType;

    [SerializeField] GameObject[] availableAttachments;

    public void InitialiseAttachmentSlot()
    {

    }
}
