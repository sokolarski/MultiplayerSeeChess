using MultiplayerSeeChess.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiplayerSeeChess.Services
{
    public interface IGameService
    {
        public string StartGame(string userName, string userConnId);
        public NewGame GetGameByID(string id);
        public bool RemoveGame(string id);
        public bool ContainsGame(string id);
        public bool RemoveWaitingPlayer(string userName);
        public KeyValuePair<string, NewGame> GetGameByPlayer(string username);
    }
}
