using MultiplayerSeeChess.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiplayerSeeChess.Game
{
    public class NewGame
    {
        public string ID { get; }

        public Player FirstPlayer { get; set; }

        public Player SecondPlayer { get; set; }

        public int CountOfTurns { get; set; }
        public byte[,] GameTable { get; set; }
        public NewGame(string id)
        {
            this.ID = id;
            this.GameTable = new byte[3, 3];
        }

        public StatusGame Turn(string userName, int x, int y)
        {
            if (IsTurn(userName) && (GameTable[x, y] == (byte)0))
            {
                byte symbol = Set(userName);
                GameTable[x, y] = symbol;
                CountOfTurns++;
                if (IsFinish(symbol))
                {
                    return StatusGame.Winner;
                }
                if (CountOfTurns == 9)
                {
                    return StatusGame.End;
                }
                return StatusGame.Ok;
            }
            return StatusGame.Error;
        }


        private bool IsFinish(byte symbol)
        {
            for (int i = 0; i < 3; i++)
            {
                bool line = true;
                bool row = true;
                for (int j = 0; j < 3; j++)
                {
                    if (GameTable[i, j] != symbol)
                    {
                        line = false;
                    }

                    if (GameTable[j, i] != symbol)
                    {
                        row = false;
                    }
                }
                if (line || row)
                {
                    return true;
                }
            }

            if ((GameTable[0, 0] == symbol && GameTable[1, 1] == symbol && GameTable[2, 2] == symbol) ||
                (GameTable[0, 2] == symbol && GameTable[1, 1] == symbol && GameTable[2, 0] == symbol))
            {
                return true;
            }
            return false;
        }

        private byte Set(string userName)
        {
            if (userName == FirstPlayer.UserName)
            {
                return 1;
            }
            return 2;
        }

        public bool IsTurn(string userName)
        {
            if ((CountOfTurns % 2) == 0 && userName == FirstPlayer.UserName)
            {
                return true;
            }
            else if ((CountOfTurns % 2) == 1 && userName == SecondPlayer.UserName)
            {
                return true;
            }
            return false;
        }

        public void AddUser(string userName, string userConnId)
        {
            if (FirstPlayer == null)
            {
                FirstPlayer = new Player(userName, userConnId);
            }
            else if (SecondPlayer == null)
            {
                SecondPlayer = new Player(userName, userConnId);
            }
            else
            {
                throw new Exception("Can`t add more players!");
            }
        }

        public Player GetOpponentUsernameAndConnId(string userName)
        {
            if (FirstPlayer.UserName == userName)
            {
                return SecondPlayer;
            }
            else
            {
                return FirstPlayer;
            }

        }

        public void ClearGame()
        {
            this.CountOfTurns = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    this.GameTable[i, j] = 0;
                }
            }
        }
    }
}
