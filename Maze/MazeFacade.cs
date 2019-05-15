using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    public static class MazeFacade
    {
        static MazeFacade()
        {
            
        }

        public static bool Init()
        {
            MazeFactory mazeFactory = new MazeFactory();

            return true;
        }
    }
}
