using UnityEngine;
#if UNITY_EDITOR

using UnityEditor;

#endif

public class Door : MonoBehaviour
{
    public int doorCost;
    [SerializeField] GameObject doorObject;

    AudioSource doorOpeningSFXSource;

    public bool isOpen;

    [Header("Animation")]
    [Tooltip("if false will destory doorObject and its children")]
    [SerializeField] bool playsAnimation;
    AnimationClip animationToPlay;

    private void Awake()
    {
        doorOpeningSFXSource = GetComponent<AudioSource>();
    }

    public void OpenDoor()
    {
        isOpen = true;

        if(doorOpeningSFXSource != null)
            doorOpeningSFXSource.Play();

        Destroy(doorObject);
        Destroy(this.gameObject, 5);
    }
    #region Editor
#if UNITY_EDITOR
    [CustomEditor(typeof(Door))]
    public class DoorEditor : Editor 
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Door door = (Door)target;
            if(door.playsAnimation)
            {
                EditorGUI.indentLevel++;
                door.animationToPlay = EditorGUILayout.ObjectField("Animation To Play", null, typeof(AnimationClip), true) as AnimationClip;
                EditorGUI.indentLevel--;
            }
        }
    }

#endif
    #endregion
}
