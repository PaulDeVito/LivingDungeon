using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	public static DungeonManager dungeonManager;
	public float turnDelay = 0.1f;


	public float levelStartDelay = 2f;
	public int playerFoodPoints = 100;

  private static Text levelText;
  private static GameObject levelImage;
	private static List<Enemy> enemies;
	private bool enemiesMoving;
  private bool doingSetup;

  void Awake()
  {
  	if (instance == null)
  		instance = this;
  	else if (instance != this)
  		Destroy(gameObject);

    Debug.Log("Game Manager Awake");
  	DontDestroyOnLoad(gameObject);

  	enemies = new List<Enemy>();
		dungeonManager = GetComponent<DungeonManager>();
    initGame();
  }

  public static void triggerRoomChange()
  {
      enemies.Clear();
      dungeonManager.setupNextRoom();
  }

  void initGame()
  {
    doingSetup = true;
    levelImage = GameObject.Find("LevelImage");
    levelImage.SetActive(true);
    levelText = GameObject.Find("LevelText").GetComponent<Text>();
    Invoke("hideLevelImage", levelStartDelay);
  	enemies.Clear();
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
		if(enemiesMoving)
    	return;

    StartCoroutine(moveEnemies());
  }

  public void addEnemyToList(Enemy script)
  {
  	enemies.Add(script);
  }

  IEnumerator moveEnemies()
  {
  	enemiesMoving = true;
  	yield return new WaitForSeconds(turnDelay);
  	if (enemies.Count == 0)
  	{
  		yield return new WaitForSeconds(turnDelay);
  	}

  	for (int i = 0; i < enemies.Count; i++)
  	{
  		enemies[i].moveEnemy();
  		yield return new WaitForSeconds(enemies[i].moveAnimationTime);
  	}

  	enemiesMoving = false;
  }

	public static DungeonManager getDungeonManager()
	{
		return dungeonManager;
	}
}
