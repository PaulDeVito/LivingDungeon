using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
    public static Player instance;

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 0.5f;
    public bool moving;
    public float timeToMove = 0.2f;
    public Vector3 destinationTile;

    public Text foodText;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip chopSound1;
    public AudioClip chopSound2;
    public AudioClip gameOverSound;

	  private Animator animator;
	  private int food;

    // Start is called before the first frame update
    protected override void Start()
    {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        foodText = GameObject.Find("FoodText").GetComponent<Text>();
        moving = false;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // GameManager.instance.updateCamera(transform.position);
        if (moving) return;

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        if(horizontal != 0)
        {
        	vertical = 0;
        }

        if (horizontal !=0 || vertical != 0)
        {
        	attemptMove<Wall> (horizontal, vertical);
        }
    }

    private void changeRooms()
    {
      GameManager.triggerRoomChange();
    }

    private void OnTriggerEnter(Collider other)
    {
    	if(other.tag == "Exit")
    	{
    		Invoke("changeRooms", moveAnimationTime + .01f);
    	}

    	else if(other.tag == "Food")
    	{
    		food += pointsPerFood;
            foodText.text = "Food: " + food;
    		other.gameObject.SetActive(false);
            SoundManager.instance.randomizeSfx(eatSound1, eatSound2);
            GameManager.getDungeonManager().getCurrentRoom().removeFoodAtPosition(other.transform.position);
    	}

    	else if(other.tag == "Soda")
    	{
    		food += pointsPerSoda;
            foodText.text = "Food: " + food;
    		other.gameObject.SetActive(false);
            SoundManager.instance.randomizeSfx(drinkSound1, drinkSound2);
            GameManager.getDungeonManager().getCurrentRoom().removeFoodAtPosition(other.transform.position);
    	}

      else if(other.tag == "Stairs")
    	{
        SoundManager.instance.randomizeSfx(drinkSound1, drinkSound2);
        GameManager.instance.triggerWinGame();
    	}
    }

    protected override bool attemptMove<T>(int xDir, int yDir)
    {
    	bool didMove = base.attemptMove<T>(xDir, yDir);
    	RaycastHit hit;

      if(didMove)
      {
        SoundManager.instance.randomizeSfx(moveSound1, moveSound2);
        moving = true;
        int newX = (int)gameObject.transform.position.x + xDir;
        int newY = (int)gameObject.transform.position.y + yDir;
        destinationTile = new Vector3(newX, newY, 0);
        Invoke("finishMoving", timeToMove);

        // yield return new WaitForSeconds(GameManager.instance.setPlayeray);
      }

    	checkIfGameOver();
      return didMove;
    }

    private void finishMoving()
    {
      moving = false;
    }

    protected override void onCantMove<T>(T component)
    {
    	Wall hitWall = component as Wall;
    	hitWall.damageWall(wallDamage);
    	animator.SetTrigger("playerChop");
        SoundManager.instance.randomizeSfx(chopSound1, chopSound2);
    }


    public void takeDamage(int loss)
    {
    	animator.SetTrigger("playerHit");
    	food -= loss;
        foodText.text = "Food: " + food;
    	checkIfGameOver();
    }

    private void checkIfGameOver()
    {
    	if (food <= 0)
        {
          SoundManager.instance.playSingle(gameOverSound);
    		  SoundManager.instance.musicSource.Stop();
          GameManager.instance.triggerGameOver();
        }
    }
}
