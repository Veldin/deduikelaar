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
    public class MazeFacadeTests
    {
        [TestMethod()]
        public void MazeFacadeInitGetMazeTest()
        {
            MazeFacade.Init();
            Maze maze = MazeFacade.GetMaze();

            Assert.IsTrue(maze is Maze);
        }

        [TestMethod()]
        public void MazeFacadeNewMaze()
        {
            MazeFacade.Init();
            Maze maze = MazeFacade.GetMaze();
            MazeFacade.NewMaze();
            Assert.IsFalse(maze == MazeFacade.GetMaze());
        }

        [TestMethod()]
        public void MazeFacadeGetMazeWidth()
        {
            MazeFacade.Init();
            Assert.IsTrue(MazeFacade.GetMaze().Walls.GetLength(0) == MazeFacade.GetMazeWidth());
        }

        [TestMethod()]
        public void MazeFacadeGetMazeHeight()
        {
            MazeFacade.Init();
            Assert.IsTrue(MazeFacade.GetMaze().Walls.GetLength(1) == MazeFacade.GetMazeHeight());
        }

        [TestMethod()]
        public void MazeFacadeIsWall()
        {
            Maze maze = MazeFacade.GetMaze();

            //create a cell on everfromTop place in the double arrafromTop
            for (int fromLeft = 0; fromLeft < MazeFacade.GetMazeWidth(); fromLeft++)
            {
                for (int fromTop = 0; fromTop < MazeFacade.GetMazeHeight(); fromTop++)
                {
                    Assert.IsTrue(
                        MazeFacade.IsWall(fromLeft, fromTop) == maze.IsWall(fromLeft, fromTop)
                    );
                }
            }

        }
    }
}