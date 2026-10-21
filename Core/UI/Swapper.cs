namespace VersusPlus.UI;
#pragma warning disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using HarmonyLib;

using Il2CppBest.HTTP.Shared.Extensions;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

using Il2CppReloaded;
using Il2CppReloaded.Data;
using Il2CppReloaded.DataModels;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Input;
using Il2CppReloaded.Services;
using Il2CppReloaded.TreeStateActivities;

using Il2CppSource.Controllers;
using Il2CppSource.DataModels;

using Il2CppSteamworks;

using Il2CppSystem;
using Il2CppSystem.Numerics;

using Il2CppTekly.Localizations;

using Il2CppTMPro;
using Il2CppTMPro;

using MelonLoader;

using ReplantAPI;
using ReplantAPI.Managers;

using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

using VersusPlus.Core;

using static MelonLoader.MelonLogger;
using static VersusPlus.Core.Core;
using static VersusPlus.Patches.Patch;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class SpriteSwapper : MonoBehaviour
{

    public static void SwapSprite(string controllerName, int index, bool isPlant = true)
    {
        string bundlePath = Path.Combine(
                            Application.streamingAssetsPath,
                            "aa",
                            "StandaloneWindows64",
                            "defaultlocalgroup_assets_ui",
                            "sprites_5b5218033ecdc00622a31e94d7a6a379.bundle");

        // Get gameobject of empty plant bg
        if (isPlant){
            GameObject backgroundPlant = GameObject.Find(
                "Panels/VersusPanels/P_VsSideChooser/Canvas/Layout/Center/Panel/SidePlants/Background");

            if (backgroundPlant == null)
            {
                MelonLogger.Error("Background not found");
                return;
            }
            // get image
            var img = backgroundPlant.GetComponent<Image>();

            // get all plant sprite, because it's already loaded along w/ empty bg
            Sprite spritePlant = null;

            string nameUpper = controllerName?.ToUpperInvariant() ?? "";

            if (nameUpper.Contains("XBOX") || nameUpper.Contains("XINPUT"))
                { spritePlant = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(s => s.name == "SPR_Vs_Side_XboxOne_Plant"); }

            else if (nameUpper.Contains("PS4") || nameUpper.Contains("DUALSHOCK"))
                { spritePlant = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(s => s.name == "SPR_Vs_Side_PS4_Plant"); }

            else if (nameUpper.Contains("PS5") || nameUpper.Contains("DUALSENSE"))
                { spritePlant = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(s => s.name == "SPR_Vs_Side_PS5_Plant"); }

            else if (nameUpper.Contains("STEAMDECK"))
                { spritePlant = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(s => s.name == "SPR_Vs_Side_Steamdeck_Plant"); }

            else if (nameUpper.Contains("BLUE"))
                { spritePlant = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(s => s.name == "SPR_Vs_Side_Switch1Blue_Plant"); }

            else if (nameUpper.Contains("RED"))
                { spritePlant = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(s => s.name == "SPR_Vs_Side_Switch1Red_Plant"); }

            else 
                { spritePlant = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(s => s.name == "SPR_Vs_Side_Generic_Plant"); }


            Sprite spriteEmptyPlant = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(s => s.name == "SPR_Vs_Side_Empty_Plant");

            if (index > -1)
            {
                if (spritePlant != null) img.sprite = spritePlant;
                else MelonLogger.Error("Sprite not currently loaded.");
            }
            else
            {
                if (spriteEmptyPlant != null) img.sprite = spriteEmptyPlant;
                else MelonLogger.Error("Sprite not currently loaded.");
            }

        } else {
            GameObject backgroundPlant = GameObject.Find(
                    "Panels/VersusPanels/P_VsSideChooser/Canvas/Layout/Center/Panel/SideZombies/Background");

            if (backgroundPlant == null)
            {
                MelonLogger.Error("Background not found");
                return;
            }
            // get image
            var img = backgroundPlant.GetComponent<Image>();

            // get all plant sprite, because it's already loaded along w/ empty bg
            Sprite spritePlant = null;
            string nameUpper = controllerName.ToUpperInvariant() ?? "";

            if (nameUpper.Contains("XBOX") || nameUpper.Contains("XINPUT"))
                { spritePlant = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(s => s.name == "SPR_Vs_Side_XboxOne_Zombie"); }

            else if (nameUpper.Contains("PS4") || nameUpper.Contains("DUALSHOCK"))
                { spritePlant = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(s => s.name == "SPR_Vs_Side_PS4_Zombie"); }

            else if (nameUpper.Contains("PS5") || nameUpper.Contains("DUALSENSE"))
                { spritePlant = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(s => s.name == "SPR_Vs_Side_PS5_Zombie"); }

            else if (nameUpper.Contains("BLUE"))
                { spritePlant = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(s => s.name == "SPR_Vs_Side_Switch1Blue_Zombie"); }

            else if (nameUpper.Contains("RED"))
                { spritePlant = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(s => s.name == "SPR_Vs_Side_Switch1Red_Zombie"); }

            else
                { spritePlant = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(s => s.name == "SPR_Vs_Side_Generic_Zombie"); }


            Sprite spriteEmptyPlant = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(s => s.name == "SPR_Vs_Side_Empty_Zombie");


            if (index > -1)
            {
                if (spritePlant != null) img.sprite = spritePlant;
                else MelonLogger.Error("Sprite not currently loaded.");
            }
            else
            {
                if (spriteEmptyPlant != null) img.sprite = spriteEmptyPlant;
                else MelonLogger.Error("Sprite not currently loaded.");
            }
        }

        
        
    }





    public static class VersusSideTracker
    {
        // Tracks which input owns which side (input1 input2 null)
        public static string ActivePlantInput = null;
        public static string ActiveZombieInput = null;
    }
    
    public static void UpdatePlayerHelper(){

    }


    [HarmonyPatch(typeof(VersusDataModel), nameof(VersusDataModel.UpdatePlantsPlayer))]
    public static class UpdatePlantsPlayer
    {

        public static void Prefix(string plantsPlayer, string inputName, ref int inputIndex)
        {
            MelonLogger.Msg($"Plants player updated: {plantsPlayer} | Input: {inputName} | Index: {inputIndex}");
            if (inputIndex > -1)
            { 
                // plant is joined
                // if plant is claimed and another input tries to select it, return
                if (VersusSideTracker.ActivePlantInput != null && VersusSideTracker.ActivePlantInput != inputName)
                {
                    return;
                }
                VersusSideTracker.ActivePlantInput = inputName; 
            } 
            else {
                // plant is left
                // check if it was zombie's doing: if plant input (can be seen as previous) is different 
                if (VersusSideTracker.ActivePlantInput != null && VersusSideTracker.ActivePlantInput != inputName){
                    return;
                }
                VersusSideTracker.ActivePlantInput = null;
            }



            if (!isp0Gamepad) // if p0 is not gamepad and p1 is (mouse and controller)
            {
                if (!isp1Gamepad)
                {
                    if (inputName == "input1")
                    {
                        MelonLogger.Warning("p0 controller is null -> mouse player, using generic");
                        SpriteSwapper.SwapSprite("Generic", inputIndex);
                        

                    }
                }
                else
                {
                    if (inputName == "input1")
                    {
                        MelonLogger.Warning("p0 controller is null -> mouse player, using generic");
                        SpriteSwapper.SwapSprite("Generic", inputIndex);

                    }
                    else if (inputName == "input2")
                    {

                        string displayName = p1Pad.displayName ?? "";
                        MelonLogger.Msg($"Pad Only: {displayName}");

                        SpriteSwapper.SwapSprite(displayName, inputIndex);
                    }
                }


            }
            else if (isp0Gamepad)
            {

                if (!isp1Gamepad)
                {
                    if (inputName == "input1")
                    {
                        string displayName = p0Pad.displayName ?? "";
                        MelonLogger.Msg($"Pad 1: {displayName}");

                        SpriteSwapper.SwapSprite(displayName, inputIndex);

                    }
                }
                else 
                {
                    if (inputName == "input1")
                    {
                        string displayName = p0Pad.displayName ?? "";
                        MelonLogger.Msg($"Pad 1: {displayName}");

                        SpriteSwapper.SwapSprite(displayName, inputIndex);

                    }
                    else if (inputName == "input2")
                    {
                        string displayName = p1Pad.displayName ?? "";
                        MelonLogger.Msg($"Pad 2: {displayName}");

                        SpriteSwapper.SwapSprite(displayName, inputIndex);
                    }
                }
                
            }



        }
    }

    [HarmonyPatch(typeof(VersusDataModel), nameof(VersusDataModel.UpdateZombiesPlayer))]
    public static class UpdateZombiesPlayer
    {
        
        public static void Prefix(string zombiesPlayer, string inputName, ref int inputIndex)
        {
            MelonLogger.Msg($"Zombies player updated: {zombiesPlayer} | Input: {inputName} | Index: {inputIndex}");
            if (inputIndex > -1) {
                if (VersusSideTracker.ActiveZombieInput != null && VersusSideTracker.ActiveZombieInput != inputName)
                {
                    return;
                }
                VersusSideTracker.ActiveZombieInput = inputName; 
            } // plant is joined
            else
            {
                // plant is left
                // check if it was zombie's doing: if plant input (can be seen as previous) is different 
                if (VersusSideTracker.ActiveZombieInput != null && VersusSideTracker.ActiveZombieInput != inputName)
                {
                    return;
                }
                VersusSideTracker.ActiveZombieInput = null;
            }



            if (!isp0Gamepad) // if p0 is not gamepad and p1 is (mouse and controller)
            {
                if (!isp1Gamepad)
                {
                    if (inputName == "input1")
                    {
                        MelonLogger.Warning("p0 controller is null -> mouse player, using generic");
                        SpriteSwapper.SwapSprite("Generic", inputIndex, false);

                    }
                }
                else
                {
                    if (inputName == "input1")
                    {
                        MelonLogger.Warning("p0 controller is null -> mouse player, using generic");
                        SpriteSwapper.SwapSprite("Generic", inputIndex, false);

                    }
                    else if (inputName == "input2")
                    {

                        string displayName = p1Pad.displayName ?? "";
                        MelonLogger.Msg($"Pad Only: {displayName}");

                        SpriteSwapper.SwapSprite(displayName, inputIndex, false);
                    }
                }


            }
            else if (isp0Gamepad)
            {
                if (!isp1Gamepad){
                    if (inputName == "input1")
                    {
                        string displayName = p0Pad.displayName ?? "";
                        MelonLogger.Msg($"Pad 1: {displayName}");

                        SpriteSwapper.SwapSprite(displayName, inputIndex, false);

                    }
                }
                else 
                {
                    if (inputName == "input1")
                    {
                        string displayName = p0Pad.displayName ?? "";
                        MelonLogger.Msg($"Pad 1: {displayName}");

                        SpriteSwapper.SwapSprite(displayName, inputIndex, false);

                    }
                    else if (inputName == "input2")
                    {
                        string displayName = p1Pad.displayName ?? "";
                        MelonLogger.Msg($"Pad 2: {displayName}");

                        SpriteSwapper.SwapSprite(displayName, inputIndex, false);
                    }
                }
                
            }
        }
    }
}
