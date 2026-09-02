namespace VersusPlus;
#pragma warning disable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BloomEngine.Config;
using BloomEngine.Config.Inputs;
using BloomEngine.Helpers;
using BloomEngine.ModMenu;

using HarmonyLib;

using Il2CppGoogle.Protobuf.WellKnownTypes;

using Il2CppInterop.Runtime.Injection;

using Il2CppMono.Security.Authenticode;

using Il2CppReloaded;
using Il2CppReloaded.Data;
using Il2CppReloaded.DataModels;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using Il2CppReloaded.TreeStateActivities;

using Il2CppSentry;
using Il2CppSentry.Protocol;

using Il2CppSource.Controllers;
using Il2CppSource.DataModels;

using Il2CppSystem;
using Il2CppSystem.Numerics;
using Il2CppSystem.Reflection;

using Il2CppTekly.Localizations;

using Il2CppTMPro;

using MelonLoader;

using ReplantAPI;
using ReplantAPI.Managers;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler;

using static Il2CppSystem.Net.Http.Headers.Parser;

using static UnityEngine.ParticleSystem.PlaybackState;

// Todo:
// - Add Balloon, Imp, Blover (done)
// - Fix Balloon logic (done)
// - Port over the patches (done)

// - Detect the controllers image

// - Add Seed Tooltip (done) and toggle for it (done)
// - Add Recharge Timer (done) and toggle for it (done)
// - Add Instant Death (done) and toggle for it (done)
// - Add Timer (done) and toggle for it (done)

// - Fix Digger spawns at the back (done)
// - Enhance Random Mode
// - Fix initial recharge (done)
// - Fix timer layer and add font (done)
// - Fix Imp logic (done)

// - (maybe) Add custom zombie seed packet for the Zombotanies (Peashooter, Wall-nut, Tall-nut, Jalapeno, Squash, Gatling Pea)

// - Add Chaos Mode: all units are instantly recharged, 0 cost, but instas will have their recharge time increased for each time using

// - Known issues:
// + System.NullReferenceException for Timer at the end
// + 
// - Hidden issues:
// + idk, it's hidden lol

public class Core : ReplantAPI
{
    public bool hasPlayedSuddenDeath = false;
    public bool hasPlayedEnd = false;
    public bool hasPlayedTen = false;
    public static bool TenMinute = false;
    public static bool StartMatch = false;
    public static bool hasChaos = false;

    public static bool hasDisplayedChaosMessage = false;

    public bool InitializeMatch = false;

    public const string Name = "VersusPlus";
    public const string Version = "0.1-beta";
    public const string Author = "Cougtt";

    SeedPacket temp;


    internal static class BloomConfig 
    {
        public static BoolConfigInput TestBoolInput { get; } = ConfigService.CreateBool(
            name: "Seed Recharge Tooltip",
            defaultValue: true,
            description: "Shows recharge of units in the tooltip box."
        );
        public static BoolConfigInput TestBoolInput2 { get; } = ConfigService.CreateBool(
            name: "Seed Recharge Timer",
            defaultValue: true,
            description: "Shows live recharge time of units."
        );
        public static BoolConfigInput TestBoolInput3 { get; } = ConfigService.CreateBool(
            name: "Instant Death",
            defaultValue: true,
            description: "Enables Instant Death Phase - A row of Peashooter is spawned at the back (if grid is empty) and all remaining lawnmowers are destroyed."
        );
        public static BoolConfigInput TestBoolInput4 { get; } = ConfigService.CreateBool(
            name: "Timer",
            defaultValue: true,
            description: "Shows the timer."
        );
        public static BoolConfigInput TestBoolInput5 { get; } = ConfigService.CreateBool(
            name: "Initial Recharge",
            defaultValue: true,
            description: "Sets initial recharge for almost all units."
        );
        public static BoolConfigInput TestBoolInput6 { get; } = ConfigService.CreateBool(
            name: "New Brain Rate Logic",
            defaultValue: true,
            description: "Sets Gravestone's brain rate to around 27 - 30 seconds and initial brain production. Disable this if you have custom brain rate logic (or just wants zombies to win)."
        );
        public static BoolConfigInput TestBoolInput7 { get; } = ConfigService.CreateBool(
            name: "No More Than 35",
            defaultValue: true,
            description: "Targets and Gravestones take no more than 35 damage per hit. Disable this if you are not afraid of Melon-pult."
        );
        public static BoolConfigInput TestBoolInput8 { get; } = ConfigService.CreateBool(
            name: "???",
            defaultValue: true,
            description: "This option is created solely for credits only. Do not enable, or else...\n" +
            "\n" +
            "Seed Recharge Tooltip - by RaptorRush, inspired by PvZ DLC Mod\n" +
            "Instant Death, Seed Recharge Timer - inspired by PvZ DLC Mod\n" +
            "Initial Recharge - partially helped by D1GQ\n" +
            "New Brain Rate Logic - by D1GQ\n" +
            "Most of the patches - by D1GQ and RaptorRush\n" +
            "\n" +
            "Thank you very much :) You 2 are the goat. I wouldn't have tried my best if it hadn't been for these two.\n" +
            "Also thank you HenHen for ReplantAPI, " +
            "and thank you PalmForest for BloomEngine."
        );
    }
    // Custom DieRow
    public static void DieRow(int row)
    {
        LawnMower inRow = LawnMowerManager.GetInRow(row);
        if (inRow == null)
        {
            MelonLogger.Warning($"[ReplantAPI] LawnMowerManager.TriggerRow: no mower found in row {row}.");
        }
        else
        {
            if (row == 0) EffectsManager.Spawn(10, 160, ParticleEffect.JackExplode);
            if (row == 1) EffectsManager.Spawn(10, 260, ParticleEffect.JackExplode);
            if (row == 2) EffectsManager.Spawn(10, 360, ParticleEffect.JackExplode);
            if (row == 3) EffectsManager.Spawn(10, 460, ParticleEffect.JackExplode);
            if (row == 4) EffectsManager.Spawn(10, 560, ParticleEffect.JackExplode);
            inRow.Die();
            var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
            if (activity != null)
            {
                activity.m_audioService.PlaySample(Constants.Sound.SOUND_EXPLOSION, false);

            }
        }
    }
    public override void OnInitializeMelon()
    {
        this.LoggerInstance.Msg("Loading core...");
        GameplayActivityApi.ActiveStarted += GameplayStarted;

        ClassInjector.RegisterTypeInIl2Cpp<Timer>();
        ClassInjector.RegisterTypeInIl2Cpp<Timer2>();

        ModMenuService.CreateEntry(this)
            .AddDisplayName(Name)
            .AddDescription($"Gives a better experience in Versus Mode!")
            .AddConfigClass(typeof(BloomConfig))
            .Register();
    }
   
    

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!BoardManager.IsLoaded) return;

        if (BoardManager.Board.mApp != null && BoardManager.Board.mApp.VersusMode != null
            && BoardManager.Board.mApp.VersusMode.m_gameplayInitialized)
        {
            
            if (!InitializeMatch){
                InitializeMatch = true;
                if (!BloomConfig.TestBoolInput5.Value) return;

                for (int index = 0; index < 2; index++){
                    for (int i = 0; i < 6; i++)
                    {
                        temp = SeedBankManager.GetPacketAt(i, index);
                        SeedType seedType = temp.PacketType;
                        PlantDefinition def = BoardManager.App.GetPlantDefinition(seedType);

                        if (def.m_seedType == SeedType.Peashooter ||
                        def.m_seedType == SeedType.Sunflower ||
                        def.m_seedType == SeedType.Wallnut ||
                        def.m_seedType == SeedType.Potatomine ||
                        def.m_seedType == SeedType.Puffshroom ||
                        def.m_seedType == SeedType.Sunshroom ||
                        def.m_seedType == SeedType.Puffshroom ||
                        def.m_seedType == SeedType.Cabbagepult ||
                        def.m_seedType == SeedType.Garlic ||
                        def.m_seedType == SeedType.ZombieGravestone ||
                        def.m_seedType == SeedType.ZombieNormal ||
                        def.m_seedType == SeedType.ZombieTrashCan)
                        {
                            continue;
                        }
                        else
                        {
                            temp.mActive = true;
                            temp.mActive = false;
                            temp.mRefreshTime = def.m_versusBaseRefreshTime;
                            temp.mRefreshing = true;


                        }


                    }
                }             
            }
        }

        



        // Plays huge wave sound when it's Sudden Death
        if (BoardManager.App != null && BoardManager.App.VersusMode != null)
        {
            if (BoardManager.App.VersusMode.Phase == VersusPhase.SuddenDeath)
            {
                if (!hasPlayedSuddenDeath)
                {
                    hasPlayedSuddenDeath = true;

                    var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
                    if (activity != null)
                    {
                        activity.m_audioService.PlaySample(Constants.Sound.SOUND_HUGE_WAVE, false);

                    }
                }
            }
        }
        
        // Plays death sounds when game ends
        if (BoardManager.App != null && BoardManager.App.VersusMode != null)
        {
            if (BoardManager.App.VersusMode.m_winInitialized)
            {
                if (!hasPlayedEnd)
                {
                    hasPlayedEnd = true;

                    var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
                    if (activity != null)
                    {
                        activity.m_audioService.PlayMusic(MusicTune.None, 0, 0);
                        activity.m_audioService.PlaySample(Constants.Sound.SOUND_RIP_LOSE, false);

                    }
                }
            }
        }
        


        // Instant Death phase
        if (TenMinute && BloomConfig.TestBoolInput3.Value)
        {         
            if (!hasPlayedTen)
            {
                hasPlayedTen = true;
                // Play explosion sound
                var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
                if (activity != null)
                {
                    activity.m_audioService.PlaySample(Constants.Sound.SOUND_FINALWAVE, false);
                }
                BoardManager.ShakeBoard(5, 5);

                // Spawn Peashooters at the back
                for (int i = 0; i < 5; i++)
                {
                    if (PlantManager.CanPlantAt(0, i, SeedType.Peashooter) == PlantingReason.Ok)
                    {
                        PlantManager.Spawn(0, i, SeedType.Peashooter);
                    }
                }
                LoggerInstance.Msg("Lawnmowers left: " + LawnMowerManager.CountUntriggered());

                // Destroy all lawnmowers
                DieRow(0);
                DieRow(1);
                DieRow(2);
                DieRow(3);
                DieRow(4);

                // Show INSTANT DEATH huge wave message
                AdviceManager.ShowAdvice("INSTANT DEATH!", MessageStyle.HugeWave, AdviceType.None);
                AdviceManager.SetDuration(500);
            }
            
        }



        // Chaos Phase
        
        if (StartMatch && BloomConfig.TestBoolInput8.Value)
        {
            if (!hasChaos){
                hasChaos = true;

                //all units are instantly recharged, 0 cost, but instas will have their recharge time increased for each time using

                // Destroy 2 random lawnmowers
                int random = UnityEngine.Random.Range(0, 5);
                int random2 = UnityEngine.Random.Range(0, 5);
                while (random == random2){
                    random2 = UnityEngine.Random.Range(0, 5);
                }
                DieRow(random);
                DieRow(random2);

                // Set units to 9990 cost
                BoardManager.AddSun(9990, 0);
                BoardManager.AddSun(9990, 1);

              
                AdviceManager.ShowAdvice("CHAOTIC DEATH!", MessageStyle.HugeWave, AdviceType.None);
                AdviceManager.SetDuration(500);
                BoardManager.ShakeBoard(10, 10);

                var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
                if (activity != null)
                {
                    activity.m_audioService.PlaySample(Constants.Sound.SOUND_EXPLOSION, false);
                    activity.m_audioService.PlaySample(Constants.Sound.SOUND_SCREAM, false); //crezy dev scream
                }
                if (!hasDisplayedChaosMessage){
                    hasDisplayedChaosMessage = true;
                    MelonLoader.MelonCoroutines.Start(WaitForChaosMessage());
                }
                MelonLoader.MelonCoroutines.Start(SpawnDoomShroom());
                // Spawms 2 random Doom-shroom on each column for plant side (not counting col 0), 1 for zombie side(not counting col 8)



                // Every 15 seconds
                // Spawn a random plant

                // Spawn a random zombie


            }
        }

    }
        
    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        base.OnSceneWasInitialized(buildIndex, sceneName);
        hasPlayedSuddenDeath = false;
        hasPlayedEnd = false;
        hasPlayedTen = false;
        TenMinute = false;
        StartMatch = false;
        hasChaos = false;

        InitializeMatch = false;

    }
   
    private static void GameplayStarted(GameplayActivity gameplayActivity)
    {
        foreach (var (@override, definition) in SeedPacketToolTipApi.ToolTipsOverrides
            .Where(t => t.Definition.SeedType != SeedType.Imitater))
        {
            int refreshTime = gameplayActivity.IsVersusMode()
                ? definition.m_versusBaseRefreshTime
                : definition.m_refreshTime;
            if (!BloomConfig.TestBoolInput.Value) break;
            if (gameplayActivity.IsVersusMode())
            {
                if (definition.m_seedType == SeedType.ZombieImp)
                {
                    @override.Set(
                    $"<color=#00ff00>Recharge: {refreshTime / 100.0} seconds<color=#000000>\nImps are tiny zombies that can sprint to your house.");
                }
                else
                {
                    @override.Set(
                        $"<color=#00ff00>Recharge: {refreshTime / 100.0} seconds<color=#000000>\n" +
                        $"{@override.OriginalFormat}");
                }
            } else {
                @override.Set(
                            $"<color=#00ff00>Recharge: {refreshTime / 100.0} seconds<color=#000000>\n" +
                            $"{@override.OriginalFormat}");
            }

        }
    }
    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
	{
		base.OnSceneWasLoaded(buildIndex, sceneName);
        //LoggerInstance.Msg("Scene Loaded: " + sceneName);
        /*
        var allDefs = UnityEngine.Object.FindObjectsOfTypeAll(Il2CppInterop.Runtime.Il2CppType.Of<PlantDefinition>());
		foreach (var obj in allDefs)
        {
			var def = obj.Cast<PlantDefinition>();
            
            //                                                                                                                 Upgraded plants
            if (def.m_seedType == SeedType.Gatlingpea)
            {
                //def.m_versusBaseRefreshTime = 6000;
                //def.m_versusSuddenDeathRefreshTime = 6000;
                //def.m_versusCost = 100;
            }
            if (def.m_seedType == SeedType.Twinsunflower)
            {
                //def.m_versusBaseRefreshTime = 6000;
                //def.m_versusSuddenDeathRefreshTime = 6000;
                //def.m_versusCost = 50;
            }
            if (def.m_seedType == SeedType.Wintermelon)
            {
                //def.m_versusBaseRefreshTime = 6000;
                //def.m_versusSuddenDeathRefreshTime = 6000;
                //def.m_versusCost = 100;
            }

            //                                                                                                                 Zombies

            if (def.m_seedType == SeedType.ZombieBalloon)
            {
                //def.m_plantToolTip = "This zombie is now forced to act as a weak shield to your almighty defense";
            }
            if (def.m_seedType == SeedType.ZombieImp)
            {
                //def.m_plantToolTip = "Imps are just as strong as a normal zombie";


            }

            

        }
        
        var allDefsZ = UnityEngine.Object.FindObjectsOfTypeAll(Il2CppInterop.Runtime.Il2CppType.Of<ZombieDefinition>());
        foreach (var obj in allDefsZ){
            var def = obj.Cast<ZombieDefinition>();
            if (def.m_zombieType == ZombieType.Imp){

            }
        }
        */


    }

    

    // Patch: Allow everything in Versus Mode
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
            theSeedType == SeedType.Blover
            ))
            {
                __result = false;
                return false;
            }
            return true;
        }
        
    }








    [HarmonyPatch(typeof(SeedChooserDataModel), nameof(SeedChooserDataModel.UpdateEntries))]
    internal static class SeedChooserDataModel_UpdateEntries_Patch
    {
        [HarmonyPrefix]
        private static bool Prefix(SeedChooserDataModel __instance)
        {
            if (__instance.m_seedChooserScreen.mApp.IsVersusMode())
            {
                //Clear data models
                __instance.m_entriesModel.Clear();


                for (int i = 0; i < __instance.m_seedChooserScreen.mChosenSeeds.Count; i++)
                {
                    var plantChosenSeed = __instance.m_seedChooserScreen.mChosenSeeds[i];
                    if (plantChosenSeed.mSeedType == SeedType.None) continue;
                    
                    PlantDefinition plantDefinition = __instance.m_seedChooserScreen.mApp.GetPlantDefinition(plantChosenSeed.mSeedType);

                    if (plantDefinition == null || plantDefinition.m_versusBaseRefreshTime == 0) continue;
               
                    SeedChooserEntryModel plantEntry = new SeedChooserEntryModel(plantDefinition, plantChosenSeed, __instance.m_seedChooserScreen, __instance, false, i);
                    __instance.m_entriesModel.Add(i.ToString(), plantEntry);
                }


                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(SeedChooserDataModel), nameof(SeedChooserDataModel.UpdateZombieEntries))]
    internal static class SeedChooserDataModel_UpdateEntries_PatchZombie
    {
        [HarmonyPrefix]
        private static bool Prefix(SeedChooserDataModel __instance)
        {
            if (__instance.m_seedChooserScreen.mApp.IsVersusMode())
            {
                //Clear data models

                __instance.m_zombieEntriesModel.Clear();
     
                for (int i = 0; i < __instance.m_seedChooserScreen.mChosenZombies.Count; i++)
                {
                    var zombieChosenSeed = __instance.m_seedChooserScreen.mChosenZombies[i];
                    if (zombieChosenSeed.mSeedType == SeedType.None) continue;

                    PlantDefinition zombieDefinition = __instance.m_seedChooserScreen.mApp.GetPlantDefinition(zombieChosenSeed.mSeedType);
                    if (zombieDefinition == null || zombieDefinition.m_versusBaseRefreshTime == 0) continue;

                    SeedChooserEntryModel zombieEntry = new SeedChooserEntryModel(zombieDefinition, zombieChosenSeed, __instance.m_seedChooserScreen, __instance, false, i);
                    __instance.m_zombieEntriesModel.Add(i.ToString(), zombieEntry);
                }
                
                return false;
            }

            return true;
        }

    }
    [HarmonyPatch(typeof(Zombie), nameof(Zombie.StopEating))]
    internal static class ImpSpeedFix
    {
        [HarmonyPostfix]
        private static void Prefix0(Zombie __instance)
        {

            if (__instance.mZombieType != ZombieType.Imp) return;
            if (!__instance.mApp.IsVersusMode()) return;


            //__instance.mZombiePhase = ZombiePhase.ZombieNormal;
            __instance.mVelX = BalloonSkipRiseFromGrave.speed;
            __instance.UpdateAnimSpeed();
        }
    }

    [HarmonyPatch(typeof(Zombie), nameof(Zombie.RiseFromGrave))]
    internal static class BalloonSkipRiseFromGrave
    {
        public static float speed;
        [HarmonyPrefix]
        private static bool Prefix0(Zombie __instance)
        {
            // Skip rise from grave entirely for balloon
            if (__instance.mZombieType != ZombieType.Imp) return true;
            if (!__instance.mApp.IsVersusMode()) return true;


            //__instance.mZombiePhase = ZombiePhase.ZombieNormal;
            speed = __instance.mVelX * 2f;
            __instance.mVelX  = speed;
            __instance.UpdateAnimSpeed();

            return true; // skip original method

        }

        [HarmonyPrefix]
        private static bool Prefix(Zombie __instance)
        {
            // Skip rise from grave entirely for balloon
            if (__instance.mZombieType != ZombieType.Balloon) return true;
            if (!__instance.mApp.IsVersusMode()) return true;

            __instance.mPosX = 800f; // Push it off-screen immediately
            __instance.mZombiePhase = ZombiePhase.BalloonFlying;


            return false; // skip original method
            
        }
        [HarmonyPrefix]
        private static bool Prefix2(Zombie __instance)
        {
            // Skip rise from grave entirely for balloon
            if (__instance.mZombieType != ZombieType.Digger) return true;
            if (!__instance.mApp.IsVersusMode()) return true;


            __instance.mPosX = 800f; // Push it off-screen immediately
            __instance.mZombiePhase = ZombiePhase.DiggerTunneling;


            return false; // skip original method



        }
        [HarmonyPrefix]
        private static bool Prefix3(Zombie __instance)
        {
            // Skip rise from grave entirely for the Zombotanies
            if (__instance.mZombieType != ZombieType.PeaHead) return true;
            if (__instance.mZombieType != ZombieType.WallnutHead) return true;
            if (__instance.mZombieType != ZombieType.SquashHead) return true;
            if (__instance.mZombieType != ZombieType.TallnutHead) return true;
            if (__instance.mZombieType != ZombieType.GatlingHead) return true;
            if (__instance.mZombieType != ZombieType.JalapenoHead) return true;
            if (!__instance.mApp.IsVersusMode()) return true;


            __instance.mPosX = 800f; // Push it off-screen immediately
            __instance.mZombiePhase = ZombiePhase.ZombieNormal;

            return false; // skip original method


        }
    }



    [HarmonyPatch(typeof(Board), nameof(Board.AddZombieInRow))]
    internal static class SpawnPlant
    {

        /*
            [HarmonyPrefix]
            private static bool Prefix0(Board __instance, ref SeedType theZombieType)
            {
                if (!__instance.mApp.IsVersusMode()) return true;

                SeedType zombieCursor = CursorManager.GetSeedTypeInCursor(1);
                SeedType plantCursor = CursorManager.GetSeedTypeInCursor(0);

                if (((int)zombieCursor >= 60 && (int)zombieCursor <= 80) 
                || ((int)plantCursor >= 60 && (int)plantCursor <= 80))
                {
                    // Force the translation

                    return true;
                }

                return true;
            }

                if (zombieCursor == SeedType.ZombieBalloon || plantCursor == SeedType.ZombieBalloon)
                {
                    // Force the translation
                    theZombieType = ZombieType.Balloon;
                }


                return true;
                */
        internal static class sigma
        {
            [HarmonyPrefix]
            private static bool Prefix(Board __instance, ref SeedType theZombieType)
            {
                if (!__instance.mApp.IsVersusMode()) return true;

                SeedType zombieCursor = CursorManager.GetSeedTypeInCursor(1);
                SeedType plantCursor = CursorManager.GetSeedTypeInCursor(0);
                if (zombieCursor == SeedType.ZombieNormal || plantCursor == SeedType.ZombieNormal)
                {
                    // Force the translation
                    //theZombieType = ZombieType.Balloon;
                    //if (GridItemManager.GetAt(GridItemType.Gravestone, theGridX, theGridY) != null)
                    //{
                    theZombieType = SeedType.ZombieFootball;
                    //}

                }
                return true;
            }
        }




        // Plays huge wave sound when Flag Zombie is spawned
        [HarmonyPrefix]
        private static bool Prefix2(Board __instance, ZombieType theZombieType)
        {
            if (theZombieType == ZombieType.Flag){
                var activity = UnityEngine.Object.FindObjectOfType<GameplayActivity>();
                if (activity != null)
                {
                    activity.m_audioService.PlaySample(Constants.Sound.SOUND_HUGE_WAVE);

                }
            }

            return true;
        }


      
    }

    

    [HarmonyPatch(typeof(Board), nameof(Board.StartLevel))]
    internal static class SpawnTimerPatch
    {
        [HarmonyPostfix]
        private static void Postfix(Board __instance)
        {
            // Only spawn the timer if it's a Versus match
            if (__instance.mApp.IsVersusMode())
            {
                // Create an empty object and attach your script. 
                // The Start() method will automatically handle the text and layers.
                GameObject timerObj = new GameObject("Timer");
                timerObj.AddComponent<Timer>();

                if (BloomConfig.TestBoolInput2.Value){
                    GameObject timerObj2 = new GameObject("LiveRecharge");
                    timerObj2.AddComponent<Timer2>();
                }
                

            }
            
        }
    }



    /*
    [HarmonyPatch(typeof(Board), nameof(Board.AddPlant))]
    internal static class AddPlantPatch
    {
        [HarmonyPostfix]
        private static void Postfix(Board __instance, SeedType theSeedType)
        {
            // Only spawn the timer if it's a Versus match
            if (!__instance.mApp.IsVersusMode()) return;
            if (!BloomConfig.TestBoolInput8.Value) return;
            if (theSeedType != SeedType.Sunflower) return;
            for (int i = 0; i < 6; i++){
                SeedPacket temp = SeedBankManager.GetPacketAt(i, 0);
                if (temp != null && temp.PacketType == SeedType.Sunflower){
                    temp.mRefreshTime += 1000;
                    break;
                }
            }


        }
    }
    */
    private static System.Collections.IEnumerator WaitForChaosMessage()
    {
        // Wait exactly 1.5 seconds
        yield return new WaitForSecondsRealtime(5f);
        AdviceManager.ShowAdvice("So, you have chosen death...", MessageStyle.TutorialLevel1, AdviceType.None);
        AdviceManager.SetDuration(500);
    }





    

    

    private static System.Collections.IEnumerator SpawnDoomShroom()
    {
        int random, random2;
        for (int i = 1; i < 8; i++){

            random = UnityEngine.Random.Range(0, 5);
            random2 = UnityEngine.Random.Range(0, 5);
            while (random == random2)
            {
                random2 = UnityEngine.Random.Range(0, 5);
            }

            if (i <= 5){
                if (i % 2 == 0){
                    PlantManager.Spawn(i, random, SeedType.Doomshroom);
                    PlantManager.Spawn(i, random2, SeedType.Doomshroom);

                    PlantManager.Spawn(i, random, SeedType.InstantCoffee);
                    PlantManager.Spawn(i, random2, SeedType.InstantCoffee);
                } else {
                    //PlantManager.Spawn(i, random, SeedType.Doomshroom);

                    //PlantManager.Spawn(i, random, SeedType.InstantCoffee);

                    BoardManager.Board.mEnableGraveStones = true;
                    BoardManager.Board.AddAGraveStone(i, random);

                }
                   
                    
                
                //BoardManager.Board.AddAGraveStone(i, random2);


            } else {
                BoardManager.Board.mEnableGraveStones = true;
                BoardManager.Board.AddAGraveStone(i, random);
                BoardManager.Board.AddAGraveStone(i, random2);
            }
            yield return new WaitForSecondsRealtime(0.15f);
        }



    }


    [HarmonyPatch(typeof(Board), nameof(Board.CanPlantAt))]
    internal static class ZombieNotPlantingonGraves
    {
        [HarmonyPrefix]
        private static bool Prefix(int theGridX, int theGridY, SeedType theType, ref PlantingReason __result)
        {
            if (theType == SeedType.ZombieGravestone && 
            GridItemManager.GetAt(GridItemType.Gravestone, theGridX, theGridY) != null)
            {
                __result = PlantingReason.NotOnGrave;
                return false;
            }

            if (theType == SeedType.ZombieGravestone &&
            theGridX == 8)
            {
                __result = PlantingReason.NotHere;
                return false;
            }
            return true;
        }
    }
    
    
    
}
