using DG.Tweening;
using UnityEngine;

public class Bob : MonoBehaviour
{
    [SerializeField]
    float bobAmount, bobSpeed;

    private void Start()
    {
        transform.DOLocalMoveY(bobAmount, bobSpeed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetId(transform);
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}
