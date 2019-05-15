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

        static MazeFacade()
        {
            
        }

        public static bool init()
        {
            mazeFactory = new MazeFactory();
            maze = mazeFactory.GetNewMaze();

            return true;
        }

        public static void NewMaze()
        {
            maze = mazeFactory.GetNewMaze();
        }
    }
}
