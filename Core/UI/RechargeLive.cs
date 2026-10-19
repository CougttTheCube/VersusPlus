namespace VersusPlus.UI;
#pragma warning disable
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using HarmonyLib;

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
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

using static MelonLoader.MelonLogger;
using VersusPlus.Core;
using static VersusPlus.Core.Core;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

internal class Timer2 : MonoBehaviour
{


    private TextMeshProUGUI[] RechargeText = new TextMeshProUGUI[6];
    private TextMeshProUGUI[] RechargeTextZ = new TextMeshProUGUI[6];
    float[] elapsedTime = new float[6];
    float[] elapsedTime2 = new float[6];

    //Panels/P_Gameplay_MainHUD/Canvas/Layout/Center/TopLeftLayout/SeedBankContainer/SeedBank/

    void Start()
    {

        //MelonLogger.Msg("Timer2 initialized!");
        Canvas customCanvas = gameObject.AddComponent<Canvas>();
        customCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

    

        customCanvas.sortingOrder = 0;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080); // Your baseline design resolution
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        GameObject seedContainerPlants = GameObject.Find("Panels/P_Gameplay_MainHUD/Canvas/Layout/Center/TopLeftLayout/SeedBankContainer/SeedBank/SeedPacks_Layout");
        GameObject seedContainerZombies = GameObject.Find("Panels/P_Gameplay_MainHUD/Canvas/Layout/Center/VersusBank/VersusBankContainer/P_VsZombiePacks_Layout");


        // Create the live recharge text objects

        if (seedContainerPlants != null)
        {
            for (int i = 0; i < 6; i++)
            {
                Transform slotTransform = seedContainerPlants.transform.GetChild(i + 3);

                GameObject textObj = new GameObject($"VersusMatchTimer{i}");
                textObj.transform.SetParent(slotTransform, false);
                textObj.transform.SetAsLastSibling();

                RechargeText[i] = textObj.AddComponent<TextMeshProUGUI>();
                RechargeText[i].color = Color.white;
                RechargeText[i].fontSize = 55;
                RechargeText[i].alignment = TextAlignmentOptions.Center;
                RechargeText[i].characterSpacing = 2f;

                RectTransform rect = RechargeText[i].GetComponent<RectTransform>();
                //Set center which is the middle of the screen
                rect.anchorMin = new Vector2(1f, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                // Set center in the text
                rect.pivot = new Vector2(0.5f, 0.5f);
                // Move
                rect.localPosition = new Vector2(0, 0);

                RechargeText[i].raycastTarget = false;

            }
        }
        if (seedContainerZombies != null)
        {
            for (int i = 0; i < 6; i++)
            {
                Transform slotTransform = seedContainerZombies.transform.GetChild(i + 3);

                GameObject textObj = new GameObject($"VersusMatchTimer{i}");
                textObj.transform.SetParent(slotTransform, false);
                textObj.transform.SetAsLastSibling();

                RechargeTextZ[i] = textObj.AddComponent<TextMeshProUGUI>();
                RechargeTextZ[i].color = Color.white;
                RechargeTextZ[i].fontSize = 55;
                RechargeTextZ[i].alignment = TextAlignmentOptions.Center;
                RechargeTextZ[i].characterSpacing = 2f;

                RectTransform rect = RechargeTextZ[i].GetComponent<RectTransform>();
                //Set center which is the middle of the screen
                rect.anchorMin = new Vector2(1f, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                // Set center in the text
                rect.pivot = new Vector2(0.5f, 0.5f);
                // Move
                rect.localPosition = new Vector2(0, 0);

                RechargeTextZ[i].raycastTarget = false;

            }
        }

        var loadedFonts = Resources.FindObjectsOfTypeAll(Il2CppInterop.Runtime.Il2CppType.Of<TMP_FontAsset>());

        foreach (var obj in loadedFonts)
        {
            var font = obj.Cast<TMP_FontAsset>();
            //MelonLogger.Msg($"Found font: {font.name}");
            if (font.name == "FNT_BrianneTod SDF") // check actual names via logging
            {
                for (int i = 0; i < 6; i++){
                    RechargeText[i].font = font;
                    RechargeTextZ[i].font = font;
                }
                break;
            }
        }




    }

    void Update()
    {
        if (BoardManager.Board == null ) return;


        if (BoardManager.Board.m_startDrawTime == 0)
        {
            if (BloomConfig.TestBoolInput2.Value){
                for (int index = 0; index < 2; index++)
                {
                    for (int j = 0; j < 6; j++)
                    {

                        SeedPacket temp = SeedBankManager.GetPacketAt(j, index);
                        if (temp == null) continue;

                        bool isZombieSeed = ((int)temp.mPacketType >= 60 && (int)temp.mPacketType <= 80);

                        if (!isZombieSeed)
                        {
                            elapsedTime[j] = (temp.mRefreshTime - temp.mRefreshCounter) / 100f;

                            RechargeText[j].color = Color.white;
                            if (elapsedTime[j] >= 10) RechargeText[j].text = $"{(int)elapsedTime[j]}";
                            else  RechargeText[j].text = $"{elapsedTime[j]:0.0}";


                            if (temp.mActive || Mathf.Abs(elapsedTime[j] - temp.mRefreshTime) < 1e-9) RechargeText[j].color = new Color(1f, 1f, 1f, 0.0f);
                            
                        }
                        else
                        {
                            elapsedTime2[j] = (temp.mRefreshTime - temp.mRefreshCounter) / 100f;

                            RechargeTextZ[j].color = Color.white;
                            if (elapsedTime2[j] >= 10) RechargeTextZ[j].text = $"{(int)elapsedTime2[j]}";
                            else RechargeTextZ[j].text = $"{elapsedTime2[j]:0.0}";

                            if (BoardManager.Board.mApp.VersusMode.Phase == VersusPhase.SuddenDeath) {
                                if (j == 0) RechargeTextZ[j].color = new Color(1f, 1f, 1f, 0.0f);
                                else {
                                    if (temp.mActive || Mathf.Abs(elapsedTime2[j] - temp.mRefreshTime) < 1e-9) RechargeTextZ[j].color = new Color(1f, 1f, 1f, 0.0f);
                                }
                            } else {
                                if (temp.mActive || Mathf.Abs(elapsedTime2[j] - temp.mRefreshTime) < 1e-9) RechargeTextZ[j].color = new Color(1f, 1f, 1f, 0.0f);
                            }
                            
                            
                        }
                        
                    }
                }


            } else {
                //lol
            }
            
            




        }
    }
}
