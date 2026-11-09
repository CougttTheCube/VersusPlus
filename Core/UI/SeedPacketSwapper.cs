namespace VersusPlus.UI;
#pragma warning disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BloomEngine.Helpers;
using BloomEngine.ModMenu;

using Il2CppBest.HTTP.SecureProtocol.Org.BouncyCastle.Tls;
using Il2CppBest.HTTP.SecureProtocol.Org.BouncyCastle.Utilities;

using Il2CppInterop.Runtime.Injection;

using MelonLoader;
using MelonLoader.Utils;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.UI;

using VersusPlus.Helper;

using static VersusPlus.Core.Core;

public class SeedPacketSwapper :  MonoBehaviour
{
        // Swap Texture2D with external image
        public static void SeedPacketZombieSwapper()
        {
            Texture2D targetZombieVSTexture = Resources.FindObjectsOfTypeAll<Texture2D>()
                .FirstOrDefault(t => t.name == "SPR_SeedPackets_Zombies_Versus");

            string GameRootDirectory = MelonEnvironment.GameRootDirectory;
            string imageDataPath = Path.Combine(GameRootDirectory, "Mods", "VersusPlus_Resources", "main.png"); //fallback

            if (BloomConfig.TestBoolInput15.Value) imageDataPath = Path.Combine(GameRootDirectory, "Mods", "VersusPlus_Resources", "new.png");
            else imageDataPath = Path.Combine(GameRootDirectory, "Mods", "VersusPlus_Resources", "main.png");

            if (File.Exists(imageDataPath))
            {
                byte[] fileData = File.ReadAllBytes(imageDataPath);
                if (ImageConversion.LoadImage(targetZombieVSTexture, fileData))
                {
                    MelonLogger.Msg("Patched SPR_SeedPackets_Zombies_Versus with custom image");
                }
                else MelonLogger.Msg("Failed to load image bytes");
            }
            else MelonLogger.Warning($"Could not find main.png at {imageDataPath}");

        }
    
}
