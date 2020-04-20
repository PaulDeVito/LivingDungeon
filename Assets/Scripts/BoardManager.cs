using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
	public GameObject door;
	public GameObject stairs;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;
	public static GameObject player;

	private static Transform[,] boardMap;
	private static List<Transform> allBoards;
	private static int numBoards;


	public void initialize(int mapWidth, int mapHeight)
	{
		player = GameObject.FindGameObjectWithTag("Player");
		boardMap = new Transform[mapWidth, mapHeight];
		allBoards = new List<Transform>();
	}

	public void initializeBoardFromRoom(Room room)
	{
		numBoards++;
		GameObject roomObject = new GameObject();
		roomObject.name = "Room" + numBoards;
		Transform boardHolder = roomObject.transform;
		boardMap[room.getXCoordinate(), room.getYCoordinate()] = boardHolder;
		Room.TileType[,] grid = room.getTileGrid();
		for (int x = 0; x < room.width; x++)
		{
			for (int y = 0; y < room.height; y++)
			{

				GameObject boardTile;
				switch(grid[y,x])
				{
					case Room.TileType.Empty:
						boardTile = floorTiles[Random.Range(0,floorTiles.Length)];
						break;
					case Room.TileType.Stairs:
						layFloorTile(boardHolder, x, y);
						boardTile = stairs;
						break;
					case Room.TileType.Door:
						boardTile = door;
						break;
					case Room.TileType.OuterWall:
						boardTile = outerWallTiles [Random.Range(0, outerWallTiles.Length)];
						break;
					case Room.TileType.InnerWall:
						boardTile = wallTiles [Random.Range(0, wallTiles.Length)];
						break;
					case Room.TileType.Food:
						layFloorTile(boardHolder, x, y);
						boardTile = foodTiles [Random.Range(0, foodTiles.Length)];
						break;
					case Room.TileType.Enemy:
						layFloorTile(boardHolder, x, y);
						boardTile = enemyTiles [Random.Range(0, enemyTiles.Length)];
						break;
					default:
						boardTile = floorTiles[Random.Range(0,floorTiles.Length)];
						break;
				}

				GameObject boardTileInstance =
					Instantiate (boardTile, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
				boardTileInstance.transform.SetParent (boardHolder);
			}
		}

		room.boardHolder = boardHolder;

		incrementAllBoardDepths();
		allBoards.Add(boardHolder);
		EventManager.TriggerBuildRoomEvent(room);
	}

	private void incrementAllBoardDepths()
	{
		foreach (Transform board in allBoards)
		{
			float newZ = board.position.z + 1;
			board.position = new Vector3(0, 0, newZ);
		}
	}

	void layFloorTile(Transform board, int x, int y)
	{
		GameObject boardTile = floorTiles[Random.Range(0,floorTiles.Length)];
		GameObject boardTileInstance =
			Instantiate (boardTile, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
		boardTileInstance.transform.SetParent (board);
	}

	public Transform getBoardFromRoom(Room room)
	{
		return boardMap[room.getXCoordinate(), room.getYCoordinate()];
	}

	public void changeRooms(Room newRoom)
	{
		incrementAllBoardDepths();
		getBoardFromRoom(newRoom).position = new Vector3(0, 0, 0);
	}

	public void setPlayerPosition(Vector3 position)
	{
		player.transform.position = position;
	}

	public Vector3 getPlayerPosition()
	{
		return player.transform.position;
	}
}
