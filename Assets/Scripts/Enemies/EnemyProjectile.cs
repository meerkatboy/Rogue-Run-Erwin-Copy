using System;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float resetTime;
    [SerializeField] public TurretTrap turretTrap; //turret trap script
    public int AD = 20; //damage dealt
    private float _lifetime;


    public void Activate()
    {
        _lifetime = 0;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        //moves bullet
        float moveSpeed = -speed * Time.deltaTime;
        transform.Translate(moveSpeed, 0, 0);

        //remove if pass lifetime
        _lifetime += Time.deltaTime;
        if (_lifetime > resetTime)
        {
            gameObject.SetActive(false);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            turretTrap.DealTrapDamage(AD, collision.GetComponent<PlayerController>());
            gameObject.SetActive(false);
        }
        else if (collision.tag == "Ground")
        {
            gameObject.SetActive(false);
        }
    }
}