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

public class DungeonManager
{
  public int numFloors = 5;
  public int currentFloor;

  public static int mapWidth = 30;
  public static int mapHeight = 30;

  public static int numDoorsRemaining = 30;
	public static int numOpenedDoors = 0;
	public static int numUnopenedDoors = 0;
	public static int numFoodRemaining = 10;
	public static int numEnemiesRemaining = 0;

  private static Room[,] roomMap;
  private Room currentRoom;
  private Direction entranceDirection;
  private Direction exitDirection;

  public DungeonManager (int numFloors)
  {
    numFloors = numFloors;
    currentFloor = numFloors - 1;
    entranceDirection = Direction.South;
    exitDirection = Direction.North;
    roomMap = new Room[mapWidth,mapHeight];
    int mapX = Random.Range(0,mapWidth);
    int mapY = Random.Range(0,mapHeight);
    Vector3 mapCoordinates = new Vector3(mapX, mapY, 0);
    currentRoom = buildRoom(currentFloor, mapCoordinates);
    roomMap[mapX, mapY] = currentRoom;
  }

  private Room buildRoom(int floor, Vector3 coordinates)
  {
    Room newRoom =  new Room(numFloors - 1, coordinates, numFoodRemaining, numEnemiesRemaining);
    List<Direction> knownConnections = new List<Direction>();
    List<Direction> availableConnections = new List<Direction>();
    surveySurroundingRooms(coordinates, out knownConnections, out availableConnections);

    int minDoors = 0;
    int maxDoors = availableConnections.Count;
    if (numUnopenedDoors <= 2) minDoors = 1;
    if (maxDoors > numDoorsRemaining) maxDoors = numDoorsRemaining;
    if (minDoors > maxDoors) minDoors = maxDoors;
    int numDoorsToPlace = Random.Range(minDoors, maxDoors + 1);

    numUnopenedDoors += numDoorsToPlace - knownConnections.Count;
    numDoorsRemaining -= numDoorsToPlace;
    numOpenedDoors += knownConnections.Count;

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

  public void setupNextRoom(Vector3 playerPosition)
  {
    exitDirection = currentRoom.getDoorDirectionAtPosition(playerPosition);
		entranceDirection = reverseDirection(exitDirection);
    Vector3 exitingCoordinates = currentRoom.mapCoordinates;
    Vector3 newCoordinates = exitingCoordinates + getDirectionOffsetVector(exitDirection);

    Room roomEntered = roomMap[(int)newCoordinates.x,(int)newCoordinates.y];
    if (roomEntered == null)
		{
			roomEntered = buildRoom(currentFloor, newCoordinates);
			roomMap[(int)newCoordinates.x,(int)newCoordinates.y] = roomEntered;
		}

    currentRoom = roomEntered;
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

  public Vector3 getPlayerStartPosition()
  {
    return currentRoom.getDoorPositionAtDirection(entranceDirection) + getDirectionOffsetVector(exitDirection);
  }

  public Room getCurrentRoom()
  {
    return currentRoom;
  }
}
