using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class Octo : MonoBehaviour, IEnemy
{
    public float walkSpeed = 3f; //speed that the octo moves at
    public DetectionZone attackZone; //zone of detection for attack
    public DetectionZone foundZone; //zone of detection to chase player
    public DetectionZone cliffZone; //checks if on an edge
    public bool hasTarget = false;
    public bool foundTarget = false;
    public WalkableDirection startDirection = WalkableDirection.Right; //vector of initial walk

    private Rigidbody2D _rb; //rigidbody component of the octo.
    private TouchingDirections _touchingDirections; //using touchigndirections to check walls.
    private Animator _animator; //animator of the octo
    private Damageable _damageable; //damageable obj of Octo.
    private EnemyHealthBar _healthBar; //healthbar of octo

    private bool _hasLOS; //checks if the player is in line of sight
    private Vector3 _playerPos; //the target player position
    private Vector2 _walkDirectionVector; //vector of walk


    public enum WalkableDirection
    {
        Right,
        Left
    } //direction the octo is moving in

    private WalkableDirection _walkDirection; //direction of walk


    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            //if the direction needs to be flipped.
            if (_walkDirection != value)
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1,
                    gameObject.transform.localScale.y); //changes localscale if wrong

                //sets walking vector
                if (value == WalkableDirection.Right)
                {
                    _walkDirectionVector = Vector2.right;
                }
                else if (value == WalkableDirection.Left)
                {
                    _walkDirectionVector = Vector2.left;
                }
            }

            _walkDirection = value;
        }
    }

    //checks if octo has locked on
    public bool HasTarget
    {
        get { return hasTarget; }
        set
        {
            hasTarget = value;
            _animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    //checks if octo has found a target
    public bool FoundTarget
    {
        get { return foundTarget; }
        set
        {
            foundTarget = value;
            _animator.SetBool(AnimationStrings.foundTarget, value);
        }
    }


    public bool CanMove
    {
        get { return _animator.GetBool(AnimationStrings.canMove); }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _touchingDirections = GetComponent<TouchingDirections>();
        _animator = GetComponent<Animator>();
        _damageable = GetComponent<Damageable>();
        _healthBar = GetComponentInChildren<EnemyHealthBar>();

        //sets initial direction
        _walkDirection = startDirection;
        if (_walkDirection == WalkableDirection.Right)
        {
            _walkDirectionVector = Vector2.right;
        }
        else
        {
            _walkDirectionVector = Vector2.left;
        }

        //setups damageable
        _damageable.InvincibleTime = 0.1f;
        _damageable.Multiplier = 5;
    }

    // Update is called once per frame
    void Update()
    {
        //checks if either a target is locked for attack or if is in range of chase
        HasTarget = attackZone.detectedColliders.Count > 0;
        foundTarget = foundZone.detectedColliders.Count > 0;

        _playerPos = foundZone.playerPos;
    }

    private void FixedUpdate()
    {
        //if is on the floor and collided with a wall, turn around
        if (_touchingDirections.IsOnWall && _touchingDirections.IsGrounded || cliffZone.detectedColliders.Count == 0)
        {
            FlipDirection();
        }


        //makes the octo move
        if (CanMove)
        {
            // TODO: check if anything above, if not and detected in LOS, jump up.
            if (foundTarget)
            {
                //checks for player LOS
                RaycastHit2D ray = Physics2D.Raycast(transform.position, _playerPos - transform.position, 30f);
                if (!(ray.collider is null))
                {
                    _hasLOS = ray.collider.CompareTag("Player");
                }

                if (_hasLOS)
                {
                    ChaseFound(); //if can chase after the player
                }
            }


            if (!_touchingDirections.IsOnWall) //if walking normally
            {
                _rb.velocity = new Vector2(walkSpeed * _walkDirectionVector.x, _rb.velocity.y);
            }
            else //if is on a wall, stop but keep the y momentum
            {
                _rb.velocity = new Vector2(0, _rb.velocity.y);
            }

            if (_rb.velocity.y > 0 && _playerPos.y + 2f < _rb.position.y) //if already over the wall, stop y momentum
            {
                _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.2f);
            }
        }
        else
        {
            //if cannot move, make him stop with a damp.
            _rb.velocity = new Vector2(Mathf.Lerp(_rb.velocity.x, 0, 0.05f), _rb.velocity.y);
        }
    }

    //sets the correct direction for chasing
    private void ChaseFound()
    {
        //goes to the direction of the player
        if ((_playerPos.x > _rb.position.x && transform.localScale.x < 0) ||
            (_playerPos.x < _rb.position.x && transform.localScale.x > 0))
        {
            FlipDirection();
        }

        //jumps if the player is above
        if (_touchingDirections.IsOnPot && _touchingDirections.IsGrounded && _playerPos.y > _rb.position.y + 1f)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, 8f);
        }
        else if (_touchingDirections.IsGrounded && _playerPos.x > _rb.position.x - 3f &&
                 _playerPos.x < _rb.position.x + 3f && _playerPos.y > _rb.position.y + 1f)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, 8f);
        }
    }

    private void FlipDirection()
    {
        if (_touchingDirections.IsGrounded)
        {
            if (WalkDirection == WalkableDirection.Right)
            {
                WalkDirection = WalkableDirection.Left;
            }
            else if (WalkDirection == WalkableDirection.Left)
            {
                WalkDirection = WalkableDirection.Right;
            }
        }
    }

    //applies vigilant debuff onto enemy
    public IEnumerator ApplyGraviton(int seconds)
    {
        walkSpeed -= 0.5f * walkSpeed;
        //Wait for x seconds
        yield return new WaitForSeconds(seconds);
        walkSpeed *= 2;
    }

    //when hit.
    public void OnHit(int damage, Vector2 knockback)
    {
        //add the knockback
        _rb.velocity = new Vector2(knockback.x, _rb.velocity.y * 0.2f + knockback.y);
        //apply the healthbar update
        _healthBar.UpdateHp(_damageable.Health, _damageable.MaxHealth);
    }

    // Start is called before the first frame update
    void Start()
    {
        //set hp bar
        _healthBar.UpdateHp(_damageable.Health, _damageable.MaxHealth);
    }
}