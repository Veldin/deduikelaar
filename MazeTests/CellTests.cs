using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Tests
{
    [TestClass()]
    public class CellTests
    {
        [TestMethod()]
        public void CellTest()
        {
            Cell cell;

            cell = new Cell(20,10,true);

            Assert.AreEqual(cell.fromLeft, 20);
            Assert.AreEqual(cell.fromTop, 10);

            Assert.IsTrue(cell.passable);
            Assert.IsFalse(cell.visited);

            cell = new Cell(-400, -500, false);

            Assert.AreEqual(cell.fromLeft, -400);
            Assert.AreEqual(cell.fromTop, -500);

            Assert.IsFalse(cell.passable);
            Assert.IsFalse(cell.visited);
        }

        [TestMethod()]
        public void hasAllBordersTest()
        {
            Cell cell;
            
            //No borders set
            cell = new Cell(0, 0, false);
            Assert.IsFalse(cell.hasAllBorders());

            //All cells surrounding are not passable
            cell = new Cell(0, 0, false);
            cell.north = new Cell(0, 0, false);
            cell.east = new Cell(0, 0, false);
            cell.south = new Cell(0, 0, false);
            cell.west = new Cell(0, 0, false);
            Assert.IsTrue(cell.hasAllBorders());

            //cells surrounding are not all passable
            cell = new Cell(0, 0, false);
            cell.north = new Cell(0, 0, true);
            cell.east = new Cell(0, 0, false);
            cell.south = new Cell(0, 0, false);
            cell.west = new Cell(0, 0, false);
            Assert.IsFalse(cell.hasAllBorders());

            //cells surrounding are not all passable
            cell = new Cell(0, 0, false);
            cell.north = new Cell(0, 0, false);
            cell.east = new Cell(0, 0, true);
            cell.south = new Cell(0, 0, false);
            cell.west = new Cell(0, 0, false);
            Assert.IsFalse(cell.hasAllBorders());

            //cells surrounding are not all passable
            cell = new Cell(0, 0, false);
            cell.north = new Cell(0, 0, false);
            cell.east = new Cell(0, 0, false);
            cell.south = new Cell(0, 0, true);
            cell.west = new Cell(0, 0, false);
            Assert.IsFalse(cell.hasAllBorders());

            //cells surrounding are not all passable
            cell = new Cell(0, 0, false);
            cell.north = new Cell(0, 0, false);
            cell.east = new Cell(0, 0, false);
            cell.south = new Cell(0, 0, false);
            cell.west = new Cell(0, 0, true);
            Assert.IsFalse(cell.hasAllBorders());
        }
    }
}