using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RocksAnimation : MonoBehaviour {

    private float yInitialCoord;

    void Start()
    {
        yInitialCoord = transform.position.y + 0.3f;
        transform.DOMove(new Vector3(transform.position.x, yInitialCoord, transform.position.z), 0f);

        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMove(new Vector3(transform.position.x, yInitialCoord - 0.6f, transform.position.z), 2f).SetEase(Ease.InOutSine));
        s.Append(transform.DOMove(new Vector3(transform.position.x, yInitialCoord, transform.position.z), 2f).SetEase(Ease.InOutSine));
        s.SetLoops(-1,LoopType.Yoyo);
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 1), 1f);
    }

}
