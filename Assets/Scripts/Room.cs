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

  public Transform boardHolder;

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
    Stairs,
    Food,
    Enemy
  }

  private TileType[,] tileGrid;
  private List<DoorTile> doors;
  private List<Vector3> items;
  private List<Vector3> enemies;
  private List<Vector3> emptyPositions;

  public Room (int floor, Vector3 coordinates)
  {
    this.mapCoordinates = coordinates;
    this.doors = new List<DoorTile>();
    initializeTileGrid();
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

  public void placeItems(int numItems, Vector3 playerPosition)
  {
    items = placeTiles(TileType.Food, numItems, playerPosition);
  }

  public void placeEnemies(int numEnemies, Vector3 playerPosition)
  {
    enemies = placeTiles(TileType.Enemy, numEnemies, playerPosition);
  }


  public Vector3 getRandomEmptyPosition()
  {
    if (emptyPositions.Count <= 0) return new Vector3(0,0,0);
    return emptyPositions[Random.Range(0, emptyPositions.Count)];
  }


  private List<Vector3> placeTiles(TileType type, int count, Vector3 playerPosition)
  {
    List<Vector3> positions = new List<Vector3>();

    while (positions.Count < count)
    {
      if (emptyPositions.Count <= 0) break;

      Vector3 tilePosition = emptyPositions[Random.Range(0, emptyPositions.Count)];
      if (tilePosition == playerPosition)
      {
        continue;
      }

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

  public void placeStairs(Direction direction)
  {
    int positionX = 0;
    int positionY = 0;

    switch(direction)
    {
      case Direction.North:
      positionY = height-2;
      positionX = Random.Range(2,width-2);
      break;
      case Direction.South:
      positionY = 1;
      positionX = Random.Range(2,width-2);
      break;
      case Direction.East:
      positionY = Random.Range(2,height-2);
      positionX = width - 2;
      break;
      case Direction.West:
      positionY = Random.Range(2,height-2);
      positionX = 1;
      break;
    }

    tileGrid[positionY, positionX] = TileType.Stairs;
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
    foreach (Vector3 itemPos in items)
    {
      if (position==itemPos)
      {
        tileGrid[(int)itemPos.y,(int)itemPos.x] = TileType.Empty;
        items.Remove(itemPos);
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

  public int getNumDoors()
  {
    return doors.Count;
  }

  public int getXCoordinate()
  {
    return (int)mapCoordinates.x;
  }

  public int getYCoordinate()
  {
    return (int)mapCoordinates.y;
  }
}
