using DG.Tweening;
using UnityEngine;

namespace GMTKGJ2019
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemEffect : MonoBehaviour
    {
        Tween tween;

        private void Start()
        {
            GameParameters parameters = GameParameters.Instance;
            float duration = parameters.ItemEffectDuration;
            float distance = parameters.ItemEffectDistance;

            tween = DOTween
                .Sequence()
                .Join(transform
                    .DOLocalMove(
                        distance * Vector2.up,
                        duration)
                    .SetRelative(true))
                .Insert(duration/2, GetComponent<SpriteRenderer>().DOFade(0, duration/2))
                .AppendCallback(() => Destroy(gameObject));
        }

        private void OnDestroy()
        {
            tween.Kill();
        }
    }
}