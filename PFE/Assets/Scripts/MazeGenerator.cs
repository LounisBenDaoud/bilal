using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{

    [Range(5, 500)]
    public int mazeWidth = 5, mazeHeight = 5;                   // The dimentions of the maze
    public int startX, startY;                                  // The position our algorithm will start from
    MazeCell[,] maze;                                           // An array of maze cells representing the maze grid

    Vector2Int currentCell;                                     // The maze cell we are currently looking at

    public MazeCell[,] GetMaze()
    {
        maze = new MazeCell[mazeWidth, mazeHeight];

        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                maze[x, y] = new MazeCell(x, y);
            }
        }

        // start carving our maze path.
        CarvePath(startX, startY);

        return maze;
    }

    List<Direction> directions = new List<Direction> {
        Direction.Up, Direction.Down, Direction.Left, Direction.Right,
    };

    List<Direction> GetRandomDirections()
    {

        // Make a copy of our direction list that we can mess around with later.
        List<Direction> dir = new List<Direction>(directions);

        // Make a direction list to put our randomized directions into.
        List<Direction> rndDir = new List<Direction>();

        while (dir.Count > 0)
        {                                            // Loop until our random direction list is empty

            int rnd = Random.Range(0, dir.Count);          // Get random index in list 
            rndDir.Add(dir[rnd]);                          // Add the random direction to our list 
            dir.RemoveAt(rnd);                             // Remove that direction so we can't choose  it again


        }

        // When we've got all four directions in a random order, return the queue
        return rndDir;
    }
    bool IsCellValid(int x, int y)
    {

        // If  the cell is outside the map or has already been visited, we  consider it not valid
        if (x < 0 || y < 0 || x > mazeWidth - 1 || y > mazeHeight - 1 || maze[x, y].visited)
            return false;
        else return true;
    }

    Vector2Int CheckNeighbour()
    {

        List<Direction> rndDir = GetRandomDirections();

        for (int i = 0; i < rndDir.Count; i++)
        {
            // Set neighbor coordinates to curreknt cell  for now
            Vector2Int neighbour = currentCell;

            // Modify neighbor coordinates based on the random directions we're currently trying 
            switch (rndDir[i])
            {
                case Direction.Up:
                    neighbour.y++;
                    break;
                case Direction.Down:
                    neighbour.y--;
                    break;
                case Direction.Right:
                    neighbour.x++;
                    break;
                case Direction.Left:
                    neighbour.x--;
                    break;
            }

            // If the neighbor we just  tried is valid, we can return that neighbor,  If not, we go again
            if (IsCellValid(neighbour.x, neighbour.y)) return neighbour;
        }
        // if we tried all directions and didin't find a valid neighbor, return the currentCell values.
        return currentCell;
    }
    void BreakWalls(Vector2Int primaryCell, Vector2Int secondaryCell)
    {
        // We can only go in one direction at a time so we can handle this using if else statements.
        if (primaryCell.x > secondaryCell.x)
        {//Primary cell's left wall .

            maze[primaryCell.x, primaryCell.y].leftWall = false;

        }
        else if (primaryCell.x < secondaryCell.x)
        {//Secondary cell's left wall

            maze[secondaryCell.x, secondaryCell.y].leftWall = false;
        }
        else if (primaryCell.y < secondaryCell.y)
        {//Primary cell's top wall .

            maze[primaryCell.x, primaryCell.y].topWall = false;

        }
        else if (primaryCell.y > secondaryCell.y)
        {//Secondary cell's top wall

            maze[secondaryCell.x, secondaryCell.y].topWall = false;
        }
    }
    //Starting at the x , y passed in , carves a path though the maze until it encounters a "dead end"
    //(a dead end is a cell with no valid neighbours).
    void CarvePath(int x, int y)
    {
        //Perform a quick check to make sure our start position is within the boundaries od the map,
        //if not , set them to a default (I'm usung 0) and throw a little warning up.
        if (x < 0 || y < 0 || x > mazeWidth - 1 || y > mazeHeight - 1)
        {
            x = y = 0;
            Debug.LogWarning("Start position is out of bounds, defaulting to 0 , 0");
        }
        //Set current cell to the starting position we were passed.
        currentCell = new Vector2Int(x, y);

        //A list to keep track of our current path.
        List<Vector2Int> path = new List<Vector2Int>();

        //loop until we encounter a dead end.
        bool deadEnd = false;
        while (!deadEnd)
        {

            //Get the next cell we're going to try.
            Vector2Int nextCell = CheckNeighbour();

            //If that cell has no valid neighbors , set deadend to true so break out of the loop.
            if (nextCell == currentCell)
            {

                for (int i = path.Count - 1; i >= 0; i--)
                {

                    currentCell = path[i];                            //set current to the next step back along our path,
                    path.RemoveAt(i);                               //Remove this step from the path.
                    nextCell = CheckNeighbour();                 //Check that cell to see if any other neighbours are valid.

                    //If we find a valid neighbour, break out of the loop.
                    if (nextCell != currentCell) break;
                }

                if (nextCell == currentCell)
                    deadEnd = true;


            }
            else
            {
                BreakWalls(currentCell, nextCell);                          //set wall flags on these two cells.
                maze[currentCell.x, currentCell.y].visited = true;       //set cell to visited before moving on.
                currentCell = nextCell;                                 //set the current cell to the valid neighbour we found.
                path.Add(currentCell);                                  //Add this cell to our path
            }
        }
    }

}


public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class MazeCell
{
    public bool visited;
    public int x, y;

    public bool topWall;
    public bool leftWall;

    // Return x and y as Vector2Int for convenience sake.

    public Vector2Int position
    {
        get
        {
            return new Vector2Int(x, y);
        }
    }

    public MazeCell(int x, int y)
    {

        // The coordinates of this cell in the maze grid.
        this.x = x;
        this.y = y;

        //Whether the algorithm has visited this cell or not - false to start
        visited = false;

        //All walls are present until the algorithm removes them from the grid
        topWall = leftWall = true;
    }
}