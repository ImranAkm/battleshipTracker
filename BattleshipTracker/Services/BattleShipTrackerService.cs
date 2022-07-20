using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleshipTracker.Interfaces;
using BattleshipTracker.Model;

namespace BattleshipTracker.Services
{
    public class BattleShipTrackerService : IBattleShipTrackerService
    {
        public Dictionary<string, List<Position>> Ships;
        public Dictionary<Board, List<Position>> Attacks;

        public BattleShipTrackerService()
        {
            Ships = new Dictionary<string, List<Position>>();
            Attacks = new Dictionary<Board, List<Position>>();
        }
        public IEnumerable<Board> CreateBoard(int numberOfPlayers, int boardSize)
        {
            if(numberOfPlayers < 1 || boardSize < 1)
            {
                return null;
            }
            var allBoards = new List<Board>();
            for (int i = 1; i <= numberOfPlayers; i++)
            {
                allBoards.Add(new Board($"Player-{i}", boardSize));
            }
            return allBoards;
        }


        public BattleShip AddShip(string shipName, Board board, Position startPosition, Position endPosition, Direction shipDirection)
        {
            if(board == null || startPosition == null || endPosition == null || string.IsNullOrEmpty(shipName))
            {
                throw new ApplicationException("Invalid request to add Battle ship");
            }
            if(IsPositionValid(board, startPosition, endPosition, shipDirection))
            {
                var ship = new BattleShip();
                ship.ShipName = shipName;
                ship.BoardName = board.BoardName;
                ship.Direction = shipDirection;
                ship.Start_Position = startPosition;
                ship.End_position = endPosition;

                var positions = new List<Position>();
                if(shipDirection == Direction.Horizontal)
                {
                    for (int i = startPosition.Y_Co_Ordinate; i <= endPosition.Y_Co_Ordinate; i++)
                    {
                        positions.Add(new Position(startPosition.X_Co_Ordinate, i));
                    }
                }
                else
                {
                    for (int i = startPosition.X_Co_Ordinate; i <= endPosition.X_Co_Ordinate; i++)
                    {
                        positions.Add(new Position(i, startPosition.Y_Co_Ordinate));
                    }
                }

                if (Ships.ContainsKey(shipName))
                {
                    positions.RemoveAll(x => Ships[shipName].Contains(x));
                    Ships[shipName].AddRange(positions);
                }
                else
                {
                    Ships.Add(shipName, positions);
                }
                return ship;
            }
            else
            {
                throw new ApplicationException("Invalid position for the Battle ship.");
            }
            
        }

        public AttackResult Attack(Board board, Position attackPosition)
        {
            if(board == null || attackPosition == null || Ships == null || Ships.Count == 0)
            {
                throw new ApplicationException("The initial setup is incomplete. Setup board and ship before attacking.");
            }


            if (Attacks.ContainsKey(board))
            {
                if (!Attacks[board].Any(x=> x.X_Co_Ordinate == attackPosition.X_Co_Ordinate && x.Y_Co_Ordinate == attackPosition.Y_Co_Ordinate))
                {
                    Attacks[board].Add(attackPosition);
                }                
            }
            else
            {
                Attacks.Add(board, new List<Position>() { attackPosition });
            }
            var TargetShip = Ships.Where(x => x.Value.Any(x=> x.X_Co_Ordinate == attackPosition.X_Co_Ordinate 
                                            && x.Y_Co_Ordinate == attackPosition.Y_Co_Ordinate));
            
            if(TargetShip != null && TargetShip.Count() == 1)
            {  
                //Join query to find if all the cells of ship have been hit
                var TargetCells = (from targets in TargetShip.First().Value
                                  join attackCells in Attacks.First().Value
                                  on new { targets.X_Co_Ordinate, targets.Y_Co_Ordinate } equals new { attackCells.X_Co_Ordinate, attackCells.Y_Co_Ordinate }
                                  select new { targets.X_Co_Ordinate, targets.Y_Co_Ordinate }).ToList();
                if(TargetCells != null && TargetCells.Count == TargetShip.First().Value.Count)
                {
                    return AttackResult.Sunk;
                }
                else
                {
                    return AttackResult.Hit;
                }                
            }

            return AttackResult.Miss;
        }

        private bool IsPositionValid(Board board, Position startPosition, Position endPosition, Direction shipDirection)
        {
            //check if position exists on the board
            if(!board.Cells.Any(x=> x.Co_Ordinates.X_Co_Ordinate == startPosition.X_Co_Ordinate && x.Co_Ordinates.Y_Co_Ordinate == startPosition.Y_Co_Ordinate) 
                || !board.Cells.Any(x => x.Co_Ordinates.X_Co_Ordinate == endPosition.X_Co_Ordinate && x.Co_Ordinates.Y_Co_Ordinate == endPosition.Y_Co_Ordinate))
            {
                return false;
            }
            //check if the placement of ship is right
            else if(shipDirection == Direction.Vertical && startPosition.X_Co_Ordinate == endPosition.X_Co_Ordinate)
            {
                return false;
            }
            else if(shipDirection == Direction.Horizontal && startPosition.Y_Co_Ordinate == endPosition.Y_Co_Ordinate)
            {
                return false;
            }
            else
            {
                return true;
            }            
        }
    }
}
