using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;
    public Transform followTarget;

    //start pos of parallax object 
    private Vector2 _startingPosition;

    //start Z value of object
    private float _startingZ;

    //distance of camera moved from start
    private Vector2 CamMoveSinceStart => followTarget != null
        ? (Vector2)cam.transform.position - _startingPosition
        : cam.transform.position;

    private float ZDistanceFromTarget => followTarget != null
        ? transform.position.z - followTarget.transform.position.z
        : transform.position.z;

    //checks if in front or behind the player
    private float ClippingPlane =>
        (cam.transform.position.z + (ZDistanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane));

    //Further away -> faster it will move
    private float ParallaxFactor => Mathf.Abs(ZDistanceFromTarget) / ClippingPlane;

    // Start is called before the first frame update
    void Start()
    {
        _startingPosition = transform.position;
        _startingZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (followTarget == null)
        {
            Destroy(this);
        }

        Vector2 newPosition = _startingPosition + CamMoveSinceStart * ParallaxFactor;

        transform.position = new Vector3(newPosition.x, newPosition.y, _startingZ);
    }
}