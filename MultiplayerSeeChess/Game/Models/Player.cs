using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiplayerSeeChess.Game.Models
{
    public class Player
    {
        public string UserName { get; set; }

        public string ConnectionId { get; set; }

        public Player(string userName, string connectionId)
        {
            this.UserName = userName;
            this.ConnectionId = connectionId;
        }
    }
}
