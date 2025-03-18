﻿using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MomodoraMFRandomizer.Patches
{
    class DeathLinkHandler
    {

        private Boolean isDead = false;
        public void CheckDeathLink(DeathLinkService deathLinkService, String username)
        {
            if (!isDead)
            {
                deathLinkService.OnDeathLinkReceived += (deathLinkObject) =>
                {
                    Platformer3D.player_hp = 0f;
                    isDead = true;
                };
            
                if (Platformer3D.player_hp == 0f)
                {
                    MelonLogger.Msg("deathlink sent");
                    deathLinkService.SendDeathLink(new DeathLink(username));
                    isDead = true;
                }
            }
            if (Platformer3D.player_hp >= 1)
            {
                isDead = false;
            }
        }
    }

}
