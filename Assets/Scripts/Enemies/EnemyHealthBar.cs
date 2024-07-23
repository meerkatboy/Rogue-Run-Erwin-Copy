using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponentInChildren<TMP_Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        if (Math.Sign(transform.parent.parent.transform.localScale.x) != Math.Sign(transform.localScale.x))
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }
    }

    //update hp
    public void UpdateHp(float currHp, float maxHp)
    {
        _text.text = currHp.ToString();
        slider.value = currHp / maxHp;
    }
}