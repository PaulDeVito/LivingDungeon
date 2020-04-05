using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;



[Serializable]
public class Room
{
  public int MINIMUM_ROOM_SIZE = 5;
  public int MAXIMUM_ROOM_SIZE = 10;
  public int width;
  public int height;
  public Vector3 mapCoordinates;

  public class DoorTile
  {
    public Direction exitDirection;
    public Vector3 position;

    public DoorTile (Direction exitDirection, int positionY, int positionX)
    {
      this.exitDirection = exitDirection;
      this.position = new Vector3(positionX, positionY, 0);
    }
  }

  public enum TileType
  {
    Empty,
    OuterWall,
    InnerWall,
    Door,
    Food,
    Enemy
  }

  private TileType[,] tileGrid;
  private List<DoorTile> doors;
  private List<Vector3> food;
  private List<Vector3> enemies;
  private List<Vector3> emptyPositions;

  public Room (int floor, Vector3 coordinates, int numFoodRemaining, int numEnemiesRemaining)
  {
    this.mapCoordinates = coordinates;
    this.doors = new List<DoorTile>();
    initializeTileGrid();
    initializeFood(numFoodRemaining);
    initializeEnemies(numEnemiesRemaining);
  }


  private void initializeTileGrid()
  {
    emptyPositions = new List<Vector3>();
    width = Random.Range(MINIMUM_ROOM_SIZE, MAXIMUM_ROOM_SIZE + 1);
    height = Random.Range(MINIMUM_ROOM_SIZE, MAXIMUM_ROOM_SIZE + 1);
    tileGrid = new TileType[height,width];
    // TODO: swap ALL tileGrid x and y
    for (int x = 0; x < width; x++)
    {
      for (int y = 0; y < height; y++)
      {
        if (x == 0 || x == width-1 || y == 0 || y == height-1)
        {
          tileGrid[y, x] = TileType.OuterWall;
        } else {
          tileGrid[y, x] = TileType.Empty;
          emptyPositions.Add(new Vector3(x, y, 0));
        }
      }
    }
  }

  private void initializeFood(int numFoodRemaining)
  {
    int numFood = 0;
    int foodProb = Random.Range(0, 100);
    if (foodProb > 98)
    {
      numFood = 6;
    } else if (foodProb > 97)
    {
      numFood = 5;
    } else if (foodProb > 96)
    {
      numFood = 4;
    } else if (foodProb > 94)
    {
      numFood = 3;
    } else if (foodProb > 90)
    {
      numFood = 2;
    } else if (foodProb > 70)
    {
      numFood = 1;
    }

    if (numFood > numFoodRemaining) numFood = numFoodRemaining;
    numFoodRemaining -= numFood;

    food = placeTiles(TileType.Food, numFood);
  }

  private void initializeEnemies(int numEnemiesRemaining)
  {
    int numEnemies = 0;
    int enemyProb = Random.Range(0,100);
    if (enemyProb > 98)
    {
      numEnemies = 3;
    } else if (enemyProb > 95)
    {
      numEnemies = 2;
    } else if (enemyProb > 90)
    {
      numEnemies = 1;
    }

    if (numEnemies > numEnemiesRemaining) numEnemies = numEnemiesRemaining;
    numEnemiesRemaining -= numEnemies;

    enemies = placeTiles(TileType.Enemy, numEnemies);
  }

  public Vector3 getRandomEmptyPosition()
  {
    if (emptyPositions.Count <= 0) return new Vector3(0,0,0);
    return emptyPositions[Random.Range(0, emptyPositions.Count)];
  }

  private List<Vector3> placeTiles(TileType type, int count)
  {
    List<Vector3> positions = new List<Vector3>();

    while (positions.Count < count)
    {
      if (emptyPositions.Count <= 0) break;

      Vector3 tilePosition = emptyPositions[Random.Range(0, emptyPositions.Count)];
      tileGrid[(int)tilePosition.y, (int)tilePosition.x] = type;
      positions.Add(tilePosition);
      emptyPositions.Remove(tilePosition);
    }

    return positions;
  }


  public void placeDoorTile(Direction direction)
  {
    int positionX = 0;
    int positionY = 0;

    switch(direction)
    {
      case Direction.North:
        positionY = height-1;
        positionX = Random.Range(1,width-1);
        break;
      case Direction.South:
        positionY = 0;
        positionX = Random.Range(1,width-1);
        break;
      case Direction.East:
        positionY = Random.Range(1,height-1);
        positionX = width - 1;
        break;
      case Direction.West:
        positionY = Random.Range(1,height-1);
        positionX = 0;
        break;
    }

    tileGrid[positionY, positionX] = TileType.Door;
    doors.Add(new DoorTile(direction, positionY, positionX));
  }

  public void printTileGrid()
  {
    for (int y = 0; y < height; y++)
    {
      string row = "(";
      for (int x = 0; x < width; x++)
      {
        row += tileGrid[y,x] + ", ";
      }
      row += ")";
    }
  }

  public bool hasDoorAtDirection(Direction direction)
  {
    foreach(DoorTile door in doors)
    {
      if (door.exitDirection==direction)
      {
        return true;
      }
    }

    return false;
  }



  public TileType[,] getTileGrid()
  {
    return tileGrid;
  }

  public Direction getDoorDirectionAtPosition(Vector3 position)
  {
    foreach (DoorTile door in doors)
    {
      if (door.position == position)
      {
        return door.exitDirection;
      }
    }

    return Direction.North;
  }

  public Vector3 getDoorPositionAtDirection(Direction direction)
  {
    foreach (DoorTile door in doors)
    {
      if (door.exitDirection == direction)
      {
        return door.position;
      }
    }

    return new Vector3(1,1,0);
  }

  public void removeFoodAtPosition(Vector3 position)
  {
    foreach (Vector3 foodPos in food)
    {
      if (position==foodPos)
      {
        tileGrid[(int)foodPos.y,(int)foodPos.x] = TileType.Empty;
        food.Remove(foodPos);
        return;
      }
    }
  }

  public int getHeight()
  {
    return height;
  }

  public int getWidth()
  {
    return width;
  }
}
