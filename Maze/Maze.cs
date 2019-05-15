using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    public class Maze
    {
        private bool[,] walls;
        private String sprite;

        public Maze()
        {

        }

        public Maze(bool[,] given)
        {
            walls = given;
        }

        public bool[,] Walls
        {
            get { return walls; }
            set { walls = value; }
        }

        public Boolean isWall(int fromLeft, int fromTop)
        {
            Debug.WriteLine(fromLeft + "| |" + fromTop);

            try{
                return walls[fromLeft, fromTop];

            }
            catch
            {
                return false;
            }
        }

    }
}
