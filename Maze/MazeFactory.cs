using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Maze
{
    public class MazeFactory
    {
        public Cell[,] cells;

        public MazeFactory()
        {

        }

        /// <summary>
        /// Creates a new maze using the "Randomized Depth-First Search" algorithm. Smalles size maze is 3 by 3. If a even number is given its made odd (by adding 1).
        /// If a maze smaller then 3x3 is requested the maze wil be generated 3x3 anyway.
        /// </summary>
        /// <remarks>
        /// The randomized depth-first search algorithm of maze generation implemented using a stack:
        ///     1. Start with a grid that has no edges(all walls)
        ///     2. Make the initial cell the current cell
        ///     3. Push the current cell to the stack and mark it as visited
        ///     4. While the stack is not empty
        ///        1. If the current cell has unvisited neighbours
        ///            1. Choose one of these neighbours randomly
        ///            2. Remove the wall(add an edge) between the current cell and the chosen neighbor
        ///            3. Push the neighbor to the stack and mark is as visited
        ///            4. Make the neighbor the current cell
        ///        2. Else
        ///            1. Pop a cell from the stack and make it the current cell
        /// </remarks>
        /// <param name="leftSize">The width of the maze.</param>
        /// <param name="rightSize">The height of the maze.</param>
        /// <returns>The created Maze</returns>
        public Maze GetNewMaze(int leftSize = 9, int rightSize = 9)
        {
            //Deu to cells needing to be able to have walls the maze sizes need to be odd
            //and the minimum amount of cells is 3 
            if (leftSize < 3)
            {
                leftSize = 3;
            }

            if (rightSize < 3)
            {
                rightSize = 3;
            }

            if (leftSize % 2 == 0)
            {
                leftSize++;
            }

            if (rightSize % 2 == 0)
            {
                rightSize++;
            }

            //Creates a cells array with the same size as the eventual maze.
            cells = new Cell[leftSize, rightSize];
            //The walls of the grid that is eventualy returned
            bool[,] walls = new bool[leftSize, rightSize];

            //The cells in the centre. Thease are anchor points where walls are between. 
            List<Cell> centreCells = new List<Cell>();

            //int[int[,]] cells = new int[1];

            //create a cell on everfromTop place in the double arrafromTop
            for (int fromLeft = 0; fromLeft < cells.GetLength(0); fromLeft++)
            {
                for (int fromTop = 0; fromTop < cells.GetLength(1); fromTop++)
                {
                    Cell needle = new Cell(fromLeft, fromTop, false);
                    cells[fromLeft, fromTop] = needle;

                    //Get the cels that have uneven fromLeft and fromTop to be sure thefromTop share borders to remove (to create a maze)
                    if (fromLeft % 2 != 0 && fromTop % 2 != 0)
                    {
                        centreCells.Add(needle);
                        needle.passable = true;
                    }
                }
            }

            //Set the walls reference of the centre cells.
            foreach (Cell cell in centreCells)
            {
                cell.north = cells[cell.fromLeft, cell.fromTop - 1];
                cell.east = cells[cell.fromLeft + 1, cell.fromTop];
                cell.south = cells[cell.fromLeft, cell.fromTop + 1];
                cell.west = cells[cell.fromLeft - 1, cell.fromTop];
            }

            /* All cells are created 
                //Creation of a maze can begin
             */

            Cell currentCell = centreCells[0];
            int index = 0;

            Stack cellStack = new Stack();

            cellStack.Push(currentCell);
            currentCell.visited = true;

            while (cellStack.Count > 0)
            {
                //Debug.WriteLine(cellStack.Count);

                Cell[] neighbours = getUnvisitedNeighbours(currentCell);

                Random random = new Random();

                Cell selectedNeighbour = null;

                //Check if all the nodes are null
                if (neighbours[0] == null && neighbours[1] == null && neighbours[2] == null && neighbours[3] == null)
                {
                    //Pop a cell from the stack and make it the current cell
                    currentCell = cellStack.Pop() as Cell;
                }
                else
                {
                    int side = 0;

                    while (selectedNeighbour == null)
                    {
                        side = random.Next(0, 4);
                        selectedNeighbour = neighbours[side];

                        Debug.WriteLine(side);
                    }

                    switch (side)
                    {
                        case 0: //north
                            currentCell.north.passable = true;
                            break;
                        case 1: //east
                            currentCell.east.passable = true;
                            break;
                        case 2: //south
                            currentCell.south.passable = true;
                            break;
                        default: //west
                            currentCell.west.passable = true;
                            break;
                    }

                    //Push the neighbor to the stack and mark is as visited

                    cellStack.Push(selectedNeighbour);
                    selectedNeighbour.visited = true;
                    currentCell = selectedNeighbour;
                }




            }

            /*while (cellStack.Count > 0)
            {
                currentCell = cellStack.Pop() as Cell;
            }*/



            for (int fromLeft = 0; fromLeft < cells.GetLength(0); fromLeft++)
            {
                for (int fromTop = 0; fromTop < cells.GetLength(1); fromTop++)
                {
                    Cell testDraw = cells[fromLeft, fromTop];

                    if (testDraw.passable == true)
                    {
                        walls[fromLeft, fromTop] = false;
                    }
                    else
                    {
                        walls[fromLeft, fromTop] = true;
                    }

                }
                Debug.WriteLine("");

            }
            Debug.WriteLine("_____________");

            Maze maze = new Maze(walls);
            return maze;
        }

        private Cell[] getUnvisitedNeighbours(Cell cell)
        {
            Cell[] neighbours = new Cell[4];


            Cell northNeighbour = null;
            if (cell.fromTop - 2 > 0)
            {
                northNeighbour = cells[cell.fromLeft, cell.fromTop - 2];
                if (!northNeighbour.visited)
                {
                    neighbours[0] = northNeighbour;
                }
            }

            Cell eastNeighbour = null;
            if (cell.fromLeft + 2 < cells.GetLength(0))
            {
                eastNeighbour = cells[cell.fromLeft + 2, cell.fromTop];
                if (!eastNeighbour.visited)
                {
                    neighbours[1] = eastNeighbour;
                }
            }

            Cell southNeighbour = null;
            if (cell.fromTop + 2 < cells.GetLength(1))
            {
                southNeighbour = cells[cell.fromLeft, cell.fromTop + 2];
                if (!southNeighbour.visited)
                {
                    neighbours[2] = southNeighbour;
                }
            }

            Cell westNeighbour = null;
            if (cell.fromLeft - 2 > 0)
            {
                westNeighbour = cells[cell.fromLeft -2, cell.fromTop];
                if (!westNeighbour.visited)
                {
                    neighbours[3] = westNeighbour;
                }
            }

            //neighbours[0] = northNeighbour;
            //neighbours[1] = eastNeighbour;
            //neighbours[2] = southNeighbour;
            //neighbours[3] = westNeighbour;

            return neighbours;
        }
    }
}
