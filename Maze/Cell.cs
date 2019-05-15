using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    public class Cell
    {
        public int fromLeft;
        public int fromTop;
        public Boolean passable;
        public Boolean visited;

        public Cell north;
        public Cell east;
        public Cell south;
        public Cell west;

        public Cell(int fromLeft, int fromTop, Boolean passable)
        {
            this.fromLeft = fromLeft;
            this.fromTop = fromTop;
            this.passable = passable;
            this.visited = false;
        }

        public Boolean hasAllBorders()
        {
            if (north.passable)
            {
                return false;
            }
            if (east.passable)
            {
                return false;
            }
            if (south.passable)
            {
                return false;
            }
            if (west.passable)
            {
                return false;
            }
            return true;
        }
    }
}
