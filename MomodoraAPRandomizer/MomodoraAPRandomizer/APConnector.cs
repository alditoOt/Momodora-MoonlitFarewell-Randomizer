﻿using HarmonyLib;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;
using BepInEx.Logging;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MomodoraAPRandomizer.Patches
{
    class APConnector
    {
        private static string game = "Momodora Moonlit Farewell";
        private static ManualLogSource Logger = APRandomizer.Logger;

        private static LoginResult TryToLogin(ArchipelagoSession session, string user)
        {
            var result = session.TryConnectAndLogin(game, user, ItemsHandlingFlags.AllItems, version: new Version(0, 5, 1));
            return result;
        }

        public static void Connect(ArchipelagoSession session, string server, string user, string pass)
        {
            LoginResult result;
            try
            {
                result = TryToLogin(session, user);
            }
            catch (Exception e)
            {
                result = new LoginFailure(e.GetBaseException().Message);
                Logger.LogInfo("Error connecting to Archipelago: " + e.Message);
            }

            if (!result.Successful)
            {
                LoginFailure failure = (LoginFailure)result;
                string errorMessage = $"Failed to Connect to {server} as {user}:";
                foreach (string error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }
                foreach (ConnectionRefusedError error in failure.ErrorCodes)
                {
                    errorMessage += $"\n    {error}";
                }

                Logger.LogInfo(errorMessage);
                return; // Did not connect, show the user the contents of `errorMessage`
            }

            var loginSuccess = (LoginSuccessful)result;
            Logger.LogInfo("soy fans");
        }

        public void ReportLocation(ArchipelagoSession session, int location)
        {
            session.Locations.CompleteLocationChecks(location);
        }

        public void UpdateItems(ArchipelagoSession session, int itemId)
        {
            session.Items.ItemReceived += (receivedItemsHelper) =>
            {
                var itemReceivedName = receivedItemsHelper.PeekItem().ItemName ?? $"Item: {itemId}";
                //momoEventHandler.GiveSkill(itemId);
                receivedItemsHelper.DequeueItem();
            };
        }

        public static void CheckDeathLink(DeathLinkService deathLinkService, String username)
        {
            deathLinkService.OnDeathLinkReceived += (deathLinkObject) =>
            {
                Platformer3D.player_hp = 0f;
            };

            if (Platformer3D.player_hp == 0f)
            {
                Logger.LogInfo("Sending Death Link");
                deathLinkService.SendDeathLink(new DeathLink(username));
            }
        }

        public static void SendCompletion(ArchipelagoSession session)
        {
            var statusUpdatePacket = new StatusUpdatePacket();
            statusUpdatePacket.Status = ArchipelagoClientState.ClientGoal;
            session.Socket.SendPacket(statusUpdatePacket);
        }
    }
}
