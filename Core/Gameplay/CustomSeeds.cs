namespace VersusPlus.Gameplay;
#pragma warning disable

// gitAngelHub123 - Custom seed type implementation + patches for them
// https://github.com/gitAngelHub123/VersusBot/blob/main/Mapa.cs

// D1GQ - Text localization bypass [modified old code]
// https://github.com/D1GQ/ReplantedOnline/blob/8c60a8dc52dae74cd1f7ab512317c35fb99d5540/src/Patches/Reloaded/Client/Localization/FormattedLocalizationIdBinderPatch.cs#L8
using HarmonyLib;

using Il2CppInterop.Runtime;

using Il2CppReloaded.Binders;
using Il2CppReloaded.Data;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using Il2CppReloaded.TreeStateActivities;

using Il2CppSource.DataModels;

using Il2CppTekly.Localizations;

using MelonLoader;

using ReplantAPI.Managers;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

using static VersusPlus.Core.Core;
using static VersusPlus.Helper.CoreHelper;
using static VersusPlus.Patches.Patch3;



// Setting up Dr Zomboss custom seed type
// gitAngelHub123
public sealed class CustomSeedType : MelonMod
{
    internal static class BackupDancerSeed
    {
        internal static SeedType Id = (SeedType)1000;
        internal static PlantDefinition def;

        internal static void Init(PlantDefinition template)
        {
            def = UnityEngine.Object.Instantiate(template);
            def.hideFlags = HideFlags.DontUnloadUnusedAsset;
            def.m_seedType = Id;
            def.m_packetIndex = (int)Id;

            def.m_versusCost = 0;
            def.m_versusBaseRefreshTime = 3000;
            def.m_versusSuddenDeathRefreshTime = 1000;

            var backupDancerSprite = new AssetReferenceSprite("b2e616b9bbbacac4980ba6de852d1a44");
            backupDancerSprite.m_SubObjectName = "SPR_SeedPackets_Zombies_Versus_22";
            //backupDancerSprite.m_SubObjectType = "UnityEngine.Sprite, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
            def.m_versusImage = backupDancerSprite;

            var backupDancerAssetReferenceGameObject = new AssetReferenceGameObject("86af4c6f62e682647853b46a5fb99a9e");
            def.m_preview = backupDancerAssetReferenceGameObject;

            var backupDancerPreviewSprite = new AssetReferenceSprite("863154a01c416424e8a2d97037894d79");
            backupDancerPreviewSprite.m_SubObjectName = "SPR_Preview_Zombie_backup";
            def.m_previewSprite = backupDancerPreviewSprite;

            def.m_previewSpriteScale = 0.79f;
            def.m_previewSpriteOffset = new Vector2(80, -66);

            def.m_plantName = "raw:Backup Dancer";
            def.m_plantToolTip = $"raw:fallback";
        }
    }
    internal static class BobsledSeed
    {
        internal static SeedType Id = (SeedType)1001;
        internal static PlantDefinition def;

        internal static void Init(PlantDefinition template)
        {
            def = UnityEngine.Object.Instantiate(template);
            def.hideFlags = HideFlags.DontUnloadUnusedAsset;
            def.m_seedType = Id;
            def.m_packetIndex = (int)Id;

            def.m_versusCost = 100;
            def.m_versusBaseRefreshTime = 3000;
            def.m_versusSuddenDeathRefreshTime = 1200;

            var backupDancerSprite = new AssetReferenceSprite("b2e616b9bbbacac4980ba6de852d1a44"); // keep this
            backupDancerSprite.m_SubObjectName = "SPR_SeedPackets_Zombies_Versus_23";
            //backupDancerSprite.m_SubObjectType = "UnityEngine.Sprite, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
            def.m_versusImage = backupDancerSprite;


            // temporarily use the backup dancer preview image because giga garg doesnt have one :v
            // todo: load the tall nut zombie preview image
            //var backupDancerAssetReferenceGameObject = new AssetReferenceGameObject("86af4c6f62e682647853b46a5fb99a9e");
            //def.m_preview = backupDancerAssetReferenceGameObject;

            var backupDancerPreviewSprite = new AssetReferenceSprite("3c550c182bb720e4796fb82a81522ad6");
            backupDancerPreviewSprite.m_SubObjectName = "SPR_Preview_Zombie_bobsled";
            def.m_previewSprite = backupDancerPreviewSprite;

            def.m_previewSpriteScale = 1f;
            def.m_previewSpriteOffset = new Vector2(80, -66);

            def.m_plantName = "raw:Zombie Bobsled Team";
            def.m_plantToolTip = "raw:fallback";
        }
    }
    internal static class GigaGargantuarSeed
    {
        internal static SeedType Id = (SeedType)1002;
        internal static PlantDefinition def;

        internal static void Init(PlantDefinition template)
        {
            def = UnityEngine.Object.Instantiate(template);
            def.hideFlags = HideFlags.DontUnloadUnusedAsset;
            def.m_seedType = Id;
            def.m_packetIndex = (int)Id;

            def.m_versusCost = 475;
            def.m_versusBaseRefreshTime = 12000;
            def.m_versusSuddenDeathRefreshTime = 5000;


            var backupDancerSprite = new AssetReferenceSprite("b2e616b9bbbacac4980ba6de852d1a44"); // keep this
            backupDancerSprite.m_SubObjectName = "SPR_SeedPackets_Zombies_Versus_20";
            //backupDancerSprite.m_SubObjectType = "UnityEngine.Sprite, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
            def.m_versusImage = backupDancerSprite;


            // temporarily use the backup dancer preview image because giga garg doesnt have one :v
            // todo: load the tall nut zombie preview image
            var backupDancerAssetReferenceGameObject = new AssetReferenceGameObject("86af4c6f62e682647853b46a5fb99a9e");
            def.m_preview = backupDancerAssetReferenceGameObject;

            var backupDancerPreviewSprite = new AssetReferenceSprite("863154a01c416424e8a2d97037894d79");
            backupDancerPreviewSprite.m_SubObjectName = "SPR_Preview_Zombie_backup";
            def.m_previewSprite = backupDancerPreviewSprite;

            def.m_previewSpriteScale = 0.8f;
            def.m_previewSpriteOffset = new Vector2(80, -66);


            def.m_plantName = "raw:Giga-gargantuar";
            def.m_plantToolTip = "raw:fallback";
        }
    }
    internal static class PeashooterZombieSeed
    {
        internal static SeedType Id = (SeedType)1003;
        internal static PlantDefinition def;

        internal static void Init(PlantDefinition template)
        {
            def = UnityEngine.Object.Instantiate(template);
            def.hideFlags = HideFlags.DontUnloadUnusedAsset;
            def.m_seedType = Id;
            def.m_packetIndex = (int)Id;

            def.m_versusCost = 50;
            def.m_versusBaseRefreshTime = 1500;
            def.m_versusSuddenDeathRefreshTime = 500;


            var backupDancerSprite = new AssetReferenceSprite("b2e616b9bbbacac4980ba6de852d1a44"); // keep this
            backupDancerSprite.m_SubObjectName = "SPR_SeedPackets_Zombies_Versus_31";
            //backupDancerSprite.m_SubObjectType = "UnityEngine.Sprite, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
            def.m_versusImage = backupDancerSprite;


            // temporarily use the backup dancer preview image because peahead doesnt have one :v
            // todo: load the peashooter zombie preview image
            var backupDancerAssetReferenceGameObject = new AssetReferenceGameObject("86af4c6f62e682647853b46a5fb99a9e");
            def.m_preview = backupDancerAssetReferenceGameObject;

            var backupDancerPreviewSprite = new AssetReferenceSprite("863154a01c416424e8a2d97037894d79");
            backupDancerPreviewSprite.m_SubObjectName = "SPR_Preview_Zombie_backup";
            def.m_previewSprite = backupDancerPreviewSprite;

            def.m_previewSpriteScale = 0.8f;
            def.m_previewSpriteOffset = new Vector2(80, -66);

            def.m_plantName = "raw:Peashooter Zombie";
            def.m_plantToolTip = "raw:fallback";
        }
    }
    internal static class GatlingPeaZombieSeed
    {
        internal static SeedType Id = (SeedType)1004;
        internal static PlantDefinition def;

        internal static void Init(PlantDefinition template)
        {
            def = UnityEngine.Object.Instantiate(template);
            def.hideFlags = HideFlags.DontUnloadUnusedAsset;
            def.m_seedType = Id;
            def.m_packetIndex = (int)Id;

            def.m_versusCost = 200;
            def.m_versusBaseRefreshTime = 5000;
            def.m_versusSuddenDeathRefreshTime = 2000;


            var backupDancerSprite = new AssetReferenceSprite("b2e616b9bbbacac4980ba6de852d1a44"); // keep this
            backupDancerSprite.m_SubObjectName = "SPR_SeedPackets_Zombies_Versus_32";
            //backupDancerSprite.m_SubObjectType = "UnityEngine.Sprite, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
            def.m_versusImage = backupDancerSprite;


            // temporarily use the backup dancer preview image because gatling pea doesnt have one :v
            // todo: load the gatling pea zombie preview image
            var backupDancerAssetReferenceGameObject = new AssetReferenceGameObject("86af4c6f62e682647853b46a5fb99a9e");
            def.m_preview = backupDancerAssetReferenceGameObject;

            var backupDancerPreviewSprite = new AssetReferenceSprite("863154a01c416424e8a2d97037894d79");
            backupDancerPreviewSprite.m_SubObjectName = "SPR_Preview_Zombie_backup";
            def.m_previewSprite = backupDancerPreviewSprite;

            def.m_previewSpriteScale = 0.8f;
            def.m_previewSpriteOffset = new Vector2(80, -66);

            def.m_plantName = "raw:Gatling Pea Zombie";
            def.m_plantToolTip = "raw:fallback";
        }
    }
    internal static class TallnutZombieSeed
    {
        internal static SeedType Id = (SeedType)1005;
        internal static PlantDefinition def;

        internal static void Init(PlantDefinition template)
        {
            def = UnityEngine.Object.Instantiate(template);
            def.hideFlags = HideFlags.DontUnloadUnusedAsset;
            def.m_seedType = Id;
            def.m_packetIndex = (int)Id;

            def.m_versusCost = 175;
            def.m_versusBaseRefreshTime = 3000;
            def.m_versusSuddenDeathRefreshTime = 1500;


            var backupDancerSprite = new AssetReferenceSprite("b2e616b9bbbacac4980ba6de852d1a44"); // keep this
            backupDancerSprite.m_SubObjectName = "SPR_SeedPackets_Zombies_Versus_30";
            //backupDancerSprite.m_SubObjectType = "UnityEngine.Sprite, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
            def.m_versusImage = backupDancerSprite;


            // temporarily use the backup dancer preview image because tall nut zombie doesnt have one :v
            // todo: load the tall nut zombie preview image
            var backupDancerAssetReferenceGameObject = new AssetReferenceGameObject("86af4c6f62e682647853b46a5fb99a9e");
            def.m_preview = backupDancerAssetReferenceGameObject;

            var backupDancerPreviewSprite = new AssetReferenceSprite("863154a01c416424e8a2d97037894d79");
            backupDancerPreviewSprite.m_SubObjectName = "SPR_Preview_Zombie_backup";
            def.m_previewSprite = backupDancerPreviewSprite;

            def.m_previewSpriteScale = 0.8f;
            def.m_previewSpriteOffset = new Vector2(80, -66);


            def.m_plantName = "raw:Tall-nut Zombie";
            def.m_plantToolTip = "raw:fallback";
        }
    }
    internal static class JalapenoZombieSeed
    {
        internal static SeedType Id = (SeedType)1006;
        internal static PlantDefinition def;

        internal static void Init(PlantDefinition template)
        {
            def = UnityEngine.Object.Instantiate(template);
            def.hideFlags = HideFlags.DontUnloadUnusedAsset;
            def.m_seedType = Id;
            def.m_packetIndex = (int)Id;

            def.m_versusCost = 150;
            def.m_versusBaseRefreshTime = 5000;
            def.m_versusSuddenDeathRefreshTime = 2000;


            var backupDancerSprite = new AssetReferenceSprite("b2e616b9bbbacac4980ba6de852d1a44"); // keep this
            backupDancerSprite.m_SubObjectName = "SPR_SeedPackets_Zombies_Versus_34";
            //backupDancerSprite.m_SubObjectType = "UnityEngine.Sprite, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
            def.m_versusImage = backupDancerSprite;


            // temporarily use the backup dancer preview image because jalapeno zombie doesnt have one :v
            // todo: load the tall nut zombie preview image
            var backupDancerAssetReferenceGameObject = new AssetReferenceGameObject("86af4c6f62e682647853b46a5fb99a9e");
            def.m_preview = backupDancerAssetReferenceGameObject;

            var backupDancerPreviewSprite = new AssetReferenceSprite("863154a01c416424e8a2d97037894d79");
            backupDancerPreviewSprite.m_SubObjectName = "SPR_Preview_Zombie_backup";
            def.m_previewSprite = backupDancerPreviewSprite;

            def.m_previewSpriteScale = 0.8f;
            def.m_previewSpriteOffset = new Vector2(80, -66);

            def.m_plantName = "raw:Jalapeno Zombie";
            def.m_plantToolTip = "raw:fallback";
        }
    }
    internal static class ZombossSeed
    {
        internal static SeedType Id = (SeedType)1007;
        internal static PlantDefinition def;

        internal static void Init(PlantDefinition template)
        {
            def = UnityEngine.Object.Instantiate(template);
            def.hideFlags = HideFlags.DontUnloadUnusedAsset;
            def.m_seedType = Id;
            def.m_packetIndex = (int)Id;

            def.m_versusCost = 1000;
            def.m_versusBaseRefreshTime = 18000;
            def.m_versusSuddenDeathRefreshTime = 18000;

            var backupDancerSprite = new AssetReferenceSprite("b2e616b9bbbacac4980ba6de852d1a44"); // keep this
            backupDancerSprite.m_SubObjectName = "SPR_SeedPackets_Zombies_Versus_1";
            //backupDancerSprite.m_SubObjectType = "UnityEngine.Sprite, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
            def.m_versusImage = backupDancerSprite;


            // temporarily use the backup dancer preview image because peahead doesnt have one :v
            // todo: load the peashooter zombie preview image
            var backupDancerAssetReferenceGameObject = new AssetReferenceGameObject("86af4c6f62e682647853b46a5fb99a9e");
            def.m_preview = backupDancerAssetReferenceGameObject;

            var backupDancerPreviewSprite = new AssetReferenceSprite("863154a01c416424e8a2d97037894d79");
            backupDancerPreviewSprite.m_SubObjectName = "SPR_Preview_Zombie_backup";
            def.m_previewSprite = backupDancerPreviewSprite;

            def.m_previewSpriteScale = 0.8f;
            def.m_previewSpriteOffset = new Vector2(80, -66);

            def.m_plantName = "raw:Dr. Zomboss";
            def.m_plantToolTip = "raw:fallback";
        }
    }




    // ...
    // Mutations of CoreLib [Waiting for permission]
    

    [HarmonyPatch(typeof(DataService), nameof(DataService.GetPlantDefinition))]
    internal static class GetDefinitionforZomboss_Patch_Prefix
    {
        private static bool Prefix(SeedType seedType, ref PlantDefinition __result)
        {
            if (seedType == ZombossSeed.Id && ZombossSeed.def != null) {__result = ZombossSeed.def; return false; }
            if (seedType == BackupDancerSeed.Id && BackupDancerSeed.def != null) {__result = BackupDancerSeed.def; return false; }
            if (seedType == BobsledSeed.Id && BobsledSeed.def != null) { __result = BobsledSeed.def; return false; }
            if (seedType == GigaGargantuarSeed.Id && GigaGargantuarSeed.def != null) { __result = GigaGargantuarSeed.def; return false; }
            if (seedType == PeashooterZombieSeed.Id && PeashooterZombieSeed.def != null) { __result = PeashooterZombieSeed.def; return false; }
            if (seedType == GatlingPeaZombieSeed.Id && GatlingPeaZombieSeed.def != null) { __result = GatlingPeaZombieSeed.def; return false; }
            if (seedType == TallnutZombieSeed.Id && TallnutZombieSeed.def != null) { __result = TallnutZombieSeed.def; return false; }
            if (seedType == JalapenoZombieSeed.Id && JalapenoZombieSeed.def != null) { __result = JalapenoZombieSeed.def; return false; }
            return true;
        }
    }

    // Somehow prevents crashing if playing if Vs Chaos
    [HarmonyPatch(typeof(UserService), nameof(UserService.HasSeedType))]
    internal static class HasSeedType_Zomboss_Patch
    {
        private static Exception Finalizer(SeedType theSeedType, ref bool __result, Exception __exception)
        {
            if (__exception != null && (theSeedType == ZombossSeed.Id || 
            theSeedType == BackupDancerSeed.Id ||
            theSeedType == BobsledSeed.Id ||
            theSeedType == GigaGargantuarSeed.Id ||
            theSeedType == PeashooterZombieSeed.Id ||
            theSeedType == GatlingPeaZombieSeed.Id ||
            theSeedType == TallnutZombieSeed.Id ||
            theSeedType == JalapenoZombieSeed.Id))
            {
                __result = true;
                return null;   
            }
            return __exception;
        }
    }






    [HarmonyPatch(typeof(Board), nameof(Board.AddZombieInRow))]
    internal static class AddZombieInRow_SwapZombies_Patch
    {
        [HarmonyPrefix]
        private static bool Prefix0(Board __instance, ref ZombieType theZombieType, ref int theRow)
        {
            if (!__instance.mApp.IsVersusMode()) return true;

            SeedType zombieCursor = CursorManager.GetSeedTypeInCursor(1);
            SeedType plantCursor = CursorManager.GetSeedTypeInCursor(0);

            // Swap ZombiquariumSnorkel with Backup Dancer
            if (zombieCursor == BackupDancerSeed.Id
            || plantCursor == BackupDancerSeed.Id)
            {
                theZombieType = ZombieType.BackupDancer;
            }
            // Swap ZombiquariumTrophy with Peashooter Zombie
            if (zombieCursor == BobsledSeed.Id
            || plantCursor == BobsledSeed.Id)
            {
                theZombieType = ZombieType.Bobsled;
            }
            if (zombieCursor == GigaGargantuarSeed.Id
            || plantCursor == GigaGargantuarSeed.Id)
            {
                theZombieType = ZombieType.RedeyeGargantuar;
            }
            if (zombieCursor == GatlingPeaZombieSeed.Id
            || plantCursor == GatlingPeaZombieSeed.Id)
            {
                theZombieType = ZombieType.GatlingHead;
            }
            if (zombieCursor == TallnutZombieSeed.Id
            || plantCursor == TallnutZombieSeed.Id)
            {
                theZombieType = ZombieType.TallnutHead;
            }
            if (zombieCursor == JalapenoZombieSeed.Id
            || plantCursor == JalapenoZombieSeed.Id)
            {
                theZombieType = ZombieType.JalapenoHead;
            }
            if (zombieCursor == PeashooterZombieSeed.Id
            || plantCursor == PeashooterZombieSeed.Id)
            {
                theZombieType = ZombieType.PeaHead;
            }

            if (zombieCursor == ZombossSeed.Id
            || plantCursor == ZombossSeed.Id)
            {
                theZombieType = ZombieType.Boss;
                theRow = 2;
            }

            return true;
        }
    }



    // Set these seedtypes to zombie
    [HarmonyPatch(typeof(Challenge), nameof(Challenge.IsZombieSeedType))]
    public static class IsZombieSeedType_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(SeedType theSeedType, ref bool __result)
        {
            if (theSeedType == BackupDancerSeed.Id ||
            theSeedType == PeashooterZombieSeed.Id ||
            theSeedType == GatlingPeaZombieSeed.Id ||
            theSeedType == TallnutZombieSeed.Id ||
            theSeedType == JalapenoZombieSeed.Id ||
            theSeedType == GigaGargantuarSeed.Id ||
            theSeedType == BobsledSeed.Id ||

            theSeedType == ZombossSeed.Id)
            {
                __result = true;
            }
        }
    }

    // Set these seedtypes to zombie: Fixes preview image of custom zombies, if not included in here then auto falls back to basic zombie
    /*
    [HarmonyPatch(typeof(Challenge), nameof(Challenge.IZombieSeedTypeToZombieType))]
    public static class IZombieSeedTypeToZombieType_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(SeedType theSeedType, ref ZombieType __result)
        {
            if (theSeedType == BackupDancerSeed.Id) __result = ZombieType.BackupDancer;
            else if (theSeedType == BobsledSeed.Id) __result = ZombieType.Bobsled;
            //else if (theSeedType == GigaGargantuarSeed.Id) __result = ZombieType.RedeyeGargantuar;
            //else if (theSeedType == ZombossSeed.Id) __result = ZombieType.Boss;
        }
    }
    */



    // Makes the custom zombies selectable
    // gitAngelHub123
    [HarmonyPatch(typeof(ChosenSeed), nameof(ChosenSeed.Selected))]
    internal static class ChosenSeed_Selected_PatchSnorkel
    {
        private static bool Prefix(ChosenSeed __instance, SeedChooserScreen seedChooserScreen, int playerIndex)
        {
            if (__instance.mSeedType != BackupDancerSeed.Id &&
            __instance.mSeedType != PeashooterZombieSeed.Id &&
            __instance.mSeedType != GatlingPeaZombieSeed.Id &&
            __instance.mSeedType != TallnutZombieSeed.Id &&
            __instance.mSeedType != JalapenoZombieSeed.Id &&
            __instance.mSeedType != GigaGargantuarSeed.Id &&
            __instance.mSeedType != BobsledSeed.Id &&
            __instance.mSeedType != ZombossSeed.Id) return true;
            seedChooserScreen.ClickedSeedInChooser(__instance, playerIndex);
            return false;
        }
    }


    // Brute force custom zombie to be placed on zombie side
    // gitAngelHub123
    [HarmonyPatch(typeof(Board), nameof(Board.CanPlantAt))]
    internal static class ZombieNotPlantingonGraves
    {
        private static void Postfix(Board __instance, int theGridX, int theGridY, SeedType theType, ref PlantingReason __result)
        {

            if (__instance == null) return;
            if (!__instance.mApp.IsVersusMode()) return;
            // Too lazy to find out why it's on plant side so brute force custom zombie to be placed on zombie side
            if (theType == BackupDancerSeed.Id ||
            theType == PeashooterZombieSeed.Id ||
            theType == GatlingPeaZombieSeed.Id ||
            theType == TallnutZombieSeed.Id ||
            theType == GigaGargantuarSeed.Id ||
            theType == JalapenoZombieSeed.Id ||
            theType == BobsledSeed.Id ||
            theType == ZombossSeed.Id)
            {
                if (theGridX >= 6 && theGridX <= 8)
                {
                    __result = PlantingReason.Ok;
                }
                else
                {
                    __result = PlantingReason.VSNotHere;
                }
            }

            return;
        }
    }
    [HarmonyPatch(typeof(Challenge), nameof(Challenge.CanPlantAt))]
    internal static class CustomZombieCanPlantAt_Patch
    {
        private static void Postfix(Challenge __instance, int theGridX, int theGridY, SeedType theType, ref PlantingReason __result)
        {

            if (BoardManager.Board == null || BoardManager.App == null) return;
            if (!__instance.mApp.IsVersusMode()) return;

            if (theType == BackupDancerSeed.Id ||
            theType == PeashooterZombieSeed.Id ||
            theType == GatlingPeaZombieSeed.Id ||
            theType == TallnutZombieSeed.Id ||
            theType == GigaGargantuarSeed.Id ||
            theType == JalapenoZombieSeed.Id ||
            theType == BobsledSeed.Id ||
            theType == ZombossSeed.Id)
            {
                if (theGridX >= 6 && theGridX <= 8)
                {
                    __result = PlantingReason.Ok;
                }
                else
                {
                    __result = PlantingReason.VSNotHere;
                }
            }

            return;
        }
    }




    // Bypass $ for tooltips
    // D1GQ
    [HarmonyPatch(typeof(FormattedLocalizationIdBinder), nameof(FormattedLocalizationIdBinder.BindValue))]
    internal static class LocalizationBinderPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(FormattedLocalizationIdBinder __instance, string value)
        {
            if (string.IsNullOrEmpty(value)) return true;

            if (value.StartsWith("raw:"))
            {
                if (__instance != null && __instance.m_text != null)
                {
                    __instance.m_text.SetText(value[4..]);
                }
                return false; 
            }
            return true; 
        }
    }


    public static bool isCustomZombieSeedType(SeedType theType){
        return theType == BackupDancerSeed.Id ||
            theType == PeashooterZombieSeed.Id ||
            theType == GatlingPeaZombieSeed.Id ||
            theType == TallnutZombieSeed.Id ||
            theType == GigaGargantuarSeed.Id ||
            theType == JalapenoZombieSeed.Id ||
            theType == BobsledSeed.Id ||
            theType == ZombossSeed.Id;
    }
    public static bool isCustomZombieSeedType_Packet(SeedPacket theType)
    {
        return theType.mPacketType == BackupDancerSeed.Id ||
            theType.mPacketType == PeashooterZombieSeed.Id ||
            theType.mPacketType == GatlingPeaZombieSeed.Id ||
            theType.mPacketType == TallnutZombieSeed.Id ||
            theType.mPacketType == GigaGargantuarSeed.Id ||
            theType.mPacketType == JalapenoZombieSeed.Id ||
            theType.mPacketType == BobsledSeed.Id ||
            theType.mPacketType == ZombossSeed.Id;
    }

}
