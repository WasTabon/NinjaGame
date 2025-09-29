using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIImageMove : MonoBehaviour
{
    [SerializeField] private RectTransform target; // сюда кидаешь Image (RectTransform)
    [SerializeField] private float moveDistance = 200f; // насколько сдвигать
    [SerializeField] private float duration = 1f; // время движения

    private void Start()
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        // Начинаем анимацию
        StartMove();
    }

    private void StartMove()
    {
        // Лево-право по оси X
        target.DOLocalMoveX(target.localPosition.x + moveDistance, duration)
            .SetLoops(-1, LoopType.Yoyo) // бесконечно туда-сюда
            .SetEase(Ease.InOutSine);   // плавное движение
    }
}
