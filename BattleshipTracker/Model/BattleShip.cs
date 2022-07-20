using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTracker.Model
{
    public class BattleShip
    {  
        public string ShipName { get; set; }

        public string BoardName { get; set; }
        public Position Start_Position {get;set;}
        public Position End_position { get; set; }

        public Direction Direction { get; set; }
    }
}
