using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempUpgradeButton : MonoBehaviour
{
    [SerializeField] private TempUpgrades tempUpgradesScript;

    //upgrade
    public void Upgrade()
    {
        int upgradeChosen = Int32.Parse(gameObject.transform.GetChild(0).name);
        tempUpgradesScript.UpgradeChosen(upgradeChosen);
        tempUpgradesScript.Hide();
    }
}