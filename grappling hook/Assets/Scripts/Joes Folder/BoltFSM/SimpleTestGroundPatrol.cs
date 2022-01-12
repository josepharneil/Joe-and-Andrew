using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class SimpleTestGroundPatrol : MonoBehaviour
{
    [SerializeField] private Transform patrolToPosition;

    public bool isInAttackRange = false;

    private Tween _tween;

    public void Patrol( float duration )
    {
        _tween = transform.DOMove(patrolToPosition.position, duration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void End()
    {
        _tween.Kill();
    }
}
