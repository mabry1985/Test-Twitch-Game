﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingCost : MonoBehaviour
{
    [SerializeField] BuildingSO building;
    public bool canBuild;
    Button button;
    List<KeyValuePair<string, int>> bCosts = new List<KeyValuePair<string, int>>(); 

    private void Start() 
    {
        bCosts = BuildCostList();
        button = GetComponent<Button>();    
    }

    private void Update() 
    {
        canBuild = CanBuild();
        print(canBuild);
        
        button.interactable = canBuild;
        
    }

    private bool CanBuild()
    {
        bool hasMats = false;
        foreach (KeyValuePair<string, int> item in bCosts)
        {
            print("material required " + item.Key + "|" + item.Value);
            if (item.Value == 0) break;

            if (GWorld.worldInventory.items[item.Key] >= item.Value)
            {
                hasMats = true;
            }
            else
            {
                hasMats = false;
            }
        }
        return hasMats;
    }

    private List<KeyValuePair<string, int>> BuildCostList()
    {
        List<KeyValuePair<string, int>> tempList = new List<KeyValuePair<string, int>>()
        {
            new KeyValuePair<string, int>("Wood", building.wood),
            new KeyValuePair<string, int>("Stone", building.stone),
            new KeyValuePair<string, int>("Iron", building.iron),

        };

        return tempList;
  
    }


}
