using NUnit.Framework;
using BattleshipTracker.Services;
using BattleshipTracker.Model;
using System.Collections.Generic;


namespace BattleshipTracker.Tests
{
    public class BattleShipTrackerServiceTest
    {
        private BattleShipTrackerService _battleShipTrackerService;
        
        [SetUp]
        public void Setup()
        {
            _battleShipTrackerService = new BattleShipTrackerService();
        }

        [Test]
        [TestCase(1,10,1)]
        [TestCase(2, 2, 2)]
        public void CreateBoard_Test(int numberOfPlayers, int boardSize, int resultCount)
        {
            var result = (List<Board>)_battleShipTrackerService.CreateBoard(numberOfPlayers, boardSize);
            Assert.IsTrue(result.Count == resultCount);
        }

        [Test]
        public void CheckBoardCells_Test()
        {
            var result = (List<Board>)_battleShipTrackerService.CreateBoard(1, 10);
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0]?.Cells?.Count == 100);
        }

        [Test]        
        public void CreateBoard_NegativeTest()
        {
            var result = _battleShipTrackerService.CreateBoard(0, 0);
            Assert.IsNull(result);
        }

        [Test]
        public void AddShip_Test()
        {
            var board = new Board("TestBoard", 10);
            var result = _battleShipTrackerService.AddShip("Test", board, new Position(2, 3), new Position(2, 6), Direction.Horizontal);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ShipName == "Test");
            Assert.IsTrue(_battleShipTrackerService.Ships.ContainsKey("Test"));
        }


        [Test]
        public void AddShip_NegativeTest()
        {          
            var board = new Board("TestBoard", 10);
            var error = Assert.Throws<System.ApplicationException>(() => _battleShipTrackerService.AddShip("Test", board, new Position(2, 3), new Position(2, 6), Direction.Vertical));
            Assert.IsNotNull(error);
            Assert.IsFalse(string.IsNullOrEmpty(error.Message));

        }

        [Test]
        public void AddShip_NullTest()
        {
            var error = Assert.Throws<System.ApplicationException>(() => _battleShipTrackerService.AddShip(null, null, null, null, Direction.Vertical));
            Assert.IsNotNull(error);
            Assert.IsFalse(string.IsNullOrEmpty(error.Message));
        }

        [Test]
        public void Attack_NullTest()
        {
            var error = Assert.Throws<System.ApplicationException>(() => _battleShipTrackerService.Attack(null, null));
            Assert.IsNotNull(error);
            Assert.IsFalse(string.IsNullOrEmpty(error.Message));
        }

        [Test]
        public void Attack_Test()
        {
            var board = new Board("AttackBoard", 10);
            _battleShipTrackerService.AddShip("Test", board, new Position(2, 3), new Position(2, 6), Direction.Horizontal);

            var result = _battleShipTrackerService.Attack(board, new Position(1, 2));
            Assert.IsTrue(result == AttackResult.Miss);

            result = _battleShipTrackerService.Attack(board, new Position(2, 3));
            Assert.IsTrue(result == AttackResult.Hit);

            result = _battleShipTrackerService.Attack(board, new Position(2, 4));
            Assert.IsTrue(result == AttackResult.Hit);

            result = _battleShipTrackerService.Attack(board, new Position(2, 5));
            Assert.IsTrue(result == AttackResult.Hit);

            result = _battleShipTrackerService.Attack(board, new Position(2, 6));
            Assert.IsTrue(result == AttackResult.Sunk);
        }
    }
}