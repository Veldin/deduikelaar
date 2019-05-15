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

        public static int tileSize;

        static MazeFacade()
        {
            tileSize = 250;
        }

        public static bool Init()
        {
            mazeFactory = new MazeFactory();
            maze = mazeFactory.GetNewMaze();

            return true;
        }

        public static void NewMaze()
        {
            maze = mazeFactory.GetNewMaze();
        }

        public static int GetMazeWidth()
        {
            return maze.Walls.GetLength(0);
        }

        public static int GetMazeHeight()
        {
            return maze.Walls.GetLength(1);
        }

        public static Boolean isWall(int fromLeft, int fromTop)
        {
            return maze.isWall(fromLeft, fromTop);
        }
    }
}
