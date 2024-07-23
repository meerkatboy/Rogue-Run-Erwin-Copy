using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TurretTrap : MonoBehaviour, ITrap
{
    [SerializeField] private float attackCooldown; //time between each fire
    [SerializeField] private Transform barrelPoint; //where the barrel is
    [SerializeField] private GameObject[] bullets;

    private float _timer;

    public bool shoot;
    private bool _recentlyFired;

    private PlayerController _playerController; //controller

    private void Update()
    {
        if (shoot && _timer > 0.2f)
        {
            bullets[FindBullet()].transform.position = barrelPoint.position;
            bullets[FindBullet()].GetComponent<EnemyProjectile>().Activate();
            _timer = 0;
        }

        _timer += Time.deltaTime;
    }

    //finds the first bullet that's not active
    private int FindBullet()
    {
        for (int i = 0; i < bullets.Length; i++)
        {
            //finds first bullet not active
            if (!bullets[i].activeInHierarchy)
            {
                return i;
            }
        }

        //else return 0;
        return 0;
    }

    public void DealTrapDamage(int damage, PlayerController playerController)
    {
        _playerController = playerController;
        DealTrapDamage(damage);
    }

    public void DealTrapDamage(int damage)
    {
        _playerController.DealTrapDamage(damage);
    }
}