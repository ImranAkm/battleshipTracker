using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleshipTracker.Model;

namespace BattleshipTracker.Interfaces
{
    public interface IBattleShipTrackerService
    {
        public IEnumerable<Board> CreateBoard(int numberOfPlayers, int boardSize);


        public BattleShip AddShip(string shipName, Board board, Position startPosition, Position endPosition, Direction shipDirection);

        public AttackResult Attack(Board board, Position attackPosition);
    }
}
