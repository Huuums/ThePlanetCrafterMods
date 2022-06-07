﻿using SpaceCraft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FeatMultiplayer
{
    internal class MessageAsteroidSpawn : MessageStringProvider
    {
        internal string rocketGroupId;
        internal int eventIndex;
        internal Vector3 spawnPosition;
        internal Vector3 landingPosition;

        internal static bool TryParse(string str, out MessageAsteroidSpawn mas)
        {
            if (MessageHelper.TryParseMessage("AsteroidSpawn|", str, 5, out var parameters))
            {
                try
                {
                    mas = new();
                    mas.rocketGroupId = parameters[1];
                    mas.eventIndex = int.Parse(parameters[2]);
                    mas.spawnPosition = DataTreatments.StringToVector3(parameters[3]);
                    mas.landingPosition = DataTreatments.StringToVector3(parameters[4]);
                    return true;
                }
                catch (Exception ex)
                {
                    Plugin.LogError(ex);
                }
            }
            mas = null;
            return false;
        }

        public string GetString()
        {
            return "AsteroidSpawn|" + rocketGroupId
                + "|" + eventIndex
                + "|" + DataTreatments.Vector3ToString(spawnPosition)
                + "|" + DataTreatments.Vector3ToString(landingPosition)
                + "\n";
        }
    }
}