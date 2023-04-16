﻿using BepInEx;
using SpaceCraft;
using HarmonyLib;
using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;
using BepInEx.Bootstrap;
using System.Reflection;
using BepInEx.Configuration;

namespace UIShowETA
{
    [BepInPlugin("akarnokd.theplanetcraftermods.uishoweta", "(UI) Show ETA", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin is loaded!");


            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ScreenTerraStage), "RefreshDisplay", new Type[0])]
        static void ScreenTerraStage_RefreshDisplay(
            TextMeshProUGUI ___percentageProcess, TerraformStagesHandler ___terraformStagesHandler)
        {
            TerraformStage nextGlobalStage = ___terraformStagesHandler.GetNextGlobalStage();
            if (nextGlobalStage == null)
            {
                ___percentageProcess.text += "<color=#FFFF00>ETA</color><br>Done";
            }
            var wuh = Managers.GetManager<WorldUnitsHandler>();
            var speed = wuh.GetUnit(nextGlobalStage.GetWorldUnitType()).GetCurrentValuePersSec();
            var remaining = nextGlobalStage.GetStageStartValue() - wuh.GetUnit(nextGlobalStage.GetWorldUnitType()).GetValue();

            if (speed <= 0)
            {
                ___percentageProcess.text += "<br><color=#FFFF00>ETA</color><br>Infinite";
            }
            else
            {
                var time = (long)(remaining / speed);
                var ts = TimeSpan.FromSeconds(time);

                if (ts.Days > 0)
                {
                    ___percentageProcess.text += string.Format("<br><color=#FFFF00>ETA</color><br>{0:#} days<br>{1}:{2:00}:{3:00}", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
                }
                else
                {
                    ___percentageProcess.text += string.Format("<br><color=#FFFF00>ETA</color><br>{1}:{2:00}:{3:00}", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
                }
            }
        }
    }
}