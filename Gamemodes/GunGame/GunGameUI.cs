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
        private readonly List<GunGamePlayer> _players;
        public GunGameUI(EffectAsset ui, List<GunGamePlayer> players)
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
            var playersSorted = _players.OrderByDescending(b => b.Score).ToList();

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

        public void ShowLeaderboard(Player winner = null)
        {
            if(winner == null)
                SendUIVisibility_AllPlayers($"Canvas/Leaderboard/Draw", true);
            else
            {
                SendUIVisibility_AllPlayers($"Canvas/Leaderboard/WinnerPlayer", true);
                SendUIImage_AllPlayers($"Canvas/Leaderboard/WinnerPlayer/Player/Avatar", PlayerUIManager.CSteamIDAvatarCache[winner.channel.owner.playerID.steamID.m_SteamID]);
                SendUIText_AllPlayers($"Canvas/Leaderboard/WinnerPlayer/Player", winner.channel.owner.playerID.characterName);
            }

            var playersSorted = _players.OrderByDescending(b => b.Score).ToList();

            SendUIVisibility_AllPlayers("Canvas/Leaderboard", true);
            for (int i = 0; i < _players.Count && i < 16; i++)
            {
                SendUIVisibility_AllPlayers($"Canvas/Leaderboard/Player_{i}", true);
                SendUIText_AllPlayers($"Canvas/Leaderboard/Player_{i}/Player", playersSorted[i].Player.channel.owner.playerID.characterName);
                SendUIImage_AllPlayers($"Canvas/Leaderboard/Player_{i}/Player/Avatar", PlayerUIManager.CSteamIDAvatarCache[playersSorted[i].Player.channel.owner.playerID.steamID.m_SteamID]);
                SendUIText_AllPlayers($"Canvas/Leaderboard/Player_{i}/Kills", $"{playersSorted[i].Kills}");
                SendUIText_AllPlayers($"Canvas/Leaderboard/Player_{i}/Deaths", $"{playersSorted[i].Deaths}");
                SendUIText_AllPlayers($"Canvas/Leaderboard/Player_{i}/Headshot%", $"{playersSorted[i].HeadshotRate}");
                SendUIText_AllPlayers($"Canvas/Leaderboard/Player_{i}/Damage", $"{playersSorted[i].DamageDealt}");
                SendUIText_AllPlayers($"Canvas/Leaderboard/Player_{i}/Score", $"{playersSorted[i].Score}");
            }
        }
    }
}
