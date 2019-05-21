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
    public class MazeFactoryTests
    {
        [TestMethod()]
        public void MazeFactoryTest()
        {
            MazeFactory mazeFactory = new MazeFactory();
            Maze maze;

            maze = mazeFactory.GetNewMaze();
            Assert.AreEqual(maze.Walls.GetLength(0), 25);
            Assert.AreEqual(maze.Walls.GetLength(1), 25);
        }

        [TestMethod()]
        public void MazeFactoryNegativeTest()
        {
            MazeFactory mazeFactory = new MazeFactory();
            Maze maze;

            maze = mazeFactory.GetNewMaze(-3,-3);
            Assert.AreEqual(maze.Walls.GetLength(0), 3);
            Assert.AreEqual(maze.Walls.GetLength(1), 3);
        }

        [TestMethod()]
        public void MazeFactoryEvenTest()
        {
            MazeFactory mazeFactory = new MazeFactory();
            Maze maze;

            maze = mazeFactory.GetNewMaze(4, 4);
            Assert.AreEqual(maze.Walls.GetLength(0), 5);
            Assert.AreEqual(maze.Walls.GetLength(1), 5);
        }

        [TestMethod()]
        public void MazeFactoryLoopTest()
        {
            MazeFactory mazeFactory = new MazeFactory();
            Maze maze;

            //create a cell on everfromTop place in the double arrafromTop
            for (int fromLeft = 0; fromLeft < 10; fromLeft++)
            {
                for (int fromTop = 0; fromTop < 10; fromTop++)
                {
                    maze = null;
                    maze = mazeFactory.GetNewMaze(fromLeft, fromTop);
                    Assert.IsTrue(maze is Maze);
                }
            }
        }
    }
}