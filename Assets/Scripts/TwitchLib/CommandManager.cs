﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Unity;
using TwitchLib.Client.Models;

public class CommandManager : MonoBehaviour
{
    public PlayerManager playerManager;
    public BotManager botManager;
    Player player;
    Animator animator;

    public void CheckCommand(int id, string name, string command, List<string> arg, Client client)
    {
        var refDictionary = playerManager.playerReferences;
        command = StringFormatter(command);


        for (int i = 0; i < arg.Count; i++)
        {
            arg[i] = StringFormatter(arg[i]);
        }

        if (refDictionary.ContainsKey(id))
        {
            player = refDictionary[id];
            animator = player.GetComponent<Animator>();
        }

        switch (command)
        {
            case "Farm":
                if (PlayerCheck())
                {
                    player.playerAnimController.CancelAnimations(animator);
                    player.HandleFarming(arg[0]);
                }
                break;
            case "Craft":
                if (PlayerCheck() && CraftingRecipes.recipes.ContainsKey(arg[0]))
                {
                    player.playerAnimController.CancelAnimations(animator);
                    player.HandleCrafting(arg[0]);
                }
                break;
            case "Join":
                HandleJoin(id, name, client);
                break;
            case "Ping":
                if (PlayerCheck())
                    player.transform.GetChild(3).gameObject.SetActive(true);
                break;
            case "Depot":
                if (PlayerCheck())
                {
                    player.playerAnimController.CancelAnimations(animator);
                    player.CancelFarming();
                    player.GetComponent<GAgent>().beliefs.ModifyState("depot", 1);
                }
                break;
            case "Worldinv":
                if (PlayerCheck())
                    HandleWorldInv(id, client, name);
                break;
            case "Inv":
                if (PlayerCheck())
                    HandleInv(id, client, name);
                break;
            case "Place":
                if (PlayerCheck())
                    if (player.placeableItemManager.placeableItems.ContainsKey(arg[0]))
                        player.PlaceItem(arg[0]);
                break;
            case "Sit":
                if (PlayerCheck() && player.isStanding)
                {
                    player.playerAnimController.CancelAnimations(animator);
                    player.SitDown();
                }
                break;
            case "Stand":
                if (PlayerCheck() && !player.isStanding)
                {
                    player.playerAnimController.CancelAnimations(animator);
                    player.SitDown();
                }
                break;
            case "Wave":
                if (PlayerCheck() && player.isStanding)
                    player.WaveHello();
                break;
            case "Follow":
                if (PlayerCheck())
                {
                    player.playerAnimController.CancelAnimations(animator);
                    player.isFollowing = !player.isFollowing;
                }
                break;
            case "Cancel":
                if (PlayerCheck())
                    player.GetComponent<GAgent>().CancelGoap();
                break;
            case "Test":
                HandleTest(name, id);
                break;
            case "Bot":
                botManager.OnBotCommand(arg);
                break;
            default:
                break;
        }
    }

    private bool PlayerCheck()
    {
        return !player.isDead && player != null;
    }

    private string StringFormatter(string command){
        var formatString = char.ToUpper(command[0]) + command.Substring(1);
        return formatString;
        
    }

    public void HandleJoin(int id, string name, Client client) {
        if (!playerManager.players.ContainsKey(id))
        {
            var playerModel = playerManager.CreatePlayerModel(name, id);
            playerManager.AddToPlayersDictionary(playerModel);
        }
        else
        {
            // need to get a limit increase for whispering capabilities
            // client.SendWhisper(name, $"Hi, {name}, you have already joined");
            client.SendMessage(client.JoinedChannels[0], $"Hi, {name}, you have already joined our neighborhood");
        }
    }

    public void HandleWorldInv(int id, Client client, string name) {
        var worldInv = playerManager.playerReferences[id].inventory.ListWorldInventory();
        client.SendMessage(client.JoinedChannels[0], $" {worldInv}");

    }

    public void HandleInv(int id, Client client, string name){
        var player = playerManager.playerReferences[id];
        var inventory = player.inventory;
        var items = inventory.ListInventory();
        var invSpace = inventory.invSpace;

        client.SendMessage(client.JoinedChannels[0], $"{name}, you have {invSpace} slots available");

        if (items.Length > 3)
            client.SendMessage(client.JoinedChannels[0], $"{items}");
    }

    public void HandleTest(string name, int id)
    {
        if (!playerManager.players.ContainsKey(id))
        {
            var playerModel = playerManager.CreatePlayerModel(name, id);
            playerManager.AddToPlayersDictionary(playerModel);
            playerManager.PlayerSpawn(playerModel);
        }
    }

    // public void HandlePlace(int id, Client client, string name) {

    // }

}