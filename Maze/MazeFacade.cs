using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    public static class MazeFacade
    {
        private static Maze maze;
        private static MazeFactory mazeFactory;

        public static int tileSize = 80;

        /// <summary>
        /// Sets the mazeFactory variable and creates a new maze using the factory.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public static bool Init()
        {
            mazeFactory = new MazeFactory();
            maze = mazeFactory.GetConcatNewMaze();

            return true;
        }

        /// <summary>
        /// Creates a new maze using the factory and sets the maze variable on the created maze.
        /// </summary>
        public static void NewMaze()
        {
            maze = mazeFactory.GetNewMaze();
        }

        public static Maze GetMaze()
        {
            return maze;
        }

        /// <summary>
        /// Returns the width of the maze using the GetLength method on the walls of the maze.
        /// </summary>
        /// <returns>Returns the width of the maze.</returns>
        public static int GetMazeWidth()
        {
            return maze.Walls.GetLength(0);
        }

        /// <summary>
        /// Returns the height of the maze using the GetLength method on the walls of the maze.
        /// </summary>
        /// <returns>Returns the height of the maze.</returns>
        public static int GetMazeHeight()
        {
            return maze.Walls.GetLength(1);
        }

        /// <summary>
        /// Calls the IsWall method of the maze at given location.
        /// </summary>
        /// <param name="fromLeft">The x coordinate to check</param>
        /// <param name="fromTop">The y coordinate to check</param>
        /// <returns>Returns the value of IsWall of the maze.</returns>
        public static Boolean IsWall(int fromLeft, int fromTop)
        {
            return maze.IsWall(fromLeft, fromTop);
        }
    }
}
