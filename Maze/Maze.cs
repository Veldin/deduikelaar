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

        /// <summary>
        /// Check if a coordinate on in the wall grid is true or fasle.
        /// </summary>
        /// <param name="fromLeft">The x coordinate</param>
        /// <param name="fromTop">The y coordinate</param>
        /// <returns>Returns true if the coordinate in the wall grid is true. Returns false otherwise.
        ///  If the requested coordinate is not within the grid or the walls grid is none it returns false.</returns>
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
