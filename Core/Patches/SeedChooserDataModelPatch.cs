namespace VersusPlus.Patches;
#pragma warning disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Il2CppReloaded.Data;
using Il2CppReloaded.Gameplay;

using Il2CppSource.DataModels;
using BloomEngine.Config;
using BloomEngine.Config.Inputs;
using BloomEngine.Helpers;
using BloomEngine.ModMenu;

using HarmonyLib;

using static VersusPlus.Core.Core;
using static VersusPlus.UI.SeedPacketSwapper;
using static VersusPlus.Gameplay.CustomSeedType;

internal class SeedChooserDataModelPatch
{


    // Reload the Seed chooser screen
    // Units with m_versusBaseRefreshTime = 0 will not be displayed
    // D1GQ

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
                var chosenZombies = __instance.m_seedChooserScreen.mChosenZombies;
                var temp = __instance.m_seedChooserScreen.mChosenZombies[3];
                // Ensure that adding seed initialization only runs once
                // Create a new ChosenSeed, take properties of [3] (from a valid zombie) (prob buckethead)
                // gitAngelHub123

                // Manually adjust tooltips for each custom zombie


                // Backup Dancer\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                if (BackupDancerSeed.def == null)
                    BackupDancerSeed.Init(__instance.m_seedChooserScreen.mApp.GetPlantDefinition(SeedType.ZombiePail));

                // Seed Recharge Tooltip for custom zombies
                if (BackupDancerSeed.def != null)
                {
                    int refreshTime = BackupDancerSeed.def.m_versusBaseRefreshTime;
                    if (BloomConfig.TestBoolInput.Value)
                        BackupDancerSeed.def.m_plantToolTip = $"raw:<color=#00ff00>Recharge: {refreshTime / 100.0} seconds<color=#000000>\nThis zombie appears in set of one whenever its seed packet is used.";

                    else
                        BackupDancerSeed.def.m_plantToolTip = "raw:This zombie appears in set of one whenever its seed packet is used.";
                }

                bool isBackupDancer = false;
                for (int i = 0; i < chosenZombies.Count; i++)
                {
                    if (chosenZombies[i].mSeedType == BackupDancerSeed.Id)
                    {
                        isBackupDancer = true;
                        break;
                    }
                }
                if (!isBackupDancer)
                {
                    var s = new ChosenSeed();
                    s.mSeedState = temp.mSeedState;
                    s.mImitaterType = temp.mImitaterType;
                    s.mSeedType = BackupDancerSeed.Id;

                    chosenZombies.Add(s);
                }
                // Bobsled\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                if (BobsledSeed.def == null)
                    BobsledSeed.Init(__instance.m_seedChooserScreen.mApp.GetPlantDefinition(SeedType.ZombiePail));
                // Seed Recharge Tooltip for custom zombies
                if (BobsledSeed.def != null)
                {
                    int refreshTime = BobsledSeed.def.m_versusBaseRefreshTime;
                    if (BloomConfig.TestBoolInput.Value)
                        BobsledSeed.def.m_plantToolTip = $"raw:<color=#00ff00>Recharge: {refreshTime / 100.0} seconds<color=#000000>\nThese zombies appear in teams of four.";

                    else
                        BobsledSeed.def.m_plantToolTip = "raw:These zombies appear in teams of four.";
                }

                bool isBobsled = false;
                for (int i = 0; i < chosenZombies.Count; i++)
                {
                    if (chosenZombies[i].mSeedType == BobsledSeed.Id)
                    {
                        isBobsled = true;
                        break;
                    }
                }
                if (!isBobsled)
                {
                    var s = new ChosenSeed();
                    s.mSeedState = temp.mSeedState;
                    s.mImitaterType = temp.mImitaterType;
                    s.mSeedType = BobsledSeed.Id;

                    chosenZombies.Add(s);
                }

                // Giga Gargantuar\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                if (GigaGargantuarSeed.def == null)
                    GigaGargantuarSeed.Init(__instance.m_seedChooserScreen.mApp.GetPlantDefinition(SeedType.ZombiePail));
                // Seed Recharge Tooltip for custom zombies
                if (GigaGargantuarSeed.def != null)
                {
                    int refreshTime = GigaGargantuarSeed.def.m_versusBaseRefreshTime;
                    if (BloomConfig.TestBoolInput.Value)
                        GigaGargantuarSeed.def.m_plantToolTip = $"raw:<color=#00ff00>Recharge: {refreshTime / 100.0} seconds<color=#000000>\nGiga-gargantuar is a giga-gigantic zombie.";

                    else
                        GigaGargantuarSeed.def.m_plantToolTip = "raw:Giga-gargantuar is a giga-gigantic zombie.";
                }

                bool isGiga = false;
                for (int i = 0; i < chosenZombies.Count; i++)
                {
                    if (chosenZombies[i].mSeedType == GigaGargantuarSeed.Id)
                    {
                        isGiga = true;
                        break;
                    }
                }
                if (!isGiga)
                {
                    var s = new ChosenSeed();
                    s.mSeedState = temp.mSeedState;
                    s.mImitaterType = temp.mImitaterType;
                    s.mSeedType = GigaGargantuarSeed.Id;

                    chosenZombies.Add(s);
                }

                // Dr Zomboss\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                if (ZombossSeed.def == null)
                    ZombossSeed.Init(__instance.m_seedChooserScreen.mApp.GetPlantDefinition(SeedType.ZombiePail));
                // Seed Recharge Tooltip for custom zombies
                if (ZombossSeed.def != null)
                {
                    int refreshTime = ZombossSeed.def.m_versusBaseRefreshTime;
                    if (BloomConfig.TestBoolInput.Value)
                        ZombossSeed.def.m_plantToolTip = $"raw:<color=#00ff00>Recharge: {refreshTime / 100.0} seconds<color=#000000>\nDr. Zomboss rules them all.";

                    else
                        ZombossSeed.def.m_plantToolTip = "raw:Dr. Zomboss rules them all.";
                }
                bool isZomboss = false;
                for (int i = 0; i < chosenZombies.Count; i++)
                {
                    if (chosenZombies[i].mSeedType == ZombossSeed.Id)
                    {
                        isZomboss = true;
                        break;
                    }
                }
                if (!isZomboss)
                {
                    var s = new ChosenSeed();
                    s.mSeedState = temp.mSeedState;
                    s.mImitaterType = temp.mImitaterType;
                    s.mSeedType = ZombossSeed.Id;

                    chosenZombies.Add(s);
                }

                // Peashooter Zombie\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                if (PeashooterZombieSeed.def == null)
                    PeashooterZombieSeed.Init(__instance.m_seedChooserScreen.mApp.GetPlantDefinition(SeedType.ZombiePail));
                // Seed Recharge Tooltip for custom zombies
                if (PeashooterZombieSeed.def != null)
                {
                    int refreshTime = PeashooterZombieSeed.def.m_versusBaseRefreshTime;
                    if (BloomConfig.TestBoolInput.Value)
                        PeashooterZombieSeed.def.m_plantToolTip = $"raw:<color=#00ff00>Recharge: {refreshTime / 100.0} seconds<color=#000000>\nShoots peas at the enemy.";

                    else
                        PeashooterZombieSeed.def.m_plantToolTip = "raw:Shoots peas at the enemy.";
                }
                bool isPeahead = false;
                for (int i = 0; i < chosenZombies.Count; i++)
                {
                    if (chosenZombies[i].mSeedType == PeashooterZombieSeed.Id)
                    {
                        isPeahead = true;
                        break;
                    }
                }
                if (!isPeahead)
                {
                    var s = new ChosenSeed();
                    s.mSeedState = temp.mSeedState;
                    s.mImitaterType = temp.mImitaterType;
                    s.mSeedType = PeashooterZombieSeed.Id;

                    chosenZombies.Add(s);
                }

                // Gatling Pea Zombie \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                if (GatlingPeaZombieSeed.def == null)
                    GatlingPeaZombieSeed.Init(__instance.m_seedChooserScreen.mApp.GetPlantDefinition(SeedType.ZombiePail));
                // Seed Recharge Tooltip for custom zombies
                if (GatlingPeaZombieSeed.def != null)
                {
                    int refreshTime = GatlingPeaZombieSeed.def.m_versusBaseRefreshTime;
                    if (BloomConfig.TestBoolInput.Value)
                        GatlingPeaZombieSeed.def.m_plantToolTip = $"raw:<color=#00ff00>Recharge: {refreshTime / 100.0} seconds<color=#000000>\nShoots four peas at your plants at a time.";

                    else
                        GatlingPeaZombieSeed.def.m_plantToolTip = "raw:Shoots four peas at your plants at a time.";
                }
                bool isGatling = false;
                for (int i = 0; i < chosenZombies.Count; i++)
                {
                    if (chosenZombies[i].mSeedType == GatlingPeaZombieSeed.Id)
                    {
                        isGatling = true;
                        break;
                    }
                }
                if (!isGatling)
                {
                    var s = new ChosenSeed();
                    s.mSeedState = temp.mSeedState;
                    s.mImitaterType = temp.mImitaterType;
                    s.mSeedType = GatlingPeaZombieSeed.Id;

                    chosenZombies.Add(s);
                }

                // Jalapeno Zombie \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                if (JalapenoZombieSeed.def == null)
                    JalapenoZombieSeed.Init(__instance.m_seedChooserScreen.mApp.GetPlantDefinition(SeedType.ZombiePail));
                // Seed Recharge Tooltip for custom zombies
                if (JalapenoZombieSeed.def != null)
                {
                    int refreshTime = JalapenoZombieSeed.def.m_versusBaseRefreshTime;
                    if (BloomConfig.TestBoolInput.Value)
                        JalapenoZombieSeed.def.m_plantToolTip = $"raw:<color=#00ff00>Recharge: {refreshTime / 100.0} seconds<color=#000000>\nExplodes and destroys a whole lane of plants after a while.";

                    else
                        JalapenoZombieSeed.def.m_plantToolTip = "raw:Explodes and destroys a whole lane of plants after a while.";
                }
                bool isJalapeno = false;
                for (int i = 0; i < chosenZombies.Count; i++)
                {
                    if (chosenZombies[i].mSeedType == JalapenoZombieSeed.Id)
                    {
                        isJalapeno = true;
                        break;
                    }
                }
                if (!isJalapeno)
                {
                    var s = new ChosenSeed();
                    s.mSeedState = temp.mSeedState;
                    s.mImitaterType = temp.mImitaterType;
                    s.mSeedType = JalapenoZombieSeed.Id;

                    chosenZombies.Add(s);
                }

                // Tallnut Zombie\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
                if (TallnutZombieSeed.def == null)
                    TallnutZombieSeed.Init(__instance.m_seedChooserScreen.mApp.GetPlantDefinition(SeedType.ZombiePail));
                // Seed Recharge Tooltip for custom zombies
                if (TallnutZombieSeed.def != null)
                {
                    int refreshTime = TallnutZombieSeed.def.m_versusBaseRefreshTime;
                    if (BloomConfig.TestBoolInput.Value)
                        TallnutZombieSeed.def.m_plantToolTip = $"raw:<color=#00ff00>Recharge: {refreshTime / 100.0} seconds<color=#000000>\nThis zombie has the health of a Tall-nut.";

                    else
                        TallnutZombieSeed.def.m_plantToolTip = "raw:This zombie has the health of a Tall-nut.";
                }
                bool isTallnut = false;
                for (int i = 0; i < chosenZombies.Count; i++)
                {
                    if (chosenZombies[i].mSeedType == TallnutZombieSeed.Id)
                    {
                        isTallnut = true;
                        break;
                    }
                }
                if (!isTallnut)
                {
                    var s = new ChosenSeed();
                    s.mSeedState = temp.mSeedState;
                    s.mImitaterType = temp.mImitaterType;
                    s.mSeedType = TallnutZombieSeed.Id;

                    chosenZombies.Add(s);
                }


                __instance.m_zombieEntriesModel.Clear();


                // Make Gravestone on top of seed list
                for (int i = 0; i < chosenZombies.Count; i++)
                {
                    var zombieChosenSeed = chosenZombies[i];
                    if (zombieChosenSeed.mSeedType == SeedType.None) continue;
                    if (zombieChosenSeed.mSeedType == SeedType.ZombieGravestone)
                    {
                        PlantDefinition zombieDefinition = __instance.m_seedChooserScreen.mApp.GetPlantDefinition(zombieChosenSeed.mSeedType);
                        if (zombieDefinition == null || zombieDefinition.m_versusBaseRefreshTime == 0) continue;

                        SeedChooserEntryModel zombieEntry = new SeedChooserEntryModel(zombieDefinition, zombieChosenSeed, __instance.m_seedChooserScreen, __instance, false, i);
                        __instance.m_zombieEntriesModel.Add(i.ToString(), zombieEntry);
                        break;
                    }


                }
                for (int i = 0; i < chosenZombies.Count; i++)
                {
                    var zombieChosenSeed = chosenZombies[i];
                    if (zombieChosenSeed.mSeedType == SeedType.None) continue;
                    if (zombieChosenSeed.mSeedType == SeedType.ZombieGravestone) continue;

                    PlantDefinition zombieDefinition = __instance.m_seedChooserScreen.mApp.GetPlantDefinition(zombieChosenSeed.mSeedType);
                    if (zombieDefinition == null || zombieDefinition.m_versusBaseRefreshTime == 0) continue;

                    SeedChooserEntryModel zombieEntry = new SeedChooserEntryModel(zombieDefinition, zombieChosenSeed, __instance.m_seedChooserScreen, __instance, false, i);
                    __instance.m_zombieEntriesModel.Add(i.ToString(), zombieEntry);
                }

                SeedPacketZombieSwapper();
                return false;
            }

            return true;
        }

    }
}
