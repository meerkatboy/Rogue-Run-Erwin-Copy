using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private TextMeshProUGUI _textMeshPro; //text object
    private Vector3 _moveSpeed = new Vector3(0, 75, 0); //speed of text moving up
    private RectTransform _textTransform; //transform of textbox

    private float _fadeTime = 1f; //time for the text to fade out
    private float _elapsedTime = 0; //time elapsed
    private Color _startColour; //starting colour


    private void Awake()
    {
        //setup the text
        _textTransform = GetComponent<RectTransform>();
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        _startColour = _textMeshPro.color;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //moves text up
        _textTransform.position += _moveSpeed * Time.deltaTime;

        //fading
        _elapsedTime += Time.deltaTime;
        //as long as fading
        if (_elapsedTime < _fadeTime)
        {
            float fadeAlpha = _startColour.a * (1 - (_elapsedTime / _fadeTime));
            _textMeshPro.color = new Color(_startColour.r, _startColour.g, _startColour.b, fadeAlpha);
        }
        else //otherwise remove text
        {
            Destroy(gameObject);
        }
    }
}