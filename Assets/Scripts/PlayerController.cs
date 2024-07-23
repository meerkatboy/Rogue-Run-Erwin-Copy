using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    //speeds
    public float movementSpeed = 8f; //speed of player.
    public float jumpImpulse = 10f; //velocity of jump
    private float _dashSpeed = 35f; //current dash speed if should be kept

    //inputs
    private Vector2 _moveInput; //gets vector from player input.

    //components
    private Rigidbody2D _rb; //Rigidbody2D of player.
    private Animator _animator; //animator of the player sprite
    private Damageable _damageable; //damageable component
    private TrailRenderer _trail; //trail
    private TouchingDirections _touchingDirections; //Used for ground checking
    private PlayerAttack _playerAttack; //player's attack info
    public DetectionZone gravitonZone; //zone of enemies to be effected by graviton

    //movement variables
    private int _maxDash = 1; //Number of dashes that can be replenished to
    private int _dashCount = 1; //player's current dash count
    private bool _isMoving = false; //is the player moving
    private float _lastDash = -999f; //time since last dash
    private float _dashCD = 0.3f; //cooldown of dash
    private Vector2 _dashDir; //direction of dash
    public bool isFacingRight = true; //which way the player is facing
    private bool _canDash = true;

    //currency
    private int _darknessCount = 10000; //Count of currency

    //permanent upgrades
    private int _dashUp = 0; //decreased dash cd, in %
    private int _behindUp = 0; //Increased damage to enemies from behind, in %
    private int _decFirstUp = 0; //Decreased first damage taken per room, %
    private int _roomHealUp = 0; //Damage healed per room, flat
    private int _darkUp = 0; //Increased darkness gained, %
    private int _killHealUp = 0; //Damaged healed per kill, flat
    private float _dmgMod = 0; //extra damage modification added on

    //temp upgrades
    private int _safeDashUp = 0; //decreased hurt while dashing, %
    private int _antiTrapUp = 0; //decreased hurt by traps, %
    private int _meatyUp = 0; //increase max HP
    private int _vigilantUp = 0; //decrease hurt upon kill, seconds
    private int _gliderUp = 0; //increase dmg in air, %
    private int _beastUp = 0; //increase dmg, %
    private int _rampageUp = 0; //increase dmg upon kill, s
    private int _surferUp = 0; //increase MS after dash, flat
    private int _gravitonUp = 0; //decrease MS of enemies after dash, s
    private int _swiftyUp = 0; //increase dodge chance, %
    private int _immortalUp = 0; //revives at 50% hp
    private int _solidUp = 0; //decrease hurt, %

    //boolean of buffs
    private bool _vigilantActive = false;
    private bool _rampageActive = false;

    //boolean of player status
    private bool _firstDeath = true;

    //lives remaining
    public int LivesRemaining
    {
        get { return _immortalUp; }
    }

    //dodge chance
    public int DodgeChance
    {
        get { return _swiftyUp; }
    }

    //bonus dmg in percentage striking from behind
    public int BehindBuff
    {
        get { return _behindUp; }
    }

    public bool CanDash
    {
        get { return _canDash; }
        set { _canDash = value; }
    }

    //checks if the player is moving
    public bool IsMoving
    {
        get { return _isMoving; }
        private set
        {
            _isMoving = value;
            //sync with the animator
            _animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    //checks if the player can move
    public bool CanMove
    {
        get { return _animator.GetBool(AnimationStrings.canMove); }
        set { _animator.SetBool(AnimationStrings.canMove, value); }
    }

    //checks if the player is dashing
    public bool Dashing
    {
        get { return _animator.GetBool(AnimationStrings.dashing); }
    }

    //checks if the player should stop dashing
    public bool StopDash
    {
        get { return _animator.GetBool(AnimationStrings.stopDash); }
    }

    //checks if the player is alive
    public bool IsAlive
    {
        get { return _animator.GetBool(AnimationStrings.isAlive); }
    }

    //checks if the player is spawning
    public bool IsSpawning
    {
        get { return _animator.GetBool(AnimationStrings.isSpawning); }
    }

    public bool IsFacingRight
    {
        get { return isFacingRight; }
        private set
        {
            //if facing is wrong
            if (isFacingRight != value)
            {
                //flip the sprite
                transform.localScale *= new Vector2(-1, 1);
            }

            isFacingRight = value;
        }
    }

    //gets darkness count
    public int DarknessCount
    {
        get { return _darknessCount; }
        set { _darknessCount = value; }
    }

    private void Awake()
    {
        //gets the rigidbody.
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>(); //gets the animator
        _touchingDirections = GetComponent<TouchingDirections>(); //wall detection
        _damageable = GetComponent<Damageable>(); //damageable component
        _damageable.SetPlayer(this);
        _trail = GetComponent<TrailRenderer>();
        _playerAttack = GetComponentInChildren<PlayerAttack>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }


    private void FixedUpdate()
    {
        if (!_damageable.IsAlive)
        {
            //if has respawn, respawn with 10% hp
            if (_immortalUp > 0)
            {
                _damageable.Health = _damageable.MaxHealth / 10;
                _immortalUp--;
                SceneController.instance.ReloadScene();
            }
            else if (_firstDeath)
            {
                //make sure only calls onces
                _firstDeath = false;
                //reset upgrades
                ResetTempUpgrades();
                //stop any motion
                _rb.velocity = Vector2.zero;
                CanDash = false;
                _trail.emitting = false; //stop the trail
                _animator.SetBool(AnimationStrings.dashing, false);
                _animator.SetBool(AnimationStrings.stopDash, true);
                //go back to spawn
                SceneController.instance.ResetPool();
                SceneController.instance.NextScene(2);
            }
        }

        float inputX = Input.GetAxisRaw("Horizontal"); //horizontal input
        float inputY = Input.GetAxisRaw("Vertical"); //vertical input


        //finds the direction the dash is going to
        _dashDir = new Vector2(inputX, inputY);

        //if the player is not moving, set the direction the player is facing as the dash direction.
        if (_dashDir == Vector2.zero)
        {
            _dashDir = new Vector2(transform.localScale.x, 0);
        }

        //if not being hit
        if (!_damageable.IsHit && !IsSpawning)
        {
            //if dashing, dash in the direction the player is facing
            if (Dashing)
            {
                if (Math.Abs(inputY) > 0)
                {
                    if (Math.Abs(inputX) + Math.Abs(inputY) > 1)
                    {
                        _rb.velocity = _dashDir.normalized * _dashSpeed / 1.2f;
                    }
                    else
                    {
                        _rb.velocity = _dashDir.normalized * _dashSpeed / 1.5f;
                    }
                }
                else
                {
                    _rb.velocity = _dashDir.normalized * _dashSpeed;
                }
            }
            else
            {
                if (StopDash) //if the dash should be stopped
                {
                    _trail.emitting = false; //stop the trail

                    //if surfer activated
                    if (_surferUp > 0)
                    {
                        //keeps a portion of the upwards momentum only
                        _rb.velocity = new Vector2(_moveInput.x * CurrentMoveSpeed + _surferUp / 10f,
                            _rb.velocity.y * 0.35f);
                    }
                    else
                    {
                        //keeps a portion of the upwards momentum only
                        _rb.velocity = new Vector2(_moveInput.x * CurrentMoveSpeed, _rb.velocity.y * 0.35f);
                    }

                    //if graviton activated
                    if (_gravitonUp > 0 && gravitonZone.detectedColliders.Count > 0)
                    {
                        foreach (Collider2D effectedCollider in gravitonZone.detectedColliders)
                        {
                            if (effectedCollider.CompareTag("Enemy"))
                            {
                                StartCoroutine(effectedCollider.gameObject.GetComponent<IEnemy>()
                                    .ApplyGraviton(_gravitonUp));
                            }
                        }
                    }

                    //resets the value of stop dash.
                    _animator.SetBool(AnimationStrings.stopDash, false);
                }
                else //change the velocity normally
                {
                    //changes the velocity
                    _rb.velocity = new Vector2(_moveInput.x * CurrentMoveSpeed, _rb.velocity.y);
                }
            }
        }
        else //if got hit
        {
            _trail.emitting = false; //stop the trail
            _animator.SetBool(AnimationStrings.dashing, false);
            _animator.SetBool(AnimationStrings.stopDash, true);
        }


        //sets the y velocity of the animator to check for rising or falling
        _animator.SetFloat(AnimationStrings.yVelocity, _rb.velocity.y);

        //replenish dash if is on the ground and not already dashing
        if (_touchingDirections.IsGrounded && !Dashing)
        {
            _dashCount = _maxDash;
        }

        // if (_rb.position.y < -9.2f)
        // {
        //     _damageable.Hit(100, Vector2.zero);
        //     _rb.velocity = Vector2.zero;
        // }
    }

    //player moves
    public void OnMove(InputAction.CallbackContext context)
    {
        //gets the vector of input
        _moveInput = context.ReadValue<Vector2>();

        if (IsAlive && !IsSpawning) //only move if the player is living and active
        {
            //checks if the player is moving
            IsMoving = _moveInput.x != 0;
            //sets the facing direction
            SetFacingDirection(_moveInput);
        }
        else
        {
            _isMoving = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // TODO: check if alive
        if (context.started && _touchingDirections.IsGrounded && CanMove)
        {
            _animator.SetTrigger(AnimationStrings.jumpTrigger);
            _rb.velocity = new Vector2(_rb.velocity.x, jumpImpulse);
        }

        if (context.canceled && _rb.velocity.y > 0f)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.5f);
        }
    }

    //when player dashes
    public void OnDash(InputAction.CallbackContext context)
    {
        //if has a dash, not being hit and not spawning
        if (context.started && _dashCount > 0 && !_damageable.IsHit && !IsSpawning && CanDash)
        {
            if (Time.time - _lastDash < _dashCD) //if dash cd not reached yet, cannot dash.
            {
                return;
            }

            _dashCount--; //reduce a dash count
            _trail.emitting = true;
            _lastDash = Time.time; //reset the cd
            _animator.SetTrigger(AnimationStrings.dashTrigger);
        }
    }

    //sets the sprite direction
    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    //calculates the current x move speed
    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove) //checks if can move
            {
                if (IsMoving && !_touchingDirections.IsOnWall) //if not on a wall and moving
                {
                    return movementSpeed;
                }
                else //if not moving or against wall
                {
                    return 0;
                }
            }
            else //if cannot move
            {
                return 0;
            }
        }
    }

    //when the player is hit.
    public void OnHit(int damage, Vector2 knockback)
    {
        //add the knockback
        _rb.velocity = new Vector2(knockback.x, _rb.velocity.y * 0.2f + knockback.y);
    }

    //change dash count
    public void AddDash(int count, GameObject obj)
    {
        //if no overflow
        if (_dashCount < _maxDash)
        {
            //add dash, disable
            _dashCount += count;
            StartCoroutine(DashUsed(obj));
        }
    }

    //disables the refill for a bit
    IEnumerator DashUsed(GameObject obj)
    {
        obj.SetActive(false);
        //Wait for 4 seconds
        yield return new WaitForSeconds(4);
        obj.SetActive(true);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //die when touch smth bad, 12 is groundhurt
        if (other.gameObject.layer == 12)
        {
            _damageable.Health = 0;
        }
    }

    //saves stats whenever new scene
    public (int[] intStats, float[] floatStats, int[] permUpStats, int[] tempUpStats) SaveStats()
    {
        int[] intStats = new[]
            { _maxDash, _damageable.MaxHealth, _playerAttack.AD, _damageable.Health, _darknessCount };
        float[] floatStats = new[] { _dashCD, movementSpeed, _dmgMod };
        int[] permUpStats = new[] { _dashUp, _behindUp, _decFirstUp, _roomHealUp, _darkUp, _killHealUp };
        int[] tempUpStats = new[]
        {
            _safeDashUp, _antiTrapUp, _meatyUp, _vigilantUp, _gliderUp, _beastUp, _rampageUp,
            _surferUp, _gravitonUp, _swiftyUp, _immortalUp
        };
        return (intStats, floatStats, permUpStats, tempUpStats);
    }


    //loads stats whenever new scene
    public void LoadStats(int[] intStats, float[] floatStats, int[] permUpStats, int[] tempUpStats)
    {
        //saves int info
        _maxDash = intStats[0];

        //increase max hp based on upgrade
        _damageable.MaxHealth = 100;
        _damageable.MaxHealth += _meatyUp;

        _playerAttack.AD = intStats[2];
        //checks if needs respawning
        if (intStats[3] > 0) //if still alive, keep hp
        {
            _damageable.Health = intStats[3];
        }

        _darknessCount = intStats[4];

        //saves float info
        _dashCD = floatStats[0];
        movementSpeed = floatStats[1];
        //adds the damage mod
        _dmgMod = floatStats[2];
        _damageable.DmgMod += _dmgMod;

        //saves perm upgrade info
        _dashUp = permUpStats[0];
        _behindUp = permUpStats[1];
        _decFirstUp = permUpStats[2];
        _roomHealUp = permUpStats[3];
        _darkUp = permUpStats[4];
        _killHealUp = permUpStats[5];

        //saves temp upgrade info
        _safeDashUp = tempUpStats[0];
        _antiTrapUp = tempUpStats[1];
        _meatyUp = tempUpStats[2];
        _vigilantUp = tempUpStats[3];
        _gliderUp = tempUpStats[4];
        _beastUp = tempUpStats[5];
        _rampageUp = tempUpStats[6];
        _surferUp = tempUpStats[7];
        _gravitonUp = tempUpStats[8];
        _swiftyUp = tempUpStats[9];
        _immortalUp = tempUpStats[10];
    }

    //adds darkness
    public void AddDarkness(int amount)
    {
        //add by multiplier
        amount += Convert.ToInt32(amount * _darkUp / 100f);
        _darknessCount += amount;
        //floating text
        CharacterEvents.CharacterDropped.Invoke(gameObject, amount);
    }

    //upgrades stats
    public void PermUpgrade(int upgrade)
    {
        switch (upgrade)
        {
            case 0: //reduce dash cd
                _dashUp += UpgradeInts.dasherIncr;
                _dashCD -= 0.03f;
                break;
            case 1: //increase backstab dmg
                _behindUp += UpgradeInts.assasinIncr;
                break;
            case 2: //first damage reduced
                _decFirstUp += UpgradeInts.vanguardIncr;
                break;
            case 3: //heal per room
                _roomHealUp += UpgradeInts.undeadIncr;
                break;
            case 4: //increased darkness
                //increases damage taken
                if (_darkUp == 0)
                {
                    _dmgMod += 0.5f;
                }

                _darkUp += UpgradeInts.gamblerIncr;
                break;
            case 5: //kill to heal
                _killHealUp += UpgradeInts.slayerIncr;
                break;
        }
    }

    //Check levels of upgrades
    public int GetPermUpgrade(int upgrade)
    {
        switch (upgrade)
        {
            case 0: //reduce dash cd
                return _dashUp;
            case 1: //increase backstab dmg
                return _behindUp;
            case 2: //first damage reduced
                return _decFirstUp;
            case 3: //heal per room
                return _roomHealUp;
            case 4: //increased darkness
                return _darkUp;
            case 5: //kill to heal
                return _killHealUp;
        }

        return 0;
    }

    //upgrades temp stats
    public void TempUpgrade(int upgrade)
    {
        switch (upgrade)
        {
            case 0: //reduce dmg in dash
                _safeDashUp += UpgradeInts.safeDashIncr;
                break;
            case 1: //reduce trap dmg taken
                _antiTrapUp += UpgradeInts.antiTrapIncr;
                break;
            case 2: //increase max HP
                _meatyUp += UpgradeInts.meatyIncr;
                _damageable.MaxHealth += 25;
                _damageable.Heal(25);
                break;
            case 3: //reduce dmg taken after kill, seconds
                _vigilantUp += UpgradeInts.vigilantIncr;
                break;
            case 4: //increase dmg dealt midair
                _gliderUp += UpgradeInts.gliderIncr;
                break;
            case 5: //deal more damage
                _beastUp += UpgradeInts.beastIncr;
                break;
            case 6: //deal more after kill
                _rampageUp += UpgradeInts.rampageIncr;
                break;
            case 7: //increase MS after dash
                _surferUp += UpgradeInts.surferIncr;
                break;
            case 8: //decrease MS of enemies after dash
                _gravitonUp += UpgradeInts.gravitonIncr;
                break;
            case 9: //dodge chance
                _swiftyUp += UpgradeInts.swiftyIncr;
                break;
            case 10: //revive
                _damageable.Heal(_damageable.MaxHealth / 2);
                break;
            case 11: //heal 50% HP
                _immortalUp += UpgradeInts.immortalIncr;
                break;
            case 12: //decrease dmg taken
                _solidUp += UpgradeInts.solidIncr;
                break;
        }
    }

    //Check levels of upgrades
    public int GetTempUpgrade(int upgrade)
    {
        switch (upgrade)
        {
            case 0:
                return _safeDashUp;
            case 1:
                return _antiTrapUp;
            case 2:
                return _meatyUp;
            case 3:
                return _vigilantUp;
            case 4:
                return _gliderUp;
            case 5:
                return _beastUp;
            case 6:
                return _rampageUp;
            case 7:
                return _surferUp;
            case 8:
                return _gravitonUp;
            case 9:
                return _swiftyUp;
            case 10:
                return 0;
            case 11:
                return _immortalUp;
            case 12:
                return _solidUp;
        }

        return 0;
    }


    //Heal after each room
    public void RoomHeal()
    {
        if (_roomHealUp > 0)
        {
            _damageable.Heal(_roomHealUp);
        }
    }

    //Heal after kill
    public void KillHeal()
    {
        if (_killHealUp > 0)
        {
            if (Random.Range(0, 100f) < 25)
            {
                _damageable.Heal(_killHealUp);
            }
        }
    }

    //reduce first damage
    public void Vanguard()
    {
        if (_decFirstUp > 0)
        {
            _damageable.Vanguard(_decFirstUp / 100f);
        }
    }


    //activates countdown for vigilant
    public IEnumerator ActivateVigilant()
    {
        if (_vigilantUp > 0)
        {
            _vigilantActive = true;
            //Wait for x seconds
            yield return new WaitForSeconds(_vigilantUp);
            _vigilantActive = false;
        }
    }

    //activates countdown for rampage
    public IEnumerator ActivateRampage()
    {
        if (_rampageUp > 0)
        {
            _rampageActive = true;
            //Wait for x seconds
            yield return new WaitForSeconds(_rampageUp);
            _rampageActive = false;
        }
    }

    //calculates the temporary damage taken mods
    public float CalcTempMod()
    {
        float _tempDmgMod = _solidUp;
        if (Dashing)
        {
            _tempDmgMod += _safeDashUp;
        }

        if (_vigilantActive)
        {
            _tempDmgMod += 50;
        }

        return _tempDmgMod / 100f;
    }

    //calculates the temporary damage dealt mods
    public float CalcTempDmg()
    {
        float _tempDmgMod = _beastUp;
        if (!_touchingDirections.IsGrounded)
        {
            _tempDmgMod += _gliderUp;
        }

        if (_rampageActive)
        {
            _tempDmgMod += 10;
        }

        return _tempDmgMod;
    }

    //reset all temp upgrades
    private void ResetTempUpgrades()
    {
        _safeDashUp = _antiTrapUp = _meatyUp = _vigilantUp = _gliderUp = _beastUp = _rampageUp = _surferUp
            = _gravitonUp = _swiftyUp = _immortalUp = _solidUp = 0;
    }

    //enter stasis
    public void Stasis()
    {
        if (IsAlive)
        {
            Destroy(gameObject);
        }
    }

    //does trap damage
    public void DealTrapDamage(int damage)
    {
        if (_antiTrapUp > 0)
        {
            damage = Convert.ToInt32(damage * ((100 - _antiTrapUp) / 100f));
        }

        _damageable.Hit(damage, Vector2.zero);
    }
}