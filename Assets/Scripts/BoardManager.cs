using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
	//reference to the transform of our Board object.
	private Transform boardHolder;
	public GameObject exit;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;
	public static GameObject player;
	private DungeonManager dungeonManager;


	void initializeBoardFromRoom(Room room)
	{
		boardHolder = new GameObject().transform;
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
					case Room.TileType.Door:
						boardTile = exit;
						break;
					case Room.TileType.OuterWall:
						boardTile = outerWallTiles [Random.Range(0, outerWallTiles.Length)];
						break;
					case Room.TileType.InnerWall:
						boardTile = wallTiles [Random.Range(0, wallTiles.Length)];
						break;
					case Room.TileType.Food:
						layFloorTile(x, y);
						boardTile = foodTiles [Random.Range(0, foodTiles.Length)];
						break;
					case Room.TileType.Enemy:
						layFloorTile(x, y);
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
	}

	void layFloorTile(int x, int y)
	{
		GameObject boardTile = floorTiles[Random.Range(0,floorTiles.Length)];
		GameObject boardTileInstance =
			Instantiate (boardTile, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
		boardTileInstance.transform.SetParent (boardHolder);
	}


	public void initializeDungeon()
	{
		dungeonManager = new DungeonManager(5);
		Room currentRoom = dungeonManager.getCurrentRoom();
		player = GameObject.FindGameObjectWithTag("Player");
		player.transform.position = currentRoom.getRandomEmptyPosition();
		initializeBoardFromRoom(currentRoom);
	}


	public void changeRooms()
	{
		foreach (Transform child in boardHolder) {
     		GameObject.Destroy(child.gameObject);
 		}

		dungeonManager.setupNextRoom(player.transform.position);
		player.transform.position = dungeonManager.getPlayerStartPosition();

		initializeBoardFromRoom(dungeonManager.getCurrentRoom());
	}


	public DungeonManager getDungeonManager()
	{
		return dungeonManager;
	}
}
