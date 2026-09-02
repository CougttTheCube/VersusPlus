namespace VersusPlus;
#pragma warning disable
using System;
using System.Collections.Generic;

using HarmonyLib;

using Il2CppReloaded;
using Il2CppReloaded.Data;
using Il2CppReloaded.DataModels;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using Il2CppReloaded.TreeStateActivities;

using Il2CppSource.Controllers;
using Il2CppSource.DataModels;

using Il2CppSystem.Numerics;

using Il2CppTMPro;

using MelonLoader;

using UnityEngine;

using static VersusPlus.Core;

using Common = Il2CppReloaded.Gameplay.Common;
using Vector2 = UnityEngine.Vector2;
using Zombie = Il2CppReloaded.Gameplay.Zombie;

// Todo:
// - Add Balloon, Imp, Blover (done)
// - Port over the patches
// - Fix initial recharge
// - Add a timer
// - Detect the controllers
// - Fix Random Mode
// - (maybe) Add toggle button for Versus tooltips
public sealed class Patch : MelonMod
{
    public override void OnInitializeMelon()
    {
        this.LoggerInstance.Msg("Loading Versus patches...");

    }

    // Flag Flower Pot as Recommended
    [HarmonyPatch(typeof(SeedChooserScreen), nameof(SeedChooserScreen.SeedNotRecommendedToPick))]
    internal static class SeedChooserScreen_FlowerPot_Recommended
    {
        [HarmonyPrefix]
        private static bool Prefix(SeedChooserScreen __instance, ref RecommentedFlags __result, SeedType theSeedType)
        {
            if (__instance.mApp.IsVersusMode() && theSeedType == SeedType.Flowerpot)
            {
                __result = RecommentedFlags.None;
                return false;
            }
            return true;
        }
    }

    // Hypnotized zombies will not eat target and gravestone
    // Fume-shroom will not hit thru gravestones and targets behind one
    // D1GQ
    [HarmonyPatch]
    internal static class ZombiePatch
    {
        [HarmonyPatch(typeof(Zombie), nameof(Zombie.EatZombie))]
        [HarmonyPrefix]
        private static bool Zombie_EatZombie_Prefix(Zombie __instance, Zombie theZombie)
        {
            if (__instance.mMindControlled)
            {
                if (theZombie.mZombieType == ZombieType.Gravestone ||
                theZombie.mZombieType == ZombieType.Target)
                {
                    return false;
                }
            }
            return true;
        }



        [HarmonyPatch]
        [HarmonyPatch(typeof(Zombie), nameof(Zombie.GetZombieRect))]
        [HarmonyPostfix]
        private static void Zombie_GetZombieRect_Postfix(Zombie __instance, ref Rect __result)
        {
            // Make Target Zombies and Gravestones invulnerable when behind another gravestone
            // This is a direct fix to Fumeshroom OP piercing logic
            if (__instance.mZombieType != ZombieType.Gravestone && __instance.mZombieType != ZombieType.Target) return;

            foreach (var gravestone in __instance.mBoard.m_vsGravestones)
            {
                // Check if Gravestone is in the same row of zombie 
                if (gravestone == __instance) continue;
                if (gravestone.mRow != __instance.mRow) continue;

                // Check if Gravestone is in front of zombie
                if (gravestone.mPosX < __instance.mPosX)
                {
                    if (gravestone.mZombiePhase != ZombiePhase.RisingFromGrave)
                    {
                        __result.position = new Vector2(-9999f, -9999f);
                        break;

                    }
                }
            }
        }
    }


    // Target and Gravestone don't take more than 35 damage per hit
    [HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
    internal static class TakeDamage_NoMoreThan35
    {
        [HarmonyPrefix]
        private static void Zombie_TakeDamage_Prefix(Zombie __instance, ref int theDamage, DamageFlags theDamageFlags)
        {

            if (__instance.mZombieType != ZombieType.Gravestone && __instance.mZombieType != ZombieType.Target) return;
            if (!BloomConfig.TestBoolInput7.Value) return;
            if (theDamage > 35) theDamage = 35;

        }
    }



    // Select Sunflower to swap
    [HarmonyPatch(typeof(ChosenSeed), nameof(ChosenSeed.Selected))]
    internal static class ChosenSeedPatch
    {
        public static bool hasRun = false;
        [HarmonyPrefix]

        private static void Prefix(SeedChooserScreen seedChooserScreen, int playerIndex, ChosenSeed __instance)
        {
            if (!seedChooserScreen.mApp.IsVersusMode()) return;
            if (hasRun) return;

            if (playerIndex == 0)
            {
                playerIndex = 1;

            }
            else if (playerIndex == 1)
            {
                playerIndex = 0;

            }
            hasRun = true;
            //GameplayActivity.VersusMode.Phase = VersusPhase.ChooseZombiePacket;
            seedChooserScreen.mApp.VersusMode.Phase = VersusPhase.ChooseZombiePacket;


        }

    }

    [HarmonyPatch(typeof(LoadSceneActivity), nameof(LoadSceneActivity.IsDoneLoading))]
    internal static class W
    {
        [HarmonyPrefix]
        private static void Prefix()
        {
            ChosenSeedPatch.hasRun = false;
        }

        [HarmonyPostfix]
        private static void Postfix()
        {
            ChosenSeedPatch.hasRun = false;
        }
    }


    // Initial brain production ranging from (shorter) 6-12 seconds 
    // D1GQ
    [HarmonyPatch]
    internal static class GravestonePatch
    {
        [HarmonyPatch(typeof(Zombie), nameof(Zombie.UpdateGravestone))]
        [HarmonyPrefix]
        private static void Zombie_UpdateGravestone_Prefix(Zombie __instance)
        {
            if (!BloomConfig.TestBoolInput6.Value) return;
            if (__instance.mZombieType != ZombieType.Gravestone) return;

            if (__instance.mZombiePhase == ZombiePhase.ZombieNormal)
            {
                __instance.mZombiePhase = (ZombiePhase)100;
                __instance.mPhaseCounter = UnityEngine.Random.Range(600, 1200);
            }
        }
    }

    // Nerf brain rate
    // D1GQ

    [HarmonyPatch]
    internal static class VersusMode_GravestoneNerf_Patch
    {
        [HarmonyPatch(typeof(Zombie), nameof(Zombie.UpdateGravestone))]
        [HarmonyPrefix]

        private static void Zombie_UpdateGravestone_Prefix(Zombie __instance, ref bool __state)
        {
            __state = false;
            if (__instance.mApp.IsVersusMode())
            {
                if (__instance.mPhaseCounter <= 1 && BloomConfig.TestBoolInput6.Value)
                {
                    __state = true; // Mark to apply nerf in postfix
                } else {
                    __state = false;
                }
            }

        }

        [HarmonyPatch(typeof(Zombie), nameof(Zombie.UpdateGravestone))]
        [HarmonyPostfix]

        private static void Zombie_UpdateGravestone_Postfix(Zombie __instance, bool __state)
        {

            if (__instance.mApp.IsVersusMode())
            {
                if (__state)
                {
                   __instance.mPhaseCounter = UnityEngine.Random.Range(6787, 7500); // => 27.15 - 30 seconds
                }
            }

        }
    }


    // Gravestones and Targets don't take splash damage
    // D1GQ

    [HarmonyPatch]
    internal static class ProjectilePatch
    {
        [HarmonyPatch(typeof(Projectile), nameof(Projectile.IsZombieHitBySplash))]
        [HarmonyPostfix]
        private static void Projectile_IsZombieHitBySplash_Postfix(Projectile __instance, Zombie theZombie, ref bool __result)
        {
            if (__instance.mProjectileType != ProjectileType.Melon &&
            __instance.mProjectileType != ProjectileType.Wintermelon) return;

            // Prevent target zombies and gravestones from taking Melonpult splash damage
            if (theZombie.mZombieType == ZombieType.Gravestone || theZombie.mZombieType == ZombieType.Target)
            {

                if (__instance.mRow != theZombie.mRow)
                {
                    __result = false;
                    return;
                }
            }

        }
    }


    // RaptorRush
    // Seed packets don't show to not have enough cost while in seed chooser screen
    [HarmonyPatch(typeof(SeedBankDataModel), nameof(SeedBankDataModel.IsChoosing), MethodType.Getter)]
    internal static class IsChoosingVersusPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(SeedBankDataModel __instance, ref bool __result)
        {
            if (!__instance.m_gameplayActivity.IsVersusMode()) return true;
            // Replace .Ref() with the actual access method for your specific game version
            var activity = __instance.m_gameplayActivity;

            if (activity != null)
            {
                // Manual property checks for C# 7.3 compatibility
                bool isVersus = activity.ReloadedGameMode == ReloadedGameMode.Versus;
                bool isChoosingPhase = activity.VersusMode.Phase == VersusPhase.ChoosePlantPacket ||
                                       activity.VersusMode.Phase == VersusPhase.ChooseZombiePacket;

                if (isVersus && isChoosingPhase)
                {
                    __result = true;
                    return false; // Skip original method
                }
            }

            return true; // Run original method
        }
    }


    // Puff-shroom no longer attacks Target Zombie
    // Fume-shroom no longer attacks Target Zombie when on column 5
    [HarmonyPatch(typeof(Plant), nameof(Plant.FindTargetZombie))]
    public static class Plant_FindTargetZombie_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Plant __instance, ref Zombie __result)
        {
            if (__instance.mSeedType != SeedType.Puffshroom)
                return;

            if (__result != null && __result.mZombieType == ZombieType.Target)
                __result = null;
        }
            

        [HarmonyPostfix]
        public static void Postfix2(Plant __instance, ref Zombie __result)
        {
                if (__instance.mSeedType != SeedType.Fumeshroom)
                    return;

                if (__result != null && __result.mZombieType == ZombieType.Target)
                {
                    if (__instance.mPlantCol == 4)
                    {
                        __result = null;
                    }
                }
            
        }
    }



    //gemini, thank you
    [HarmonyPatch(typeof(Plant), nameof(Plant.FindTargetZombie))]
    public static class PultTargetingPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Plant __instance, ref Zombie __result)
        {

            if (__instance.mBoard == null || !__instance.mBoard.mApp.IsVersusMode()) return;
            bool isPult = __instance.mSeedType == SeedType.Cabbagepult ||
                          __instance.mSeedType == SeedType.Kernelpult ||
                          __instance.mSeedType == SeedType.Melonpult ||
                          __instance.mSeedType == SeedType.Wintermelon;
            if (!isPult) return;

            if (__result == null || (__result.mZombieType != ZombieType.Gravestone && __result.mZombieType != ZombieType.Target))
                return;

            // 3. The plant targeted a Gravestone. Let's see if a real zombie exists in this row.
            Zombie bestRealZombie = null;
            float lowestX = 99999f; // We want the zombie closest to the house (lowest X)

            foreach (var zombie in __instance.mBoard.GetZombies()) //thank you D1GQ for GetZombies extension
            {
                // Skip invalid, dead zombies, or zombies in other rows
                if (zombie == null || zombie.mDead || zombie.mRow != __instance.mRow)
                    continue;

                // Skip gravestones and targets
                if (zombie.mZombieType == ZombieType.Gravestone || zombie.mZombieType == ZombieType.Target)
                    continue;

                // Make sure the zombie is actually in front of the plant
                if (zombie.mPosX > __instance.mX)
                {
                    // Find the one furthest left (closest to the plant/house)
                    if (zombie.mPosX < lowestX)
                    {
                        lowestX = zombie.mPosX;
                        bestRealZombie = zombie;
                    }
                }
            }

            // 4. If we found a real zombie in the row, overwrite the result!
            // If we didn't find one, __result stays as the Gravestone.
            if (bestRealZombie != null)
            {
                __result = bestRealZombie;
            }
        }
    }



    // Getting rid of that annoying clicking seed packet sound
    // claude
    public static class SoundSuppressState
    {
        public static bool IsChooserOpen = false;
        public static bool IsPlaying = false;
        public static bool IsVersus = false;

        public static bool ShouldSuppress => IsChooserOpen || IsPlaying || IsVersus;
    }

    // --- Normal seed chooser ---

    [HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.ShowSeedChooserScreen))]
    public static class ShowSeedChooser_Patch
    {
        [HarmonyPostfix]
        public static void Postfix() => SoundSuppressState.IsChooserOpen = true;
    }

    [HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.HideSeedChooserScreen))]
    public static class HideSeedChooser_Patch
    {
        [HarmonyPostfix]
        public static void Postfix() => SoundSuppressState.IsChooserOpen = false;
    }

    // --- Normal gameplay ---

    [HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.StartPlaying))]
    public static class StartPlaying_Patch
    {
        [HarmonyPostfix]
        public static void Postfix() => SoundSuppressState.IsPlaying = true;
    }

    [HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.EndLevel))]
    public static class EndLevel_Patch
    {
        [HarmonyPostfix]
        public static void Postfix() => SoundSuppressState.IsPlaying = false;
    }

    [HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.KillBoard))]
    public static class KillBoard_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            SoundSuppressState.IsPlaying = false;
            SoundSuppressState.IsVersus = false;
        }
    }

    [HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.DoBackToMain))]
    public static class DoBackToMain_Patch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            SoundSuppressState.IsPlaying = false;
            SoundSuppressState.IsVersus = false;
        }
    }

    // --- Versus mode phase changes ---

    [HarmonyPatch(typeof(VersusMode), nameof(VersusMode.Phase), MethodType.Setter)]
    public static class VersusMode_Phase_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(VersusPhase value)
        {
            SoundSuppressState.IsVersus = value == VersusPhase.ChoosePlantPacket
                                       || value == VersusPhase.ChooseZombiePacket
                                       || value == VersusPhase.Gameplay
                                       || value == VersusPhase.SuddenDeath;
        }
    }

    // --- Sound suppression: mouse ---

    [HarmonyPatch(typeof(ButtonAudio), nameof(ButtonAudio.OnPointerDown))]
    public static class ButtonAudio_OnPointerDown_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix() => !SoundSuppressState.ShouldSuppress;
    }

    [HarmonyPatch(typeof(ButtonFoleyAudio), nameof(ButtonFoleyAudio.OnPointerDown))]
    public static class ButtonFoleyAudio_OnPointerDown_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix() => !SoundSuppressState.ShouldSuppress;
    }

    // --- Sound suppression: controller (OnSubmit = confirm button) ---

    [HarmonyPatch(typeof(ButtonAudio), nameof(ButtonAudio.OnSubmit))]
    public static class ButtonAudio_OnSubmit_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix() => !SoundSuppressState.ShouldSuppress;
    }

    [HarmonyPatch(typeof(ButtonFoleyAudio), nameof(ButtonFoleyAudio.OnSubmit))]
    public static class ButtonFoleyAudio_OnSubmit_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix() => !SoundSuppressState.ShouldSuppress;
    }


    
}
