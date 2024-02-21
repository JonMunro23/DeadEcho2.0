using DG.Tweening;
using UnityEngine;

public class SimpleMoveTo : MonoBehaviour
{
    [SerializeField]
    Transform gameobjectToMoveTo;
    [SerializeField]
    float moveDuration;

    private void Start()
    {
        transform.DOMove(gameobjectToMoveTo.position, moveDuration).SetEase(Ease.Linear);
    }
}
