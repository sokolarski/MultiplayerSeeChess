using MultiplayerSeeChess.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiplayerSeeChess.Services
{
    public class GameService : IGameService
    {
        private Dictionary<string, string> connectedPlayers = new Dictionary<string, string>();
        private Dictionary<string, NewGame> playedGames = new Dictionary<string, NewGame>();

        public NewGame GetGameByID(string id)
        {
            if (playedGames.ContainsKey(id))
            {
                return playedGames[id];
            }
            return null;
        }

        public bool RemoveWaitingPlayer(string userName)
        {
            if (connectedPlayers.ContainsKey(userName))
            {
                connectedPlayers.Remove(userName);
                return true;
            }
            return false;
        }

        public KeyValuePair<string,NewGame> GetGameByPlayer(string username)
        {
            var game = playedGames.Where(g => (g.Value.FirstPlayer.UserName == username) || (g.Value.SecondPlayer.UserName == username))
                                  .FirstOrDefault();
            return game;
        }

        public bool RemoveGame(string gameId)
        {
            return playedGames.Remove(gameId);
        }

        public bool ContainsGame(string gameId)
        {
            return playedGames.ContainsKey(gameId);
        }
        public string StartGame(string userName, string userConnId)
        {
            if (!connectedPlayers.ContainsKey(userName))
            {
                connectedPlayers.Add(userName, userConnId);
            }
            if (connectedPlayers.Count() == 2)
            {
                var guid = Guid.NewGuid().ToString(); ;
                var game = new NewGame(guid);
                foreach (var player in connectedPlayers)
                {
                    game.AddUser(player.Key, player.Value);
                }
                playedGames.Add(guid, game);
                connectedPlayers.Clear();
                return guid;
            }
            return null;
        }
    }
}
