using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor.Rendering.BuiltIn.ShaderGraph;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner confiner;
    [SerializeField] private Collider2D previousRoom;
    [SerializeField] private Collider2D nextRoom;

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if colliding with a player。
        if (collision.CompareTag("Player"))
        {
            /*moves camera to room depending on position coming from.
             Because it's OnTriggerExit, the player x value will be larger than trigger if going right*/
            if (collision.transform.position.x < transform.position.x)
            {
                confiner.m_BoundingShape2D = previousRoom;
            }
            else
            {
                confiner.m_BoundingShape2D = nextRoom;
            }
        }
    }
}
