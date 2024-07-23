using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DetectionZone : MonoBehaviour
{
    //list of detected colliders within the zone
    public List<Collider2D> detectedColliders = new List<Collider2D>();
    public Vector3 playerPos;
    private Collider2D _col;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        detectedColliders.Add(collision);
        
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        playerPos = collision.transform.position;
    }
    
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        detectedColliders.Remove(collision);
    }
}
