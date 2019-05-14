using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    public class MazeFactory
    {
        public MazeFactory()
        {
            Console.WriteLine("MazeFactory");

            bool[,] walls = new bool[26, 26];
            Stack cellStack = new Stack();

            //int[int[,]] cells = new int[1];

            //Set all the walls to true
            for (int x = 0; x < walls.GetLength(0); x++)
            {
                for (int y = 0; y < walls.GetLength(1); y++)
                {
                    walls[x, y] = true; //set all cells to true so they act like a wall

                    //Get the cels that have uneven x and y to be sure they share borders to remove (to create a maze)
                    if (x %2 != 0 && y %2 != 0)
                    {
                    
                    }
                }
            }
        }
    }
}
