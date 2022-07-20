using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipTracker.Model
{
    public class Board
    {
        public string BoardName { get; set; }

        public List<Cell> Cells { get; set; } 

        public Board(string name, int boardSize)
        {
            BoardName = name;
            Cells = new List<Cell>();
            for (int i = 1; i <= boardSize; i++)
            {
                for (int j = 1; j <= boardSize; j++)
                {
                    Cells.Add(new Cell(i, j));
                }
            }
        }
    }
}
