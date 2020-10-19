using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiplayerSeeChess.Game.Models
{
    public class Point
    {
        public byte X { get; set; }

        public byte Y { get; set; }

        public char Symbol { get; set; }

        public Point()
        {

        }

        public Point(byte x , byte y, char symbol)
        {
            this.X = x;
            this.Y = y;
            this.Symbol = symbol;
        }
    }
}
