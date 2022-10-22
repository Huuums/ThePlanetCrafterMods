﻿using BepInEx;
using SpaceCraft;
using HarmonyLib;
using System.Collections.Generic;
using System.Globalization;
using BepInEx.Logging;
using BepInEx.Configuration;
using System.IO;
using System;
using System.IO.Compression;
using System.Text;
using System.Collections;
using UnityEngine;
using MijuTools;
using BepInEx.Bootstrap;
using UnityEngine.InputSystem;
using System.Reflection;

namespace FeatTechniciansExile
{
    [BepInPlugin("akarnokd.theplanetcraftermods.feattechniciansexile", "(Feat) Technicians Exile", "0.1.0.0")]
    [BepInDependency(modFeatMultiplayerGuid, BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {

        const string modFeatMultiplayerGuid = "akarnokd.theplanetcraftermods.featmultiplayer";

        static ManualLogSource logger;

        internal static Texture2D technicianFront;
        internal static Texture2D technicianBack;

        static Func<string> multiplayerMode;

        // /tp 1990 76 1073
        static Vector3 technicianDropLocation = new Vector3(1995, 75.7f, 1073);

        static TechnicianAvatar avatar;

        static readonly int technicianWorldObjectIdStart = 70000;

        static WorldObject escapePod;
        static WorldObject livingPod;
        static WorldObject desk;
        static WorldObject bed;
        static WorldObject screen;
        static WorldObject grower;
        static WorldObject collector;
        static WorldObject flower;
        static WorldObject chair;
        static WorldObject chest;
        static WorldObject solar;
        static readonly List<WorldObject> baseComponents = new();

        static Vector3 technicianLocation1;
        static Quaternion technicianRotation1;

        static Vector3 technicianLocation2;
        static Quaternion technicianRotation2;

        static Vector3 technicianLocation3;
        static Quaternion technicianRotation3;

        enum QuestPhase
        {
            Not_Started,
            Arrival,
            Initial_Help,
            Base_Setup,
            Operating
        }

        static QuestPhase questPhase = QuestPhase.Not_Started;

        static bool ingame;

        static Asteroid asteroid;

        static FieldInfo asteroidHasCrashed;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin is loaded!");

            logger = Logger;

            if (Chainloader.PluginInfos.TryGetValue(modFeatMultiplayerGuid, out var pi))
            {
                multiplayerMode = (Func<string>)AccessTools.Field(pi.Instance.GetType(), "apiGetMultiplayerMode").GetValue(null);
            }
            Assembly me = Assembly.GetExecutingAssembly();
            string dir = Path.GetDirectoryName(me.Location);

            technicianFront = LoadPNG(Path.Combine(dir, "Technician_Front.png"));
            technicianBack = LoadPNG(Path.Combine(dir, "Technician_Back.png"));

            asteroidHasCrashed = AccessTools.Field(typeof(Asteroid), "hasCrashed");

            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        static Texture2D LoadPNG(string filename)
        {
            Texture2D tex = new Texture2D(100, 200);
            tex.LoadImage(File.ReadAllBytes(filename));

            return tex;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SessionController), "Start")]
        static void SessionController_Start()
        {
            bool dontSaveMe = true;

            logger.LogInfo("Start");

            baseComponents.Clear();

            logger.LogInfo("  Finding the Escape Pod");
            escapePod = WorldObjectsHandler.GetWorldObjectViaId(technicianWorldObjectIdStart);
            if (escapePod == null)
            {
                logger.LogInfo("    Creating the Escape Pod");
                escapePod = WorldObjectsHandler.CreateNewWorldObject(GroupsHandler.GetGroupViaId("EscapePod"), 
                    technicianWorldObjectIdStart);
                escapePod.SetPositionAndRotation(technicianDropLocation, Quaternion.identity * Quaternion.Euler(0, -90, 0));
                escapePod.SetDontSaveMe(true);
                WorldObjectsHandler.InstantiateWorldObject(escapePod, false);
            }

            var livingPodBase = technicianDropLocation + new Vector3(0, 0, 15);

            logger.LogInfo("  Finding the Living Pod");
            livingPod = WorldObjectsHandler.GetWorldObjectViaId(technicianWorldObjectIdStart + 1);
            if (livingPod == null)
            {
                logger.LogInfo("    Creating the Living Pod");
                livingPod = WorldObjectsHandler.CreateNewWorldObject(GroupsHandler.GetGroupViaId("pod"), 
                    technicianWorldObjectIdStart + 1);
                livingPod.SetPositionAndRotation(livingPodBase, Quaternion.identity * Quaternion.Euler(0, 90, 0));
                livingPod.SetPanelsId(new List<int> { 1, 4, 1, 1, 5, 7 });
                livingPod.SetDontSaveMe(dontSaveMe);
                WorldObjectsHandler.InstantiateWorldObject(livingPod, false);
            }

            var livingPodFloor = livingPodBase + new Vector3(0, 1, 0);

            var bedPosition = livingPodFloor + new Vector3(3, 0, -1);

            logger.LogInfo("  Finding the Bed");
            bed = WorldObjectsHandler.GetWorldObjectViaId(technicianWorldObjectIdStart + 2);
            if (bed == null)
            {
                logger.LogInfo("    Creating the Bed");
                bed = WorldObjectsHandler.CreateNewWorldObject(GroupsHandler.GetGroupViaId("BedSimple"), 
                    technicianWorldObjectIdStart + 2);
                bed.SetPositionAndRotation(bedPosition, Quaternion.identity * Quaternion.Euler(0, 180, 0));
                bed.SetDontSaveMe(dontSaveMe);
                WorldObjectsHandler.InstantiateWorldObject(bed, false);
            }

            var deskPosition = livingPodFloor + new Vector3(-1, 0, -3);
            logger.LogInfo("  Finding the Desk");
            desk = WorldObjectsHandler.GetWorldObjectViaId(technicianWorldObjectIdStart + 3);
            if (desk == null)
            {
                logger.LogInfo("    Creating the Desk");
                desk = WorldObjectsHandler.CreateNewWorldObject(GroupsHandler.GetGroupViaId("Desktop1"), 
                    technicianWorldObjectIdStart + 3);
                desk.SetPositionAndRotation(deskPosition, Quaternion.identity * Quaternion.Euler(0, -90, 0));
                desk.SetDontSaveMe(dontSaveMe);
                WorldObjectsHandler.InstantiateWorldObject(desk, false);
            }

            logger.LogInfo("  Finding the Rockets Screen");
            screen = WorldObjectsHandler.GetWorldObjectViaId(technicianWorldObjectIdStart + 4);
            if (screen == null)
            {
                logger.LogInfo("    Creating the Rockets Screen");
                screen = WorldObjectsHandler.CreateNewWorldObject(GroupsHandler.GetGroupViaId("ScreenRockets"),
                    technicianWorldObjectIdStart + 4);
                screen.SetPositionAndRotation(deskPosition + new Vector3(-0.5f, 1f, 0.25f), Quaternion.identity * Quaternion.Euler(0, 180, 0));
                screen.SetDontSaveMe(dontSaveMe);
                WorldObjectsHandler.InstantiateWorldObject(screen, false);
            }

            logger.LogInfo("  Finding the Grower");
            grower = WorldObjectsHandler.GetWorldObjectViaId(technicianWorldObjectIdStart + 5);
            if (grower == null)
            {
                logger.LogInfo("    Creating the Grower");
                grower = WorldObjectsHandler.CreateNewWorldObject(GroupsHandler.GetGroupViaId("VegetableGrower1"),
                    technicianWorldObjectIdStart + 5);
                grower.SetPositionAndRotation(livingPodFloor + new Vector3(2, 0, 2), Quaternion.identity * Quaternion.Euler(0, -45, 0));
                grower.SetDontSaveMe(dontSaveMe);
                WorldObjectsHandler.InstantiateWorldObject(grower, false);
            }

            logger.LogInfo("  Finding the Collector");
            collector = WorldObjectsHandler.GetWorldObjectViaId(technicianWorldObjectIdStart + 6);
            if (collector == null)
            {
                logger.LogInfo("    Creating the Collector");
                collector = WorldObjectsHandler.CreateNewWorldObject(GroupsHandler.GetGroupViaId("WaterCollector1"),
                    technicianWorldObjectIdStart + 6);
                collector.SetPositionAndRotation(livingPodBase + new Vector3(0, -1, 15), Quaternion.identity);
                collector.SetDontSaveMe(dontSaveMe);
                WorldObjectsHandler.InstantiateWorldObject(collector, false);
            }

            logger.LogInfo("  Finding Flower Pot");
            flower = WorldObjectsHandler.GetWorldObjectViaId(technicianWorldObjectIdStart + 7);
            if (flower == null)
            {
                logger.LogInfo("    Creating the Flower Pot");
                flower = WorldObjectsHandler.CreateNewWorldObject(GroupsHandler.GetGroupViaId("FlowerPot1"),
                    technicianWorldObjectIdStart + 7);
                flower.SetPositionAndRotation(livingPodFloor + new Vector3(-2, 0, 2.25f), Quaternion.identity);
                flower.SetDontSaveMe(dontSaveMe);
                WorldObjectsHandler.InstantiateWorldObject(flower, false);

                Inventory inv = InventoriesHandler.GetInventoryById(flower.GetLinkedInventoryId());
                WorldObject flowerSeed = WorldObjectsHandler.CreateNewWorldObject(GroupsHandler.GetGroupViaId("SeedGold"));
                if (!inv.AddItem(flowerSeed))
                {
                    WorldObjectsHandler.DestroyWorldObject(flowerSeed);
                }
            }

            logger.LogInfo("  Finding Chair");
            chair = WorldObjectsHandler.GetWorldObjectViaId(technicianWorldObjectIdStart + 8);
            if (chair == null)
            {
                logger.LogInfo("    Creating the Chair");
                chair = WorldObjectsHandler.CreateNewWorldObject(GroupsHandler.GetGroupViaId("Chair1"),
                    technicianWorldObjectIdStart + 8);
                chair.SetPositionAndRotation(deskPosition + new Vector3(1f, 0, 2), Quaternion.identity * Quaternion.Euler(0, -45, 0));
                chair.SetDontSaveMe(dontSaveMe);
                WorldObjectsHandler.InstantiateWorldObject(chair, false);
            }

            logger.LogInfo("  Finding Chest");
            chest = WorldObjectsHandler.GetWorldObjectViaId(technicianWorldObjectIdStart + 9);
            if (chest == null)
            {
                logger.LogInfo("    Creating the Chest");
                chest = WorldObjectsHandler.CreateNewWorldObject(GroupsHandler.GetGroupViaId("Container1"),
                    technicianWorldObjectIdStart + 9);
                chest.SetPositionAndRotation(livingPodFloor + new Vector3(-0.25f, 0, 2.25f), Quaternion.identity * Quaternion.Euler(0, 90, 0));
                chest.SetDontSaveMe(dontSaveMe);
                chest.SetText("Technician #221021");
                WorldObjectsHandler.InstantiateWorldObject(chest, false);
            }

            logger.LogInfo("  Finding Solar");
            solar = WorldObjectsHandler.GetWorldObjectViaId(technicianWorldObjectIdStart + 10);
            if (solar == null)
            {
                logger.LogInfo("    Creating the Solar");
                solar = WorldObjectsHandler.CreateNewWorldObject(GroupsHandler.GetGroupViaId("EnergyGenerator2"),
                    technicianWorldObjectIdStart + 10);
                solar.SetPositionAndRotation(livingPodFloor + new Vector3(0, 5, 0), Quaternion.identity);
                solar.SetDontSaveMe(dontSaveMe);
                WorldObjectsHandler.InstantiateWorldObject(solar, false);
            }

            technicianLocation1 = technicianDropLocation + new Vector3(0, -0.5f, 0);
            technicianRotation1 = Quaternion.identity * Quaternion.Euler(0, -90, 0);

            technicianLocation2 = deskPosition + new Vector3(-0.25f, 0, 2);
            technicianRotation2 = Quaternion.identity * Quaternion.Euler(0, 180, 0);

            technicianLocation3 = bedPosition + new Vector3(-0.85f, -0.45f, 0);
            technicianRotation3 = Quaternion.identity * Quaternion.Euler(-90, 0, 0);

            logger.LogInfo("  Creating the Technician");
            avatar = TechnicianAvatar.CreateAvatar(Color.white);
            avatar.SetPosition(technicianLocation1, technicianRotation1);

            logger.LogInfo("  Limiting object interactions");
            Destroy(livingPod.GetGameObject().GetComponentInChildren<ActionDeconstructible>());
            Destroy(desk.GetGameObject().GetComponentInChildren<ActionDeconstructible>());
            Destroy(bed.GetGameObject().GetComponentInChildren<ActionDeconstructible>());
            Destroy(screen.GetGameObject().GetComponentInChildren<ActionDeconstructible>());
            Destroy(grower.GetGameObject().GetComponentInChildren<ActionDeconstructible>());
            Destroy(grower.GetGameObject().GetComponentInChildren<ActionnableInteractive>());
            Destroy(grower.GetGameObject().GetComponentInChildren<ActionOpenable>());
            Destroy(collector.GetGameObject().GetComponentInChildren<ActionDeconstructible>());
            Destroy(collector.GetGameObject().GetComponentInChildren<ActionnableInteractive>());
            Destroy(collector.GetGameObject().GetComponentInChildren<ActionOpenable>());
            Destroy(flower.GetGameObject().GetComponentInChildren<ActionDeconstructible>());
            Destroy(flower.GetGameObject().GetComponentInChildren<ActionnableInteractive>());
            Destroy(flower.GetGameObject().GetComponentInChildren<ActionOpenable>());
            Destroy(chair.GetGameObject().GetComponentInChildren<ActionDeconstructible>());
            Destroy(chest.GetGameObject().GetComponentInChildren<ActionnableInteractive>());
            Destroy(chest.GetGameObject().GetComponentInChildren<ActionOpenable>());
            Destroy(chest.GetGameObject().GetComponentInChildren<ActionDeconstructible>());
            Destroy(chest.GetGameObject().GetComponentInChildren<ActionOpenUi>());
            Destroy(solar.GetGameObject().GetComponentInChildren<ActionDeconstructible>());

            baseComponents.Add(livingPod);
            baseComponents.Add(desk);
            baseComponents.Add(bed);
            baseComponents.Add(screen);
            baseComponents.Add(grower);
            baseComponents.Add(collector);
            baseComponents.Add(flower);
            baseComponents.Add(chair);
            baseComponents.Add(chest);
            baseComponents.Add(solar);

            LoadState();
            SetVisibilityViaCurrentPhase();

            ingame = true;
            logger.LogInfo("Done");
        }

        void Update()
        {
            if (!ingame)
            {
                return;
            }

            var player = GetPlayerMainController();

            switch (questPhase)
            {
                case QuestPhase.Not_Started:
                    {
                        CheckStartConditions();
                        break;
                    }
                case QuestPhase.Arrival:
                    {
                        CheckArrival();
                        break;
                    }
                case QuestPhase.Initial_Help:
                    {
                        CheckInitialHelp();
                        break;
                    }
                case QuestPhase.Base_Setup:
                    {
                        CheckBaseSetup();
                        break;
                    }
                case QuestPhase.Operating:
                    {
                        CheckOperating();
                        break;
                    }
            }
        }

        void CheckStartConditions()
        {
            var wu = Managers.GetManager<WorldUnitsHandler>();
            var wut = wu.GetUnit(DataConfig.WorldUnitType.Terraformation);
            if (wut.GetValue() >= 0/* 1100000 */)
            {
                questPhase = QuestPhase.Arrival;
                SaveState();
                SetVisibilityViaCurrentPhase();
            }
        }

        void CheckArrival()
        {
            if (asteroid == null)
            {
                var mh = Managers.GetManager<MeteoHandler>();
                /*
                foreach (var me in mh.meteoEvents)
                {
                    logger.LogInfo("Dump meteo events: " + me.environmentVolume.name);
                }*/
                var meteoEvent = mh.meteoEvents[9];
                logger.LogInfo("Launching arrival meteor: " + meteoEvent.environmentVolume.name);

                mh.meteoSound.StartMeteoAudio(meteoEvent);
                if (meteoEvent.asteroidEventData != null)
                {
                    var selectedAsteroidEventData = UnityEngine.Object.Instantiate(meteoEvent.asteroidEventData);

                    var ah = Managers.GetManager<AsteroidsHandler>();

                    GameObject obj = Instantiate(
                        selectedAsteroidEventData.asteroidGameObject,
                        technicianDropLocation + new Vector3(0, 1000, 0),
                        Quaternion.identity,
                        ah.gameObject.transform
                    );
                    obj.transform.LookAt(technicianDropLocation);
                    
                    asteroid = obj.GetComponent<Asteroid>();
                    asteroid.SetLinkedAsteroidEvent(selectedAsteroidEventData);
                    asteroid.debrisDestroyTime = 5;
                    asteroid.placeAsteroidBody = false;
                    selectedAsteroidEventData.ChangeExistingAsteroidsCount(1);
                    selectedAsteroidEventData.ChangeTotalAsteroidsCount(1);
                }
            } 
            else
            {
                if ((bool)asteroidHasCrashed.GetValue(asteroid))
                {
                    asteroid = null;
                    questPhase = QuestPhase.Initial_Help;
                    SaveState();
                    SetVisibilityViaCurrentPhase();
                }
            }
        }

        void CheckInitialHelp()
        {

        }

        void CheckBaseSetup()
        {

        }

        void CheckOperating()
        {

        }
            
        internal static PlayerMainController GetPlayerMainController()
        {
            PlayersManager p = Managers.GetManager<PlayersManager>();
            if (p != null)
            {
                return p.GetActivePlayerController();
            }
            return null;
        }

        static void SetVisibilityViaCurrentPhase()
        {
            switch (questPhase)
            {
                case QuestPhase.Not_Started:
                case QuestPhase.Arrival:
                    {
                        avatar.SetVisible(false);
                        escapePod.GetGameObject().SetActive(false);

                        foreach (var wo in baseComponents)
                        {
                            wo.GetGameObject().SetActive(false);
                        }
                        break;
                    }
                case QuestPhase.Initial_Help:
                    {
                        avatar.SetVisible(true);
                        escapePod.GetGameObject().SetActive(true);

                        foreach (var wo in baseComponents)
                        {
                            wo.GetGameObject().SetActive(false);
                        }
                        break;
                    }
                case QuestPhase.Base_Setup:
                    {
                        avatar.SetVisible(true);
                        escapePod.GetGameObject().SetActive(true);

                        foreach (var wo in baseComponents)
                        {
                            wo.GetGameObject().SetActive(false);
                        }
                        break;
                    }
                case QuestPhase.Operating:
                    {
                        avatar.SetVisible(true);
                        escapePod.GetGameObject().SetActive(true);

                        foreach (var wo in baseComponents)
                        {
                            wo.GetGameObject().SetActive(true);
                        }
                        break;
                    }
            }
        }

        static void LoadState()
        {
            string state = escapePod.GetText();
            if (string.IsNullOrEmpty(state))
            {
                questPhase = QuestPhase.Not_Started;
            }
            else
            {
                // TODO
            }
        }

        static void SaveState()
        {

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UiWindowPause), nameof(UiWindowPause.OnQuit))]
        static void UiWindowPause_OnQuit()
        {
            ingame = false;
        }

    }
}
