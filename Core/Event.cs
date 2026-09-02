namespace VersusPlus;
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Il2CppReloaded.Data;
using Il2CppReloaded.Gameplay;

using MelonLoader;

using ReplantAPI;
using ReplantAPI.Managers;

using UnityEngine;
using UnityEngine.InputSystem;

using static VersusPlus.Core;

internal class Event : MonoBehaviour
{
    public void StartMod()
    {
        // Subscribe to events using +=
        MelonLogger.Msg("Plant Event initialized!");
        
    }

    // Call this if your mod unloads or is disabled
    public void StopMod()
    {
        // Always unsubscribe using -= to prevent memory leaks and infinite loops
        
    }

    // Matches Action<Plant>
    private void HandlePlantSpawned(Plant spawnedPlant)
    {
        // Example: Apply a custom buff or effect to the plant as soon as it is placed
        Console.WriteLine($"A plant was placed: {spawnedPlant.mSeedType}");
    }
}
