using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit; //damage and knockback

    [SerializeField] private int _maxHealth = 100; //max hp
    [SerializeField] private int _health = 100; //current hp

    private bool _isInvincible; //checks if character invincible
    private float _timeSinceHit = 0; //time since hit
    public float _invincibilityTime = 0.5f; //invincible after hit
    private int _dropMultiplier = 1; //multiplies drops
    private float _dmgMod = 1; //damage modifier
    private float _vanguardBuff = 0; //damage reduction from vangaurd
    private bool _hasVangaurd = false; //if the player is benefiting from vanguard

    private bool _isAlive = true;
    private bool _isPlayer = false;

    private Animator _animator;

    private PlayerController _playerController;

    public float InvincibleTime
    {
        get { return _invincibilityTime; }
        set { _invincibilityTime = value; }
    }

    public bool IsPlayer
    {
        get { return _isPlayer; }
        set { _isPlayer = value; }
    }

    public bool IsHit
    {
        get { return _animator.GetBool(AnimationStrings.isHit); }
        private set
        {
            //sometimes dies in the middle of setting.
            if (IsAlive)
            {
                _animator.SetBool(AnimationStrings.isHit, value);
            }
        }
    }

    public int MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }


    public int Health
    {
        get { return _health; }
        set
        {
            _health = value;
            if (_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    public bool IsAlive
    {
        get { return _isAlive; }
        set
        {
            _isAlive = value;
            _animator.SetBool(AnimationStrings.isAlive, value);
        }
    }

    //returns drop multiplier
    public int Multiplier
    {
        get { return _dropMultiplier; }
        set { _dropMultiplier = value; }
    }

    //returns damage mod
    public float DmgMod
    {
        get { return _dmgMod; }
        set { _dmgMod = value; }
    }


    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //if is invincible, check to remove the status
        if (_isInvincible)
        {
            if (_timeSinceHit > _invincibilityTime)
            {
                //can be hit
                _isInvincible = false;
                _timeSinceHit = 0;
            }

            _timeSinceHit += Time.deltaTime; //add the time increment.
        }
    }

    //hits the player
    public bool Hit(int damage, Vector2 knockback)
    {
        if (IsPlayer)
        {
            //perm dmg modifier minus the temporary damage reduced
            damage = Convert.ToInt32(damage * (_dmgMod - _playerController.CalcTempMod()));
            //swifty dodge chance
            if (_playerController.DodgeChance > 0)
            {
                int roll = Random.Range(1, 101);
                if (_playerController.DodgeChance >= roll)
                {
                    damage = 0;
                    //no knockback either
                    knockback = Vector2.zero;
                }
            }
        }

        if (IsAlive && !_isInvincible)
        {
            if (Health - damage < 0)
            {
                Health = 0;
            }
            else
            {
                if (damage > 0)
                {
                    Health -= damage;
                }
                else
                {
                    Health -= 0;
                }
            }

            _isInvincible = true;

            //notify other components damageable was hit and handle knockback.
            IsHit = true;
            damageableHit.Invoke(damage, knockback);
            //floating text call
            CharacterEvents.CharacterDamaged.Invoke(gameObject, damage);

            //if has vanguard and got hit, revert.
            if (_hasVangaurd && IsPlayer)
            {
                _hasVangaurd = false;
                _dmgMod += _vanguardBuff;
            }

            return true;
        }

        return false;
    }

    //heals the player
    public void Heal(int healing)
    {
        if (IsAlive)
        {
            //makes sure healing doesnt surpass max hp
            int maxHeal = Mathf.Max(MaxHealth - Health, 0);
            int actualHeal = Mathf.Min(maxHeal, healing);
            Health += actualHeal;
            if (actualHeal > 0)
            {
                CharacterEvents.CharacterHealed.Invoke(gameObject, healing);
            }
        }
    }

    //activates vanguard
    public void Vanguard(float buff)
    {
        if (!_hasVangaurd)
        {
            _hasVangaurd = true;
            _vanguardBuff = buff;
            _dmgMod -= _vanguardBuff;
        }
    }

    public void SetPlayer(PlayerController playerController)
    {
        _playerController = playerController;
        IsPlayer = true;
    }

    //kill self
    public void KillSelf()
    {
        if (IsAlive)
        {
            Hit(MaxHealth, Vector2.zero);
        }
    }
}