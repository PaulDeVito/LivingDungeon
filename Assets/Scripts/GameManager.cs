using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	public static DungeonManager dungeonManager;
	public static float baseEnemySpeed = 0.1f;


	public float levelStartDelay = 2f;
	public int playerFoodPoints = 100;

  	private static Text levelText;
  	private static GameObject levelImage;
	private static List<Enemy> enemies;
	private static List<Enemy> activeEnemies;
	private static List<Enemy> inactiveEnemies;
	private bool enemiesMoving;
  	private bool doingSetup;

  	void Awake()
  	{
  		if (instance == null)
  			instance = this;
  		else if (instance != this)
  			Destroy(gameObject);

    	Debug.Log("Game Start");
  		DontDestroyOnLoad(gameObject);

  		enemies = new List<Enemy>();
  		activeEnemies = new List<Enemy>();
  		inactiveEnemies = new List<Enemy>();
		dungeonManager = GetComponent<DungeonManager>();
    	initGame();
  }

  	public static void triggerRoomChange()
  	{
  	    dungeonManager.setupNextRoom();
		categorizeEnemies();
  	}

	private static void categorizeEnemies()
	{
		Debug.Log("Total enemies: " + enemies.Count);
		activeEnemies.Clear();
		inactiveEnemies.Clear();
		foreach(Enemy enemy in enemies)
		{

			if (enemy.gameObject.transform.parent.gameObject.activeSelf)
			{
				activeEnemies.Add(enemy);
			} else {
				inactiveEnemies.Add(enemy);
			}
		}
		Debug.Log("Total active enemies: " + activeEnemies.Count);
		Debug.Log("Total inactive enemies: " + inactiveEnemies.Count);
	}

  void initGame()
  {
    doingSetup = true;
    levelImage = GameObject.Find("LevelImage");
    levelImage.SetActive(true);
    levelText = GameObject.Find("LevelText").GetComponent<Text>();
    Invoke("hideLevelImage", levelStartDelay);
  	enemies.Clear();
		Debug.Log("calling initialize dungeon manager");
		dungeonManager.initializeDungeon();
  }

  private void hideLevelImage()
  {
      levelImage.SetActive(false);
      doingSetup = false;
  }

  public void triggerGameOver()
  {
		Invoke("gameOver", 0.1f);
  }

	private void gameOver()
	{
		levelText.text = "YOU DIED";
    levelImage.SetActive(true);
  	enabled = false;
	}

	public void triggerWinGame()
	{
		Debug.Log("Won the game");
		Invoke("winGame", 0.1f);
	}

	private void winGame()
	{
		levelText.text = "SWEET WIIIIINNAMONNNN";
    levelImage.SetActive(true);
  	enabled = false;
	}

  void Update()
  {
  }


  public void addEnemyToList(Enemy script)
  {
  	enemies.Add(script);
  }


	public static DungeonManager getDungeonManager()
	{
		return dungeonManager;
	}
}
