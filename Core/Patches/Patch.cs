namespace VersusPlus.Patches;
#pragma warning disable

// D1GQ - Gravestone Size Fix
// https://github.com/D1GQ/ReplantedOnline/blob/main/src/Network/Reloaded/Client/Object/Gameplay/ZombieComponents/GravestoneNetworkComponent.cs

// D1GQ - Music Horde Fix
// https://github.com/D1GQ/ReplantedOnline/blob/c5f30dd28cd59929a9513178ca8a6ec8103c0c3e/src/Patches/Reloaded/Gameplay/Versus/BoardPatch.cs

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
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

using Il2CppSpine.Unity;

using Il2CppSystem.Numerics;

using Il2CppTekly.Localizations;

using Il2CppTMPro;

using MelonLoader;

using ReplantAPI.Managers;

using UnityEngine;
using UnityEngine.UI;

using VersusPlus.Core;
using VersusPlus.UI;



using static Il2CppReloaded.Constants;
using static MelonLoader.MelonLogger;
using static VersusPlus.Core.Core;

using Common = Il2CppReloaded.Gameplay.Common;
using Vector2 = UnityEngine.Vector2;
using Zombie = Il2CppReloaded.Gameplay.Zombie;

public sealed class Patch : MelonMod
{
    public override void OnInitializeMelon()
    {
        this.LoggerInstance.Msg("Loading Versus patches...");

    }
    // Allow new units
    [HarmonyPatch(typeof(SeedChooserScreen), nameof(SeedChooserScreen.SeedNotAllowedToPick))]
    internal static class SeedChooserDataModelPatch
    {

        [HarmonyPrefix]
        private static bool Prefix4(SeedChooserScreen __instance, SeedType theSeedType, ref bool __result)
        {
            if (__instance.mApp.IsVersusMode() &&
            (theSeedType == SeedType.ZombieBalloon ||
            theSeedType == SeedType.ZombieImp ||
            theSeedType == SeedType.Gatlingpea ||
            theSeedType == SeedType.Twinsunflower ||
            theSeedType == SeedType.Wintermelon ||
            theSeedType == SeedType.Flowerpot ||
            theSeedType == SeedType.Blover ||
            theSeedType == SeedType.Plantern ||
            theSeedType == SeedType.Marigold ||
            theSeedType == SeedType.Gloomshroom ||
            theSeedType == SeedType.GoldMagnet ||
            theSeedType == SeedType.Spikerock ||
            theSeedType == SeedType.Cattail ||
            theSeedType == SeedType.Cobcannon
            ))
            {
                __result = false;
                return false;
            }

            return true;
        }

    }

    // Flag Flower Pot as Recommended

    [HarmonyPatch(typeof(SeedChooserScreen), nameof(SeedChooserScreen.SeedNotRecommendedToPick))]
    internal static class SeedChooserScreen_FlowerPot_Recommended
    {
        [HarmonyPrefix]
        private static bool Prefix(SeedChooserScreen __instance, ref RecommentedFlags __result, SeedType theSeedType)
        {
            if (__instance.mApp.IsVersusMode() && (theSeedType == SeedType.Flowerpot))
            {
                __result = RecommentedFlags.None;
                return false;
            }
            return true;
        }
    }

    


    // Toggle: Target and Gravestone don't take more than 35 damage per hit
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






    // i dont remember if this is useful, but i swear if i delete it something bad happens
    [HarmonyPatch(typeof(FocusCircleController), nameof(FocusCircleController.OnEnable))]
    public static class SuppressFocusPatch
    {
        public static void Postfix(FocusCircleController __instance)
        {
            if (Core.activateonce)
            {
                // Find "Canvas/Black Background" under the FocusCircleController root
                var bgTransform = __instance.transform.Find("Canvas/Black Background");
                if (bgTransform != null)
                {
                    bgTransform.gameObject.SetActive(false);
                }
            }
        }

    }


    // Makes gravestone look larger
    // D1GQ
    [HarmonyPatch(typeof(Zombie), nameof(Zombie.ZombieInitialize))]
    public static class GravestoneInitPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Zombie __instance)
        {
            if (__instance == null || __instance.Pointer == System.IntPtr.Zero)
                return;

            if (__instance.mZombieType == ZombieType.Gravestone)
            {
                
                __instance.mController.m_scale = new(1.2f, 1.2f);
                __instance.mController.m_visualOffset = new(125f, -335, 0f);
                __instance.mController.m_shadowController.m_shadowImageOffset = new(-0f, -0f, 0f);
                __instance.mController.m_shadowController.transform.localScale = new(0f, 0f, 0f);
                /*
                var rect2 = __instance.mZombieRect;
                rect2.x += 14f;
                __instance.mZombieRect = rect2;
                */
                __instance.mZombieRect = new(50f, 50f, __instance.mZombieRect.width, __instance.mZombieRect.height);
            }

            if (__instance.mZombieType == ZombieType.Target){
                __instance.mZombieRect = new(50f, 50f, __instance.mZombieRect.width, __instance.mZombieRect.height);
            }
            //var rect = __instance.mZombieRect;
            //rect.y += 55f;
            //__instance.mZombieRect = rect;

        }
    }


    
    // Gravestone glows before producing brains
    [HarmonyPatch(typeof(Zombie), nameof(Zombie.UpdateGravestone))]
    internal static class GravestoneGlow_Patch
    {

        [HarmonyPostfix]
        private static void Postfix(Zombie __instance)
        {
            if (__instance == null) return;
            if (__instance.mZombieType != ZombieType.Gravestone) return;
            if (!__instance.mApp.IsVersusMode()) return;

            if (__instance.mZombiePhase == (ZombiePhase)100 && 
                __instance.mPhaseCounter <= 400)
            {
                __instance.mJustGotShotCounter = 100;
            }

        }
    }


    // Fix upgraded plants offset (except Winter Melon)
    [HarmonyPatch(typeof(Plant), nameof(Plant.PlantInitialize))]
    public static class PlantInitPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Plant __instance)
        {
           if (__instance == null || __instance.Pointer == System.IntPtr.Zero)
                return;

           if ((__instance.mSeedType == SeedType.Twinsunflower ||
           __instance.mSeedType == SeedType.Gatlingpea ||
           __instance.mSeedType == SeedType.Gloomshroom ||
           __instance.mSeedType == SeedType.Cattail ||
           __instance.mSeedType == SeedType.Spikerock ||
           __instance.mSeedType == SeedType.GoldMagnet ||
           __instance.mSeedType == SeedType.Cobcannon ||
           __instance.mSeedType == SeedType.Imitater) && __instance.mController != null)
           {
               UnityEngine.Vector3 currentOffset = __instance.mController.m_visualOffset;
    
               currentOffset.x += 15f;

                __instance.mController.m_visualOffset = currentOffset;
           }
        }
    }

    // Coffee bean size patch
    [HarmonyPatch(typeof(PlantController), nameof(PlantController.OnInit))]
    public static class PlantControllerOffsetPatch
    {
        public static void Postfix(EffectController __instance)
        {
            if (__instance.gameObject.name.Contains("CoffeeBean"))
            {
                __instance.VisualOffset = new UnityEngine.Vector3(125f, -135f, 0f);
                __instance.m_scale = new UnityEngine.Vector2(1.2f, 1.2f);
            }
        }

        public static void Postfix(PlantController __instance)
        {
            if (__instance.gameObject.name.Contains("SplitPea"))
            {
                //__instance.AnimationController.SkeletonAnimation.timeScale = 0.7f;
                /*
                foreach (var x in __instance.AnimationController.SkeletonAnimation.state.tracks){
                    if (x != null && x.Animation != null && x.Animation.Name.Contains("head_idle")){
                        x.timeScale = 1f;
                       
                    }
                    if (x != null && x.Animation != null && x.Animation.Name.Contains("idle"))
                    {
                        x.timeScale = 1f;
                        
                    }
                }
                */
            }
            
        }
    }

    // Fix Wall-nut bite particles offset
    [HarmonyPatch(typeof(EffectController), nameof(EffectController.OnEnable))]
    public static class EffectControllerOffsetPatch
    {
        public static void Postfix(EffectController __instance)
        {
            if (__instance.gameObject.name.Contains("P_WallnutEatSmall"))
            {
                __instance.VisualOffset = new UnityEngine.Vector3(23f, -20f, 0f);
            }
            else if (__instance.gameObject.name.Contains("P_FumeShroomCloud"))
            {
                __instance.VisualOffset = new UnityEngine.Vector3(75f, 11f, 0f);
            }
            else if (__instance.gameObject.name.Contains("P_PeaSplat"))
            {
                float random = UnityEngine.Random.Range(32, 39);
                __instance.VisualOffset = new UnityEngine.Vector3(-34f, -random, 0f);
            }
            else if (__instance.gameObject.name.Contains("P_SnowPeaSplat"))
            {
                float random = UnityEngine.Random.Range(32, 39);
                __instance.VisualOffset = new UnityEngine.Vector3(-34f, -random, 0f);
            }
            else if (__instance.gameObject.name.Contains("P_SnowPeaPuff"))
            {
                __instance.VisualOffset = new UnityEngine.Vector3(0f, -15f, 0f);
            }
            else if (__instance.gameObject.name.Contains("P_Wintermelon"))
            {
                //__instance.VisualOffset = new UnityEngine.Vector3(-25f, -100f, 0f);
            }
            else if (__instance.gameObject.name.Contains("P_MelonSplash"))
            {
                //__instance.VisualOffset = new UnityEngine.Vector3(-25f, -100f, 0f);
            }
            else if (__instance.gameObject.name.Contains("P_GraveStoneRise(Clone)"))
            {
                __instance.VisualOffset = new UnityEngine.Vector3(20f, -10f, 0f);
            }
            else if (__instance.gameObject.name.Contains("P_ZombieRise(Clone)"))
            {
                __instance.m_scale = new Unity.Mathematics.float2(1.1f, 1.1f);
            }

        }
    }

    [HarmonyPatch(typeof(ProjectileController), nameof(ProjectileController.OnInit))]
    public static class ProjectileControllerOffsetPatch
    {
        public static void Postfix(ProjectileController __instance)
        {
            if (__instance.gameObject.name.Contains("Projectile-Pea"))
            {
                __instance.VisualOffset = new UnityEngine.Vector3(0f, -15f, 0f);
            }
            else if (__instance.gameObject.name.Contains("Projectile-Snowpea"))
            {
                __instance.VisualOffset = new UnityEngine.Vector3(0f, -15f, 0f);
            }
            else if (__instance.gameObject.name.Contains("Projectile-Fireball"))
            {
                __instance.VisualOffset = new UnityEngine.Vector3(100f, -115f, 0f);
                //__instance.m_projectile.mShadowOffsetX
            }
            else if (__instance.gameObject.name.Contains("Projectile-Star"))
            {
                //__instance.gameObject. = new Unity.Mathematics.float2(-50f, 3f);
                //__instance.m_projectile.GetProjectileRect();

            }

            //P_Reanim_JalapenoFire_FirePea
            //P_Reanim_JalapenoFire
        }
    }


    // Nerf starfruit shotgun behaviour
    [HarmonyPatch(typeof(Projectile), nameof(Projectile.GetProjectileRect))]
    public static class ProjectileRectPatch
    {
        public static void Postfix(Projectile __instance, ref Rect __result)
        {
            if (__instance.mProjectileType == ProjectileType.Star)
            {
                if (!BloomConfig.TestBoolInput14.Value) return;
                float scale = 0.9f; // shrink to 0.9x size

                float newWidth = __result.width * scale;
                float newHeight = __result.height * scale;

                float offsetX = (__result.width - newWidth) / 2f;
                float offsetY = (__result.height - newHeight) / 2f;

                __result = new Rect(__result.x + offsetX,
                                    __result.y + offsetY,
                                    newWidth,
                                    newHeight);
            }
        }
    }




    [HarmonyPatch(typeof(ReanimationController), nameof(ReanimationController.OnInit))]
    public static class ReanimationControllerOffsetPatch
    {
        public static void Postfix(ProjectileController __instance)
        {
            if (__instance.gameObject.name.Contains("P_Reanim_JalapenoFire"))
            {
                __instance.VisualOffset = new UnityEngine.Vector3(210f, -285f, 0f);
            }

            //P_Reanim_JalapenoFire_FirePea
            //P_Reanim_JalapenoFire
        }
    }

    // Camera doesn't pan when zombie walk into house anymore
    [HarmonyPatch(typeof(Zombie), nameof(Zombie.WalkIntoHouse))]
    public static class BoardMovePatch
    {
        public static bool isZombieWalkIntoHouse = false;
        public static void Prefix()
        {
            isZombieWalkIntoHouse = true;
        }

    }

    [HarmonyPatch(typeof(Board), nameof(Board.Move))]
    public static class FreezeCameraPatch
    {
        // Returning false skips the original Move method => locking camera (because i like it)
        public static bool Prefix()
        {
            if (BoardMovePatch.isZombieWalkIntoHouse) {
                BoardMovePatch.isZombieWalkIntoHouse = false;
                return false; 
            }
            return true;

        }
    }
    


    // Changes text and its properties of sth
    [HarmonyPatch(typeof(TMP_Text), nameof(TMP_Text.text), MethodType.Setter)]
    public static class TMPTextSetterPatch
    {
        public static void Prefix(TMP_Text __instance, ref string value)
        {
            if (__instance.gameObject.name == "Header"  &&
                __instance.gameObject.transform.parent != null &&
                __instance.gameObject.transform.parent.name == "SeedChooser_Player2")
            {
                value = "pik ur zombeez";
                __instance.color = new UnityEngine.Color(0f, 0.9f, 0f, 1f);
            }




            // Change P1 name in picking side screen to Player 1 name
            if (__instance.gameObject.name == "PlayerNumber1" &&
                __instance.gameObject.transform.parent != null &&
                __instance.gameObject.transform.parent.name == "Selected" &&
                __instance.gameObject.transform.parent.parent != null &&
                __instance.gameObject.transform.parent.parent.name == "SidePlants")
            {
                value = BloomConfig.Player1Name.Value;

                __instance.enableAutoSizing = true;

                __instance.fontSizeMin = 100;
                __instance.fontSizeMax = 100;

                __instance.rectTransform.sizeDelta = new UnityEngine.Vector2(383.8374f, 175.8698f);
            
                __instance.rectTransform.localPosition = new UnityEngine.Vector3(-60f, 530f, 0f);
            }

            if (__instance.gameObject.name == "PlayerNumber1" &&
                __instance.gameObject.transform.parent != null &&
                __instance.gameObject.transform.parent.name == "Selected" &&
                __instance.gameObject.transform.parent.parent != null &&
                __instance.gameObject.transform.parent.parent.name == "SideZombies")
            {
                value = BloomConfig.Player1Name.Value;

                __instance.enableAutoSizing = true;

                __instance.fontSizeMin = 100;
                __instance.fontSizeMax = 100;

                __instance.rectTransform.sizeDelta = new UnityEngine.Vector2(383.8374f, 175.8698f);

                __instance.rectTransform.localPosition = new UnityEngine.Vector3(90f, 530f, 0f);
            }




            // Change P2 name in picking side screen to Player 2 name
            if (__instance.gameObject.name == "PlayerNumber2" &&
                __instance.gameObject.transform.parent != null &&
                __instance.gameObject.transform.parent.name == "Selected" &&
                __instance.gameObject.transform.parent.parent != null &&
                __instance.gameObject.transform.parent.parent.name == "SidePlants")
            {
                value = BloomConfig.Player2Name.Value;

                __instance.enableAutoSizing = true;

                __instance.fontSizeMin = 100;
                __instance.fontSizeMax = 100;

                __instance.rectTransform.sizeDelta = new UnityEngine.Vector2(383.8374f, 175.8698f);

                __instance.rectTransform.localPosition = new UnityEngine.Vector3(-60f, 530f, 0f);
            }

            if (__instance.gameObject.name == "PlayerNumber2" &&
                __instance.gameObject.transform.parent != null &&
                __instance.gameObject.transform.parent.name == "Selected" &&
                __instance.gameObject.transform.parent.parent != null &&
                __instance.gameObject.transform.parent.parent.name == "SideZombies")
            {
                value = BloomConfig.Player2Name.Value;

                __instance.enableAutoSizing = true;

                __instance.fontSizeMin = 100;
                __instance.fontSizeMax = 100;

                __instance.rectTransform.sizeDelta = new UnityEngine.Vector2(383.8374f, 175.8698f);

                __instance.rectTransform.localPosition = new UnityEngine.Vector3(90f, 530f, 0f);
            }



            // Change player name in winning screen
            if (__instance.gameObject.name == "HeaderLabel" &&
                __instance.gameObject.transform.parent != null &&
                __instance.gameObject.transform.parent.name == "Player1" &&
                __instance.gameObject.transform.parent.parent != null &&
                __instance.gameObject.transform.parent.parent.name == "NamePlates")
            {
                value = BloomConfig.Player1Name.Value;
            }

            if (__instance.gameObject.name == "HeaderLabel" &&
                __instance.gameObject.transform.parent != null &&
                __instance.gameObject.transform.parent.name == "Player2" &&
                __instance.gameObject.transform.parent.parent != null &&
                __instance.gameObject.transform.parent.parent.name == "NamePlates")
            {
                value = BloomConfig.Player2Name.Value;
            }


            return;
        }
        
    }


    // Fix music horde
    // D1GQ
    [HarmonyPatch(typeof(Board), nameof(Board.CountZombiesOnScreen))]
    public static class CountZombies{
        [HarmonyPrefix]
        public static bool Board_CountZombiesOnScreen_Prefix(Board __instance, ref int __result)
        {
            if (__instance == null) return true;
            if (!__instance.mApp.IsVersusMode()) return true;

            int count = 0;
            if (__instance.mApp.VersusMode.Phase == VersusPhase.SuddenDeath) count = 10; // Force to play horde after sudden death

            foreach (var zombie in __instance.GetZombies())
            {
                if (zombie.mZombieType == ZombieType.Gravestone || zombie.mZombieType == ZombieType.Target)
                    continue;

                if (!zombie.IsDeadOrDying() && zombie.IsOnBoard())
                {
                    count++;
                }
            }

            __result = count;

            return false;
        }
    }
    




}
