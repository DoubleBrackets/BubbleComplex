using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class SimpleAnimator : MonoBehaviour, IAnimationClipSource
{
    [SerializeField]
    private AnimationClip _animationClip;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private bool _playOnAwake;

    public PlayableGraph PlayablePlayableGraph => _playableGraph;

    private PlayableGraph _playableGraph;

    private bool _graphCreated;

    private AnimationClip _currentClip;

    private bool _manualUpdate;

    private void Awake()
    {
        if (_playOnAwake)
        {
            Play();
        }
    }

    private void OnDestroy()
    {
        if (_playableGraph.IsValid())
        {
            _playableGraph.Destroy();
        }
    }

    public void GetAnimationClips(List<AnimationClip> results)
    {
        results.Add(_animationClip);
    }

    public void UpdateAnim(float deltaTime)
    {
        _playableGraph.Evaluate(deltaTime);
    }

    public void Play(bool restart = false)
    {
        Play(_animationClip, restart);
    }

    public void Play(AnimationClip clip, bool restart = false)
    {
        // Lazy instatiation
        if (!_graphCreated || _currentClip != clip)
        {
            if (_playableGraph.IsValid())
            {
                _playableGraph.Destroy();
            }

            _graphCreated = true;
            CreateGraph(clip);
        }

        if (_playableGraph.IsValid())
        {
            if (!_manualUpdate)
            {
                _playableGraph.Play();
            }

            if (restart)
            {
                _playableGraph.GetRootPlayable(0).SetTime(0);
            }
        }
    }

    public void SetNormalizedTime(float normalizedTime)
    {
        if (_playableGraph.IsValid())
        {
            _playableGraph.GetRootPlayable(0).SetTime(normalizedTime * _currentClip.length);
        }
    }

    public void Stop()
    {
        if (_playableGraph.IsValid())
        {
            SetNormalizedTime(1);
            _playableGraph.Evaluate();
            _playableGraph.Stop();
        }
    }

    public void SetManualUpdate(bool manualUpdate)
    {
        _manualUpdate = manualUpdate;
        if (_playableGraph.IsValid())
        {
            _playableGraph.Stop();
        }
    }

    private void CreateGraph(AnimationClip clip)
    {
        _playableGraph = PlayableGraph.Create($"{gameObject.name}.SimpleAnimator");

        _currentClip = clip;

        var output = AnimationPlayableOutput.Create(_playableGraph, string.Empty, _animator);

        var animationClipPlayable = AnimationClipPlayable.Create(_playableGraph, clip);

        output.SetSourcePlayable(animationClipPlayable);
    }
}