using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public enum Direction
{
  North,
  South,
  East,
  West
}

public class DungeonManager : MonoBehaviour
{
  public static BoardManager boardManager;
  public int numFloors = 5;
  public int currentFloor;
  public bool stairsPlaced;

  public static int mapWidth = 30;
  public static int mapHeight = 30;

  public int maxDoorsOnFloor = 20;
  public int minDoorsOnFloor = 15;
  public static int numDoorsPlaced = 0;
	public static int numOpenedDoors = 0;
	public static int numUnopenedDoors = 0;

  public int maxItemsOnFloor = 15;
  public int minItemsOnFloor = 5;
  public static int numItemsPlaced = 0;

  public int maxEnemiesOnFloor = 10;
  public int minEnemiesOnFloor = 5;
  public static int numEnemiesPlaced = 0;

  private static Room[,] roomMap;
  private Room currentRoom;
  private Direction entranceDirection;
  private Direction exitDirection;


  public void initializeDungeon()
  {
    currentFloor = numFloors - 1;
    exitDirection = Direction.North;
    entranceDirection = Direction.South;
    roomMap = new Room[mapWidth,mapHeight];
    boardManager = GetComponent<BoardManager>();
    boardManager.initialize(mapWidth, mapHeight);
    currentRoom = buildFirstRoom();
  }

  private Room buildFirstRoom()
  {
    numUnopenedDoors += 1;
    numDoorsPlaced += 1;
    int mapX = Random.Range(0,mapWidth);
    int mapY = Random.Range(0,mapHeight);
    Room newRoom =  new Room(currentFloor, new Vector3(mapX, mapY, 0));
    roomMap[mapX, mapY] = newRoom;

    Array directions = Enum.GetValues(typeof(Direction));
    newRoom.placeDoorTile((Direction)directions.GetValue(Random.Range(0,4)));
    newRoom.placeItems(determineNumItems(newRoom), new Vector3(-1,-1,-1));

    boardManager.initializeBoardFromRoom(newRoom);
    boardManager.setPlayerPosition(newRoom.getRandomEmptyPosition());

    return newRoom;
  }

  public void setupNextRoom()
  {
    Vector3 playerPosition = boardManager.getPlayerPosition();
    exitDirection = currentRoom.getDoorDirectionAtPosition(playerPosition);
    entranceDirection = reverseDirection(exitDirection);
    Vector3 exitingCoordinates = currentRoom.mapCoordinates;
    Vector3 newCoordinates = exitingCoordinates + getDirectionOffsetVector(exitDirection);

    Room roomEntered = roomMap[(int)newCoordinates.x,(int)newCoordinates.y];
    if (roomEntered == null)
    {
      roomEntered = buildRoom(currentFloor, newCoordinates, entranceDirection);
      if (shouldPlaceStairs(roomEntered))
      {
        roomEntered.placeStairs(exitDirection);
        stairsPlaced = true;
      }

      roomMap[(int)newCoordinates.x,(int)newCoordinates.y] = roomEntered;
      boardManager.initializeBoardFromRoom(roomEntered);
    }

    boardManager.changeRooms(roomEntered, currentRoom);
    Vector3 playerStartPosition = roomEntered.getDoorPositionAtDirection(entranceDirection)
                                            + getDirectionOffsetVector(exitDirection);
    boardManager.setPlayerPosition(playerStartPosition);

    currentRoom = roomEntered;
  }

  private Room buildRoom(int floor, Vector3 coordinates, Direction entranceDirection)
  {
    List<Direction> knownConnections = new List<Direction>();
    List<Direction> availableConnections = new List<Direction>();
    surveySurroundingRooms(coordinates, out knownConnections, out availableConnections);

    int minDoors = 0;
    int maxDoors = availableConnections.Count;
    if (numUnopenedDoors <= 2 && numDoorsPlaced < minDoorsOnFloor) minDoors = 1;
    int remainingDoorsAllowed = maxDoorsOnFloor - numDoorsPlaced;
    if (maxDoors > remainingDoorsAllowed) maxDoors = remainingDoorsAllowed;
    if (minDoors > maxDoors) minDoors = maxDoors;
    int numDoorsToPlace = Random.Range(minDoors, maxDoors + 1);

    numUnopenedDoors += numDoorsToPlace - knownConnections.Count;
    numDoorsPlaced += numDoorsToPlace;
    numOpenedDoors += knownConnections.Count;

    Room newRoom =  new Room(numFloors - 1, coordinates);
    while (numDoorsToPlace > 0)
    {
      int i = Random.Range(0,availableConnections.Count);
      Direction direction = availableConnections[i];
      newRoom.placeDoorTile(direction);
      availableConnections.RemoveAt(i);
      numDoorsToPlace--;
    }

    foreach (Direction direction in knownConnections)
    {
      newRoom.placeDoorTile(direction);
    }

    Vector3 playerLocation = newRoom.getDoorPositionAtDirection(entranceDirection)
                        + getDirectionOffsetVector(reverseDirection(entranceDirection));
    newRoom.placeItems(determineNumItems(newRoom), playerLocation);
    newRoom.placeEnemies(determineNumEnemies(newRoom), playerLocation);

    return newRoom;
  }

  private void surveySurroundingRooms(Vector3 mapCoordinates, out List<Direction> connectedDirs, out List<Direction> availableDirs)
  {
    connectedDirs = new List<Direction>();
    availableDirs = new List<Direction>();
    Array directions = Enum.GetValues(typeof(Direction));

    foreach (Direction direction in directions)
    {
      Vector3 coords = mapCoordinates + getDirectionOffsetVector(direction);
      if (coords.x >= mapWidth || coords.x < 0 || coords.y >= mapHeight || coords.y < 0)
      {
        continue;
      }

      Room adjacentRoom = roomMap[(int)coords.x, (int)coords.y];
      if (adjacentRoom == null)
      {
        availableDirs.Add(direction);
        continue;
      }

      if (adjacentRoom.hasDoorAtDirection(reverseDirection(direction)))
      {
        connectedDirs.Add(direction);
      }
    }
  }

  private int determineNumItems(Room room)
  {
    int score = generateItemScore(room);
    int[] values = new int[] {5, 4, 3, 2, 1};
    int[] probabilities = new int[] {97, 94, 90, 85, 65};
    int numItems = resolveGenerationProbability(values, probabilities, score);
    if (numUnopenedDoors == 0)
    {
      numItems = Math.Max(minItemsOnFloor - numItemsPlaced, 0);
    }

    if (numItemsPlaced >= maxItemsOnFloor)
    {
      numItems = 0;
    }

    return numItems;
  }

  private int generateItemScore(Room room)
  {
    int minScore = 15 * (5 - room.getNumDoors());
    int maxScore = 100 - (10 * (room.getNumDoors() - 1));
    return Random.Range(minScore, maxScore);
  }

  private int determineNumEnemies(Room room)
  {
    int score = generateEnemyScore(room);
    int[] values = new int[] {3,2,1};
    int[] probabilities = new int[] {97,70,40};
    int numEnemies = resolveGenerationProbability(values, probabilities, score);
    if (numUnopenedDoors <= 1 && numOpenedDoors >= minDoorsOnFloor / 2)
    {
      numEnemies = Math.Max(minEnemiesOnFloor - numEnemiesPlaced, 0);
    }

    if (numEnemiesPlaced >= maxEnemiesOnFloor)
    {
      numEnemies = 0;
    }

    if (room.getHeight() <= 6 || room.getWidth() <= 6)
    {
      numEnemies = 0;
    }


    return numEnemies;
  }

  private int generateEnemyScore(Room room)
  {
    int minScore = 20 * (room.getNumDoors() - 1);
    int maxScore = 60 + (10 * (room.getNumDoors()));
    return Random.Range(minScore, maxScore);
  }

  private bool shouldPlaceStairs(Room room)
  {
    if (stairsPlaced)
    {
      return false;
    }
    if (numUnopenedDoors == 0)
    {
      return true;
    }

    if (numDoorsPlaced == maxDoorsOnFloor)
    {
      int score = Random.Range(0,100);
      if (score > 70)
      {
        return true;
      }
    }

    if (numOpenedDoors >= minDoorsOnFloor)
    {
      if (room.getNumDoors() == 1)
      {
        int score = Random.Range(0,100);
        if (score > 60)
        {
          return true;
        }
      }
    }

    return false;
  }


  private int resolveGenerationProbability(int[] resultValues, int[] resultProbabilities, int generatedScore)
  {
    Debug.Assert(resultValues.Length == resultProbabilities.Length);
    for (int i = 0; i < resultValues.Length; i++)
    {
      if (generatedScore > resultProbabilities[i])
      {
        return resultValues[i];
      }
    }

    return 0;
  }

  static private Direction reverseDirection(Direction inDirection)
  {
    switch(inDirection)
    {
      case Direction.North:
        return Direction.South;
      case Direction.South:
        return Direction.North;
      case Direction.East:
        return Direction.West;
      case Direction.West:
        return Direction.East;
    }

    return Direction.North;
  }

  static private Vector3 getDirectionOffsetVector(Direction direction)
  {
    switch(direction)
    {
      case Direction.North:
        return new Vector3(0,1,0);
      case Direction.South:
        return new Vector3(0,-1,0);
      case Direction.East:
        return new Vector3(1,0,0);
      case Direction.West:
        return new Vector3(-1,0,0);
    }
    return new Vector3(0,0,0);
  }

  public Room getCurrentRoom()
  {
    return currentRoom;
  }
}
