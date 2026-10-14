namespace VersusPlus.Patches;
#pragma warning disable
// D1GQ - Fume-shroom Piercing Logic Fix [Old Code]:
// https://github.com/D1GQ/ReplantedOnline/commit/17c472c1fcf00d568b7e47209285e94d2ced7cfb

// D1GQ - Hypnotized Zombie Fix [Old Code]
// https://github.com/D1GQ/ReplantedOnline/blob/3f1fe17378ec0c98ab9866aa74bb775a3fd71ff6/src/Patches/Reloaded/Gameplay/Versus/Zombies/ZombiePatch.cs#L13

// D1GQ - Initial brain production Fix
// https://github.com/D1GQ/ReplantedOnline/commit/c7fc482433a6c4d36640e84c5eca3ae4e6733c38




using VersusPlus.Core;
using static VersusPlus.Core.Core;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using HarmonyLib;

using Il2Cpp;

using Il2CppBest.HTTP.SecureProtocol.Org.BouncyCastle.Bcpg;

using Il2CppReloaded;
using Il2CppReloaded.Data;
using Il2CppReloaded.DataModels;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using Il2CppReloaded.TreeStateActivities;

using Il2CppSource.Binders;
using Il2CppSource.Controllers;
using Il2CppSource.DataModels;

using Il2CppSystem.Numerics;

using Il2CppTMPro;

using MelonLoader;

using ReplantAPI.Managers;

using UnityEngine;
using UnityEngine.UI;

using static Il2CppReloaded.Constants;


using Common = Il2CppReloaded.Gameplay.Common;
using Vector2 = UnityEngine.Vector2;
using Zombie = Il2CppReloaded.Gameplay.Zombie;

using System.Runtime.CompilerServices;
using Il2CppReloaded.Services.StreamPlayService;

public sealed class Patch2 : MelonMod
{
    public override void OnInitializeMelon()
    {
        this.LoggerInstance.Msg("Loading Versus Important patches...");

    }

    // Hypnotized zombies will not eat target and gravestone
    // Fume-shroom will not hit thru gravestones and targets behind one
    // D1GQ
    [HarmonyPatch]
    internal static class ZombieEat_Fumeshroom_Patch
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
        private static void Zombie_Fumeshroom_Postfix(Zombie __instance, ref Rect __result)
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


    // Initial brain production ranging from 3.6 - 7.2 seconds 
    // D1GQ
    [HarmonyPatch]
    internal static class Gravestone_InitBrainRate_Patch
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
                __instance.mPhaseCounter = UnityEngine.Random.Range(900, 1800);
            }
        }
    }


    // Gravestones and Targets don't take splash damage
    [HarmonyPatch]
    internal static class Gravestone_Target_SplashDamagePatch
    {
        [HarmonyPatch(typeof(Projectile), nameof(Projectile.IsZombieHitBySplash))]
        [HarmonyPostfix]
        private static void Projectile_IsZombieHitBySplash_Postfix(Projectile __instance, Zombie theZombie, ref bool __result)
        {
            if (__instance.mProjectileType != ProjectileType.Melon &&
            __instance.mProjectileType != ProjectileType.Wintermelon) return;

            // Prevent target zombies and gravestones from taking splash damage
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




    // Puff-shroom no longer attacks Target Zombie when on column 6 or less
    // Fume-shroom no longer attacks Target Zombie when on column 5 or less
    [HarmonyPatch(typeof(Plant), nameof(Plant.FindTargetZombie))]
    public static class Plant_FindTargetZombie_Puffshroom_Fumeshroom_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Plant __instance, ref Zombie __result)
        {
            if (__instance.mSeedType != SeedType.Puffshroom)
                return;

            if (__result != null && __result.mZombieType == ZombieType.Target)
            {
                if (__instance.mPlantCol <= 5)
                {
                    __result = null;
                }
            }
        }


        [HarmonyPostfix]
        public static void Postfix2(Plant __instance, ref Zombie __result)
        {
            if (__instance.mSeedType != SeedType.Fumeshroom)
                return;

            if (__result != null && __result.mZombieType == ZombieType.Target)
            {
                if (__instance.mPlantCol <= 4)
                {
                    __result = null;
                }
            }

        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.FindTargetZombie))]
    public static class PultTargetingPatch
    {
        // Catapult plants prioritize zombies over gravestones

        [HarmonyPostfix]
        public static void Postfix(Plant __instance, ref Zombie __result)
        {
            if (__instance.mBoard == null || !__instance.mBoard.mApp.IsVersusMode()) return;

            if (!(__instance.mSeedType == SeedType.Cabbagepult ||
                  __instance.mSeedType == SeedType.Kernelpult ||
                  __instance.mSeedType == SeedType.Melonpult ||
                  __instance.mSeedType == SeedType.Wintermelon)) return;

            if (__result == null || 
            (__result.mZombieType != ZombieType.Gravestone && __result.mZombieType != ZombieType.Target))
                return;

            Zombie bestRealZombie = null;
            float lowestX = 99999f;

            foreach (var zombie in __instance.mBoard.GetZombies()) // Thank you D1GQ for GetZombies extension
            {
                if (zombie == null || zombie.mDead || zombie.mRow != __instance.mRow)
                    continue;

                if (zombie.mZombieType == ZombieType.Gravestone || 
                    zombie.mZombieType == ZombieType.Target|| 
                    zombie.mZombieType == ZombieType.Balloon)
                    continue;

                // Make sure the zombie is actually in front of the plant
                if (zombie.mPosX > __instance.mX)
                {
                    // Find the one furthest left
                    if (zombie.mPosX < lowestX)
                    {
                        lowestX = zombie.mPosX;
                        bestRealZombie = zombie;
                    }
                }
            }

            // If null then result would be Gravestone/Target
            if (bestRealZombie != null)
            {
                __result = bestRealZombie;
            }
        }


    }


    [HarmonyPatch(typeof(Projectile), nameof(Projectile.FindCollisionTarget))]
    internal static class xd{
        [HarmonyPostfix]
        public static void Postfix(Projectile __instance, ref Zombie __result)
        {
            if (__instance.mMotionType != ProjectileMotion.Lobbed) return;


            if (__result != null &&
               (__result.mZombieType == ZombieType.Gravestone || __result.mZombieType == ZombieType.Target)
               && isRealZombieInRow(__instance.mApp.m_board, __result.mRow))
            {
                __result = null;
            }
        }
    }


    public static bool isRealZombieInRow(Board board, int row)
    {
        if (board == null) return false;

        foreach (var zombie in board.GetZombies())
        {
            if (zombie == null || zombie.mDead || zombie.mRow != row)
                continue;

            if (zombie.mZombieType != ZombieType.Gravestone &&
                zombie.mZombieType != ZombieType.Target &&
                zombie.mZombieType != ZombieType.Balloon)
            {
                return true;
            }
        }

        return false;
    }

    // Select Sunflower to swap
    // ts just mumbo jumbo code, need to fix
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



    
}
