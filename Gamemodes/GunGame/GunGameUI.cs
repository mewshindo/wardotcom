using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using WarDotCom.UI;

namespace WarDotCom.Gamemodes.GunGame
{
    public class GunGameUI : GamemodeUI
    {
        private byte _activePlayerCards;
        private readonly Dictionary<Player, GunGamePlayer> _players;
        public GunGameUI(EffectAsset ui, Dictionary<Player, GunGamePlayer> players)
        {
            _players = players;
            SendUI_AllPlayers(ui);
        }

        public void UpdateClock(float secondsRemaining)
        {
            if(secondsRemaining < 0f)
                secondsRemaining = 0f;

            int totalSeconds = (int)Math.Ceiling(secondsRemaining);

            SendUIVisibility_AllPlayers($"Arrow_{59 - (secondsRemaining % 60)}", true);

            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            SendUIText_AllPlayers("Clock_Text", $"{minutes:D2}:{seconds:D2}");
        }

        public void UpdateTopScoreboard()
        {
            var playersSorted = _players.Values.OrderByDescending(b => b.Score).ToList();

            if(_activePlayerCards > playersSorted.Count)
            {
                for(int i = 7; i > playersSorted.Count; i--)
                {
                    SendUIVisibility_AllPlayers($"Canvas/Container/Players_UI/Players_Container/Player_{i}", false);
                }
                _activePlayerCards = (byte)playersSorted.Count;
            }

            for(int i = 0; i < playersSorted.Count && i < 8; i++)
            {
                if(_activePlayerCards <= i)
                {
                    SendUIVisibility_AllPlayers($"Canvas/Container/Players_UI/Players_Container/Player_{i}", true);
                    _activePlayerCards++;
                }
                var pfp = PlayerUIManager.CSteamIDAvatarCache[playersSorted[i].CSteamID];
                SendUIImage_AllPlayers($"Canvas/Container/Players_UI/Players_Container/Player_{i}/Avatar_Container/Avatar", pfp);
                SendUIText_AllPlayers($"Canvas/Container/Players_UI/Players_Container/Player_{i}/Stats/Score/Kills", $"{playersSorted[i].Kills}");
                SendUIVisibility_AllPlayers($"Canvas/Container/Players_UI/Players_Container/Player_{i}/Stats/{playersSorted[i].CurrentWeapon}", true);
            }
        }
    }
}
