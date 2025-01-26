using Bubble;
using Events;
using UnityEngine;

public class ProtagEvents : MonoBehaviour
{
    [SerializeField]
    private Bubble.Bubble _bubble;

    [Header("Event (Out)")]

    [SerializeField]
    private VoidEvent _onEnterNegative;

    [SerializeField]
    private VoidEvent _onExitNegative;

    private bool _negatived;

    private void Awake()
    {
        _bubble.OnAbsorbedByOther.AddListener(OnAbsorbedByOther);
        _bubble.OnBecomeIndividual.AddListener(OnBecomeIndividual);
    }

    private void OnDestroy()
    {
        _bubble.OnAbsorbedByOther.RemoveListener(OnAbsorbedByOther);
        _bubble.OnBecomeIndividual.RemoveListener(OnBecomeIndividual);
    }

    private void OnAbsorbedByOther(Bubble.Bubble bubble)
    {
        if (bubble.BubbleType == BubbleType.Negative)
        {
            _negatived = true;
            _onEnterNegative.Raise();
        }
    }

    private void OnBecomeIndividual()
    {
        if (_negatived)
        {
            _negatived = false;
            _onExitNegative.Raise();
        }
    }
}