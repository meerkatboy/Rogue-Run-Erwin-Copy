using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    //canvas
    public Canvas canvas;

    //player
    private GameObject _player;
    private PlayerController _playerController;

    //health bar
    public TMP_Text healthText; //text of hp
    public Image healthBar; //green bar of hp
    public TMP_Text livesText; //text of lives remaining
    private Damageable _damageable; //player's damageable component

    //floating text
    public GameObject damageTextPrefab; //damage text
    public GameObject healTextPrefab; //healing text
    public GameObject dropTextPrefab; //drop text

    //currencies
    public TMP_Text darknessText; //text of darkness count

    private void Awake()
    {
        //gets the player component
        _player = GameObject.FindGameObjectWithTag("Player");
        //gets damageable component
        _damageable = _player.GetComponent<Damageable>();
        //controller
        _playerController = _player.GetComponent<PlayerController>();
        //gets canvas
        canvas = FindObjectOfType<Canvas>();
    }

    private void OnEnable()
    {
        CharacterEvents.CharacterDamaged += CharacterTookDamage;
        CharacterEvents.CharacterHealed += CharacterHeals;
        CharacterEvents.CharacterDropped += CharacterDrops;
    }

    private void OnDisable()
    {
        CharacterEvents.CharacterDamaged -= CharacterTookDamage;
        CharacterEvents.CharacterHealed -= CharacterHeals;
        CharacterEvents.CharacterDropped -= CharacterDrops;
    }

    // Start is called before the first frame update
    void Start()
    {
        healthBar.fillAmount = _damageable.Health / 100f; //sets green hp bar
        healthText.text = _damageable.Health + "/" + _damageable.MaxHealth; //sets text for hp
        darknessText.text = _playerController.DarknessCount.ToString();
        livesText.text = _playerController.LivesRemaining.ToString(); //sets lives remaining
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = _damageable.Health / 100f; //sets green hp bar
        healthText.text = _damageable.Health + "/" + _damageable.MaxHealth; //sets text for hp
        darknessText.text = _playerController.DarknessCount.ToString();
        livesText.text = _playerController.LivesRemaining.ToString(); //sets lives remaining
    }

    //when something takes damage, floating text
    public void CharacterTookDamage(GameObject character, int damage)
    {
        //create text where the character is
        Vector3 spawnPos = Camera.main.WorldToScreenPoint(character.transform.position);

        //instantiates the text at where it should be
        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity, canvas.transform)
            .GetComponent<TMP_Text>();
        //sets text
        damage *= -1;
        tmpText.text = damage.ToString();
    }

    //when something heals, floating text
    public void CharacterHeals(GameObject character, int heal)
    {
        //create text where the character is
        Vector3 spawnPos = Camera.main.WorldToScreenPoint(character.transform.position);

        //instantiates the text at where it should be
        TMP_Text tmpText = Instantiate(healTextPrefab, spawnPos, Quaternion.identity, canvas.transform)
            .GetComponent<TMP_Text>();
        //sets text
        tmpText.text = heal.ToString();
    }

    //when picks up darkness, floating text
    public void CharacterDrops(GameObject character, int amount)
    {
        //create text where the character is
        Vector3 spawnPos = Camera.main.WorldToScreenPoint(character.transform.position);

        //instantiates the text at where it should be
        TMP_Text tmpText = Instantiate(dropTextPrefab, spawnPos, Quaternion.identity, canvas.transform)
            .GetComponent<TMP_Text>();
        //sets text
        tmpText.text = "+" + amount + "darkness";
    }
}