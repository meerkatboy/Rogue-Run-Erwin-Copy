using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    public ContactFilter2D castFilter; //cast filter to use ray casting to check

    public float groundDistance = 0.05f; //max distance between player and floor to be grounded.
    public float wallDistance = 0.2f; //max distance between player and wall to be on wall.
    public float ceilDistance = 0.05f; //max distance between player and ceiling to be on ceiling.
    public float potDistance = 3f; //distance for enemy to jump

    private CapsuleCollider2D _touchingCol; //player collider
    private Animator _animator; //player animation

    private RaycastHit2D[] _groundHits = new RaycastHit2D[5]; //ray casts that hits the floor
    private RaycastHit2D[] _wallHits = new RaycastHit2D[5];
    private RaycastHit2D[] _ceilHits = new RaycastHit2D[5];
    private RaycastHit2D[] _potHits = new RaycastHit2D[5]; //potential walls

    //checks which direction the player is facing using localScale.
    private Vector2 WallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    [SerializeField] private bool _isGrounded = true; //player grounded?

    //Property for if the player is grounded.

    public bool IsGrounded
    {
        get { return _isGrounded; }
        private set
        {
            _isGrounded = value;
            _animator.SetBool(AnimationStrings.isGrounded, value);
        }
    }

    [SerializeField] private bool _isOnWall = true; //player is touching wall?

    //Property for if the player is on a wall.
    public bool IsOnWall
    {
        get { return _isOnWall; }
        private set
        {
            _isOnWall = value;
            _animator.SetBool(AnimationStrings.isOnWall, value);
        }
    }

    [SerializeField] private bool _isOnPot = true; //player is front of wall

    //Property for if the player is front of wall.
    public bool IsOnPot
    {
        get { return _isOnPot; }
        private set { _isOnPot = value; }
    }

    [SerializeField] private bool _isOnCeil = true; //player on ceiling?

    //Property for if the player is on a ceiling.
    public bool IsOnCeil
    {
        get { return _isOnCeil; }
        private set
        {
            _isOnCeil = value;
            _animator.SetBool(AnimationStrings.isOnCeil, value);
        }
    }


    private void Awake()
    {
        _touchingCol = GetComponent<CapsuleCollider2D>();
        _animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //checks if the player is grounded (array of touching below blocks > 0)
        IsGrounded = _touchingCol.Cast(Vector2.down, castFilter, _groundHits, groundDistance) > 0;
        //checks if the player is touching a wall (on both sides)
        IsOnWall = _touchingCol.Cast(WallCheckDirection, castFilter, _wallHits, wallDistance) > 0;
        IsOnCeil = _touchingCol.Cast(Vector2.up, castFilter, _ceilHits, ceilDistance) > 0;
        IsOnPot = _touchingCol.Cast(WallCheckDirection, castFilter, _potHits, potDistance) > 0;
    }
}