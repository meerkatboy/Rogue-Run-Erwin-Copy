using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PermUpgrades : MonoBehaviour
{
    //player
    private GameObject _player;
    private PlayerController _playerController;

    //levels of upgrades
    private int[] _stats = new int[6];
    private int[] _levels = new int[6];

    //costs of upgrades
    private int[] _commonCost = new int[] { 50, 100, 200, 300, 500 };
    private int[] _rareCost = new int[] { 100, 200, 300, 500, 1000 };
    private int[] _mythicCost = new int[] { 300, 500, 1000, 1500, 2000 };

    //list of upgrades
    private Upgrade[] _upgrades = new Upgrade[]
    {
        new Upgrade
        {
            Name = "Dasher", Description = "Decrease the delay between dashes by X%. \n Currently V% \n\n Z"
        },
        new Upgrade
        {
            Name = "Assassin", Description = "Increases damage to enemies from behind by X%. \n Currently V% \n\n Z"
        },
        new Upgrade
        {
            Name = "Vanguard",
            Description = "Decreases the first instance of damage taken per room by X%. \n Currently V% \n\n Z"
        },
        new Upgrade { Name = "Undead", Description = "Heal for XHP after each room.  \n Currently VHP \n\n Z" },
        new Upgrade
        {
            Name = "Gambler",
            Description = "You earn X% more darkness, but take twice the damage.  \n Currently V% \n\n Z"
        },
        new Upgrade
        {
            Name = "Slayer",
            Description = "You have a X% chance to gain 5HP after each enemy killed.  \n Currently V% \n\n Z"
        },
    };

    private void OnEnable()
    {
        //gets the player component
        _player = GameObject.FindGameObjectWithTag("Player");
        //controller
        _playerController = _player.GetComponent<PlayerController>();
        ButtonsSet();
    }


    [SerializeField] private Button Upgrade_button1;
    [SerializeField] private Button Upgrade_button2;
    [SerializeField] private Button Upgrade_button3;
    [SerializeField] private Button Upgrade_button4;
    [SerializeField] private Button Upgrade_button5;
    [SerializeField] private Button Upgrade_button6;

    [SerializeField] private TMP_Text Upgrade_DescriptionText1;
    [SerializeField] private TMP_Text Upgrade_DescriptionText2;
    [SerializeField] private TMP_Text Upgrade_DescriptionText3;
    [SerializeField] private TMP_Text Upgrade_DescriptionText4;
    [SerializeField] private TMP_Text Upgrade_DescriptionText5;
    [SerializeField] private TMP_Text Upgrade_DescriptionText6;


    public void ButtonsSet()
    {
        //gets the current levels
        for (int i = 0; i < 6; i++)
        {
            _stats[i] = _playerController.GetPermUpgrade(i);
            _levels[i] = _stats[i] / UpgradeInts.permArr[i];
        }

        // Setting text
        Upgrade_button1.transform.GetChild(0).GetComponent<TMP_Text>().text = _upgrades[0].Name;
        Upgrade_button2.transform.GetChild(0).GetComponent<TMP_Text>().text = _upgrades[1].Name;
        Upgrade_button3.transform.GetChild(0).GetComponent<TMP_Text>().text = _upgrades[2].Name;
        Upgrade_button4.transform.GetChild(0).GetComponent<TMP_Text>().text = _upgrades[3].Name;
        Upgrade_button5.transform.GetChild(0).GetComponent<TMP_Text>().text = _upgrades[4].Name;
        Upgrade_button6.transform.GetChild(0).GetComponent<TMP_Text>().text = _upgrades[5].Name;


        Upgrade_DescriptionText1.text = DescriptionString(_upgrades[0], 0);
        Upgrade_DescriptionText2.text = DescriptionString(_upgrades[1], 1);
        Upgrade_DescriptionText3.text = DescriptionString(_upgrades[2], 2);
        Upgrade_DescriptionText4.text = DescriptionString(_upgrades[3], 3);
        Upgrade_DescriptionText5.text = DescriptionString(_upgrades[4], 4);
        Upgrade_DescriptionText6.text = DescriptionString(_upgrades[5], 5);


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

        Upgrade_button4.GetComponent<Image>().color = rarityColors[_levels[3]];
        if (_levels[3] == 5)
        {
            Upgrade_button4.interactable = false;
        }

        Upgrade_button5.GetComponent<Image>().color = rarityColors[_levels[4]];
        if (_levels[4] == 5)
        {
            Upgrade_button5.interactable = false;
        }

        Upgrade_button6.GetComponent<Image>().color = rarityColors[_levels[5]];
        if (_levels[5] == 5)
        {
            Upgrade_button6.interactable = false;
        }
    }

    // UPGRADES
    public void UpgradeChosen(int upgrade)
    {
        //common upgrades
        if (upgrade <= 2)
        {
            //if have enough money, buy
            if (_playerController.DarknessCount >= _commonCost[_levels[upgrade]])
            {
                _playerController.DarknessCount -= _commonCost[_levels[upgrade]];
                _playerController.PermUpgrade(upgrade);
            }
        }
        //rare upgrades
        else if (upgrade <= 3)
        {
            //if have enough money, buy
            if (_playerController.DarknessCount >= _rareCost[_levels[upgrade]])
            {
                _playerController.DarknessCount -= _rareCost[_levels[upgrade]];
                _playerController.PermUpgrade(upgrade);
            }
        }
        //mythic upgrades
        else if (upgrade <= 5)
        {
            //if have enough money, buy
            if (_playerController.DarknessCount >= _mythicCost[_levels[upgrade]])
            {
                _playerController.DarknessCount -= _mythicCost[_levels[upgrade]];
                _playerController.PermUpgrade(upgrade);
            }
        }
    }

    private String CostString(int upgrade)
    {
        if (_levels[upgrade] == 5)
        {
            return "<color=\"purple\"> Maxed";
        }

        String str;
        //common upgrades
        if (upgrade <= 2)
        {
            str = _commonCost[_levels[upgrade]].ToString() + " darkness";
            //if have enough money, green txt
            if (_playerController.DarknessCount >= _commonCost[_levels[upgrade]])
            {
                return "<color=\"green\">" + str;
            }
            else //else red
            {
                return "<color=\"red\">" + str;
            }
        }
        //rare upgrades
        else if (upgrade <= 3)
        {
            str = _rareCost[_levels[upgrade]].ToString() + " darkness";
            //if have enough money, green txt
            if (_playerController.DarknessCount >= _rareCost[_levels[upgrade]])
            {
                return "<color=\"green\">" + str;
            }
            else
            {
                return "<color=\"red\">" + str;
            }
        }
        //mythic upgrades
        else if (upgrade <= 5)
        {
            str = _mythicCost[_levels[upgrade]].ToString() + " darkness";
            //if have enough money, make text green
            if (_playerController.DarknessCount >= _mythicCost[_levels[upgrade]])
            {
                return "<color=\"green\">" + str;
            }
            else
            {
                return "<color=\"red\">" + str;
            }
        }

        return "";
    }


    private class Upgrade
    {
        public string Name { get; set; }
        public string Description { get; set; }
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
            str = upgrade.Description.Replace("X", (_stats[i] + UpgradeInts.tempArr[i]).ToString());
        }


        str = str.Replace("V", _stats[i].ToString());

        str = str.Replace("Z", CostString(i));


        return str;
    }
}