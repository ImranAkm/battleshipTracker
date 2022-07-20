using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTracker.Model
{
    public class Position
    {
        public int X_Co_Ordinate { get; set; }
        public int Y_Co_Ordinate { get; set; }

        public Position(int x, int y)
        {
            X_Co_Ordinate = x;
            Y_Co_Ordinate = y;
        }
    }
}
