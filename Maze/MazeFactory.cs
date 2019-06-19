using LogSystem;
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
        /// Returns a maze that is created from 4 mazes stuck together. This makes for mazes that feel more random and gives more ways to get to the same point (easyer to traverse)
        /// </summary>
        /// <param name="leftSize">The with size of the maze (where there are 4 generated of).</param>
        /// <param name="rightSize">The height size of the maze (where there are 4 generated of).</param>
        /// <returns>An int that indicates the passable walls surrounding the cell.</returns>
        public Maze GetConcatNewMaze(int leftSize = 12, int rightSize = 12)
        {
            Maze leftUpper = GetNewMaze(leftSize, rightSize);
            Maze leftLower = GetNewMaze(leftSize, rightSize);
            Maze rightUpper = GetNewMaze(leftSize, rightSize);
            Maze rightLowwer = GetNewMaze(leftSize, rightSize);

            int FullWidth = leftUpper.Walls.GetLength(0) + rightUpper.Walls.GetLength(0) - 1;
            int FullHeight = leftUpper.Walls.GetLength(1) + leftLower.Walls.GetLength(1) - 1;

            bool[,] fullWalls = new bool[FullWidth, FullHeight];

            int leftoffset = 0;
            int topoffset = 0;
            for (int fromLeft = 0; fromLeft < leftUpper.Walls.GetLength(0); fromLeft++)
            {
                for (int fromTop = 0; fromTop < leftUpper.Walls.GetLength(1); fromTop++)
                {
                    fullWalls[fromLeft + leftoffset, fromTop + topoffset] = leftUpper.Walls[fromLeft, fromTop];
                }
            }

            leftoffset = leftUpper.Walls.GetLength(0) - 1;
            topoffset = 0;
            for (int fromLeft = 0; fromLeft < rightUpper.Walls.GetLength(0); fromLeft++)
            {
                for (int fromTop = 0; fromTop < rightUpper.Walls.GetLength(1); fromTop++)
                {
                    fullWalls[fromLeft + topoffset, fromTop + leftoffset] = rightUpper.Walls[fromLeft, fromTop];
                }
            }

            leftoffset = 0;
            topoffset = leftUpper.Walls.GetLength(1) - 1;
            for (int fromLeft = 0; fromLeft < leftLower.Walls.GetLength(0); fromLeft++)
            {
                for (int fromTop = 0; fromTop < leftLower.Walls.GetLength(1); fromTop++)
                {
                    fullWalls[fromLeft + topoffset, fromTop + leftoffset] = leftLower.Walls[fromLeft, fromTop];
                }
            }

            leftoffset = leftUpper.Walls.GetLength(0) - 1;
            topoffset = leftUpper.Walls.GetLength(1) - 1;
            for (int fromLeft = 0; fromLeft < rightLowwer.Walls.GetLength(0); fromLeft++)
            {
                for (int fromTop = 0; fromTop < rightLowwer.Walls.GetLength(1); fromTop++)
                {
                    fullWalls[fromLeft + topoffset, fromTop + leftoffset] = rightLowwer.Walls[fromLeft, fromTop];
                }
            }

            // The middle point of the maze
            int middlePoint = leftUpper.Walls.GetLength(0) - 1;
            Random random = new Random();

            //For every point to the edge from the middle a random wall is removed.
            List<int> choices = new List<int>();
            for (int i = 0; i < middlePoint; i += 2)
            {
                choices.Add(i + 1);
            }
            fullWalls[middlePoint, choices[random.Next(choices.Count)]] = false;

            choices.Clear();
            for (int i = middlePoint; i < middlePoint * 2; i += 2)
            {
                choices.Add(i + 1);
            }
            fullWalls[middlePoint, choices[random.Next(choices.Count)]] = false;

            choices.Clear();
            for (int i = 0; i < middlePoint; i += 2)
            {
                choices.Add(i + 1);
            }
            fullWalls[choices[random.Next(choices.Count)], middlePoint] = false;

            choices.Clear();
            for (int i = middlePoint; i < middlePoint * 2; i += 2)
            {
                choices.Add(i + 1);
            }
            fullWalls[choices[random.Next(choices.Count)], middlePoint] = false;

            //Return the wall.
            return new Maze(fullWalls);
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
        public Maze GetNewMaze(int leftSize = 25, int rightSize = 25)
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

            Stack cellStack = new Stack();

            cellStack.Push(currentCell);
            currentCell.visited = true;

            int lastSide = 0;

            while (cellStack.Count > 0)
            {

                Cell[] neighbours = GetUnvisitedNeighbours(currentCell);

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
                    }

                    if (lastSide == side)
                    {
                        selectedNeighbour = null;
                        while (selectedNeighbour == null)
                        {
                            side = random.Next(0, 4);
                            selectedNeighbour = neighbours[side];
                        }
                    }

                    lastSide = side;

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

            //Convert the cell array to a boolean array containing the value true or false based on the passability of a cell.
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
            }

            ///To create a more random looking (but not true random feeling) maze. The amound of splits with 3 rouds are counted.
            ///If there are not more then 2 of them rebuild the maze. (This avoids mazes that feel to repeditice.)
            int countCellsWithMoreThenThreeNeighbours = 0;
            foreach (Cell cell in centreCells)
            {
                if (CountPassableWalls(cell) >= 3)
                {
                    countCellsWithMoreThenThreeNeighbours++;
                }
            }

            Maze maze = null;
            if (countCellsWithMoreThenThreeNeighbours < 4)
            {
                maze = GetNewMaze(leftSize, rightSize); //Remake the maze
            }
            else
            {
                maze = new Maze(walls); //Put the walls in the container
            }

            return maze;
        }

        /// <summary>
        /// Counts the walls that are passible around the given cell.
        /// </summary>
        /// <param name="cell">The given cell to measure from.</param>
        /// <returns>An int that indicates the passable walls surrounding the cell.</returns>
        private int CountPassableWalls(Cell cell)
        {
            int neighbours = 0;

            //northNeighbour
            if (cell.north != null && cell.north.passable == true)
            {
                neighbours++;
            }

            //eastNeighbour
            if (cell.east != null && cell.east.passable == true)
            {
                neighbours++;
            }

            //southNeighbour
            if (cell.south != null && cell.south.passable == true)
            {
                neighbours++;
            }

            //southNeighbour
            if (cell.west != null && cell.west.passable == true)
            {
                neighbours++;
            }

            return neighbours;
        }

        /// <summary>
        /// Gets an array of size 4 with the unvisited neighbours filled in and null on the other values.
        /// The first value is the north cell, second the east, third the south, and fourth the west cell.
        /// This ignores whether or not the wall between the neighbours is there or not.
        /// </summary>
        /// <param name="cell">The Cell the unvisitedneighbours you want.</param>
        /// <returns>A cell array with the unvisited neighbours filled in.</returns>
        private Cell[] GetUnvisitedNeighbours(Cell cell)
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

            return neighbours;
        }
    }
}
