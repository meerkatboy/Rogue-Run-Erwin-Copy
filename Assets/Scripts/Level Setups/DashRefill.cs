using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DashRefill : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if colliding with a player。
        if (collision.CompareTag("Player"))
        {
            //add 1 dash
            collision.gameObject.GetComponent<PlayerController>().AddDash(1, gameObject);
        }
    }
}