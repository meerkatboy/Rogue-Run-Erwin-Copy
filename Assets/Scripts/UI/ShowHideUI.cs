using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideUI : MonoBehaviour
{
    [SerializeField] private GameObject UI;

    //player
    private GameObject _player;
    private PlayerController _playerController;

    private void Awake()
    {
        UI.SetActive(false);
        //gets the player component
        _player = GameObject.FindGameObjectWithTag("Player");
        //controller
        _playerController = _player.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if colliding with a player。
        if (collision.CompareTag("Player"))
        {
            //show upgrade UI
            UI.SetActive(true);
            //disable dash so player can click
            _playerController.CanDash = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if colliding with a player。
        if (collision.CompareTag("Player"))
        {
            //remove UI
            UI.SetActive(false);
            //reenable dashing
            _playerController.CanDash = true;
        }
    }
}