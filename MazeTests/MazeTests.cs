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
    public class MazeTests
    {
        [TestMethod()]
        public void MazeTest()
        {
            Maze maze;
            maze = new Maze();

            bool[,] walls = new bool[1,1];
            maze = new Maze(walls);

            Assert.AreEqual(maze.Walls, walls);
        }

        [TestMethod()]
        public void IsWall()
        {
            Maze maze;
            maze = new Maze();

            bool[,] walls = new bool[1, 1];
            maze = new Maze(walls);

            Assert.AreEqual(maze.Walls, walls);

            walls = new bool[2, 2];
            walls[0, 0] = true;
            walls[0, 1] = true;
            walls[1, 0] = true;
            walls[1, 1] = true;
            maze = new Maze(walls);

            //The whole maze is accesable
            Assert.IsTrue(maze.IsWall(0, 0));
            Assert.IsTrue(maze.IsWall(0, 1));
            Assert.IsTrue(maze.IsWall(1, 0));
            Assert.IsTrue(maze.IsWall(1, 1));

            //Out of bounds checks
            Assert.IsFalse(maze.IsWall(-1, -1));
            Assert.IsFalse(maze.IsWall(2, 2));
        }
    }
}