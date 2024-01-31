using DG.Tweening;

public static class TweenUtils
{
    public static Tween GetBlankTween(float delaySec = 0.2f)
    {
        return DOVirtual.DelayedCall(delaySec, () => { });
    }
}
