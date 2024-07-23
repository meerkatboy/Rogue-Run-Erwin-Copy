using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TempUpgrades : MonoBehaviour
{
    //player
    private GameObject _player;
    private PlayerController _playerController;

    //list of UI active upgrades
    private List<int> availableUpgrades;

    private bool firstTime = true;

    //list of upgrades
    Upgrade[] _upgrades = new Upgrade[]
    {
        new Upgrade { Name = "SafeDash", Description = "Decrease damage taken while dashing by X%. \n Currently V%" },
        new Upgrade { Name = "AntiTrap", Description = "Decrease damage dealt by traps by X%. \n Currently V%" },
        new Upgrade { Name = "Meaty", Description = "Increase max health by XHP. \n Currently VHP" },
        new Upgrade { Name = "Vigilant", Description = "Decrease the delay between dashes by X%. \n Currently V%" },
        new Upgrade { Name = "Glider", Description = "Increase damage dealt midair by X%. \n Currently V%" },
        new Upgrade { Name = "Beast", Description = "Increase damage dealt by X%. \n Currently V%" },
        new Upgrade
        {
            Name = "Rampage",
            Description = "Increase damage dealt after a kill by 40% for X seconds. \n Currently V seconds"
        },
        new Upgrade
        {
            Name = "Surfer", Description = "Increase move speed by X% briefly after a dash. \n Currently V%"
        },
        new Upgrade
        {
            Name = "Graviton",
            Description =
                "Decrease movement speed of nearby enemies after dashing by 40% for X seconds. \n Currently V seconds"
        },
        new Upgrade { Name = "Swifty", Description = "Gain a X% dodge chance against enemies. \n Currently V%" },
        new Upgrade { Name = "Heal", Description = "Instantly heal 50% of your max health." },
        new Upgrade
        {
            Name = "Immortal", Description = "+1 revive after dying at 10% max health. \n Currently V revives."
        },
        new Upgrade { Name = "Solid", Description = "Decrease damage taken by X%. \n Currently V%" }
    };

    //levels of upgrades
    private int[] _stats = new int[3];
    private int[] _levels = new int[3];


    [SerializeField] private Button Upgrade_button1;
    [SerializeField] private Button Upgrade_button2;
    [SerializeField] private Button Upgrade_button3;

    [SerializeField] private TMP_Text Upgrade_DescriptionText1;
    [SerializeField] private TMP_Text Upgrade_DescriptionText2;
    [SerializeField] private TMP_Text Upgrade_DescriptionText3;


    public void OnEnable()
    {
        //gets the player component
        _player = GameObject.FindGameObjectWithTag("Player");
        //controller
        _playerController = _player.GetComponent<PlayerController>();
        ButtonsSet(firstTime);
        if (firstTime)
        {
            firstTime = !firstTime;
        }
    }

    public void ButtonsSet(bool shuffle)
    {
        //randomizing the upgrades shown
        if (shuffle)
        {
            availableUpgrades = new List<int>();
            int number;
            for (int i = 0; i < 3; i++)
            {
                do
                {
                    number = Random.Range(0, _upgrades.Length);
                } while (availableUpgrades.Contains(number));

                availableUpgrades.Add(number);
            }
            //availableUpgrades.Add(11); testing purposes
        }


        Upgrade Upgrade_1 = _upgrades[availableUpgrades[0]];
        Upgrade Upgrade_2 = _upgrades[availableUpgrades[1]];
        Upgrade Upgrade_3 = _upgrades[availableUpgrades[2]];

        //gets the current levels
        for (int i = 0; i < 3; i++)
        {
            _stats[i] = _playerController.GetTempUpgrade(availableUpgrades[i]);
            _levels[i] = _stats[i] / UpgradeInts.tempArr[availableUpgrades[i]];
        }

        // Setting text
        Upgrade_button1.transform.GetChild(0).GetComponent<TMP_Text>().text = Upgrade_1.Name;
        Upgrade_button2.transform.GetChild(0).GetComponent<TMP_Text>().text = Upgrade_2.Name;
        Upgrade_button3.transform.GetChild(0).GetComponent<TMP_Text>().text = Upgrade_3.Name;

        // Replacing the string with current and next level value
        Upgrade_DescriptionText1.text = DescriptionString(Upgrade_1, 0);
        Upgrade_DescriptionText2.text = DescriptionString(Upgrade_2, 1);
        Upgrade_DescriptionText3.text = DescriptionString(Upgrade_3, 2);

        // Setting color of the buttons
        Dictionary<int, Color> rarityColors = new Dictionary<int, Color>();
        rarityColors.Add(0, new Color(1, 1, 1, 1));
        rarityColors.Add(1, new Color(0.5f, 1f, 0.5f, 1));
        rarityColors.Add(2, new Color(0f, 0.5f, 1f, 1));
        rarityColors.Add(3, new Color(0.3f, 0f, 0.5f, 1));
        rarityColors.Add(4, new Color(1f, 1f, 0f, 1));
        rarityColors.Add(5, new Color(1f, 0.5f, 0.2f, 1));


        //sets colors, disable if needed
        Upgrade_button1.GetComponent<Image>().color = rarityColors[_levels[0]];
        if (_levels[0] == 5)
        {
            Upgrade_button1.interactable = false;
        }

        Upgrade_button2.GetComponent<Image>().color = rarityColors[_levels[1]];
        if (_levels[1] == 5)
        {
            Upgrade_button2.interactable = false;
        }

        Upgrade_button3.GetComponent<Image>().color = rarityColors[_levels[2]];
        if (_levels[2] == 5)
        {
            Upgrade_button3.interactable = false;
        }
    }

    // UPGRADES
    public void UpgradeChosen(int upgrade)
    {
        _playerController.TempUpgrade(availableUpgrades[upgrade]);
    }


    //builds the string for the description box of upgrades
    private string DescriptionString(Upgrade upgrade, int i)
    {
        string str = "";
        if (_levels[i] == 5)
        {
            str = upgrade.Description.Replace("X", (_stats[i]).ToString());
        }
        else
        {
            str = upgrade.Description.Replace("X", (_stats[i] + UpgradeInts.tempArr[availableUpgrades[i]]).ToString());
        }

        str = str.Replace("V", _stats[i].ToString());

        return str;
    }

    //temp upgrades only show up once
    public void Hide()
    {
        ButtonsSet(false);
        Upgrade_button1.interactable = false;
        Upgrade_button2.interactable = false;
        Upgrade_button3.interactable = false;
    }

    public class Upgrade
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}