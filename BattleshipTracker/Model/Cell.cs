using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTracker.Model
{
    public class Cell
    {
        public Position Co_Ordinates { get; set; }

        public Cell(int x, int y)
        {
            Co_Ordinates = new Position(x, y);
        }

        
    }
}
