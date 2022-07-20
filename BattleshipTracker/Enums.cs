using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTracker
{
    public enum Direction
    {
         Horizontal = 1,
         Vertical = 2
    }

    public enum AttackResult
    {
        Hit,
        Miss,
        Sunk
    }
}
