using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBehaviour : StateMachineBehaviour
{
    public float fadeTime = 1f; //time for the sprite to fade away
    private float _timeElapsed = 0f;
    //private SpriteRenderer _spriteRenderer;
    private GameObject _toRemove;
    //private Color _startColor;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timeElapsed = 0f;
        //_spriteRenderer = animator.GetComponent<SpriteRenderer>();
        //_startColor = _spriteRenderer.color;
        _toRemove = animator.gameObject;
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timeElapsed += Time.deltaTime;
        
        //float newAlpha = _startColor.a *  (1 - _timeElapsed / fadeTime); //fades the sprite

        //_spriteRenderer.color = new Color(_startColor.r, _startColor.g, _startColor.b, newAlpha);

        if (_timeElapsed > fadeTime) //destroy after time is up
        {
            Destroy(_toRemove);
        }
    }
}
