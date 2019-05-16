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

        public Boolean IsWall(int fromLeft, int fromTop)
        {
            if (Walls is null)
            {
                return false;
            }

            //Check out of bounds
            if (fromLeft > walls.GetLength(0) - 1 || fromLeft < 0)
            {
                return false;
            }

            if (fromTop > walls.GetLength(1) - 1 || fromTop < 0)
            {
                return false;
            }

            return walls[fromLeft, fromTop];
        }
    }
}
