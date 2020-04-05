using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
	public int playerDamage;

	private Animator animator;
	private Transform target;
	private bool skipMove;

    public AudioClip attackSound1;
    public AudioClip attackSound2;

    void Start()
    {
        GameManager.instance.addEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    	base.Start();
    }

    protected override bool attemptMove<T>(int xDir, int yDir)
    {
    	if(skipMove)
    	{
    		skipMove = false;
    		return false;
    	}


    	base.attemptMove<T>(xDir, yDir);
    	skipMove = true;
        return true;
    }

    public void moveEnemy()
    {
    	int xDir = 0;
    	int yDir = 0;

    	if(Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
    		yDir = target.position.y > transform.position.y ? 1 : -1;
    	else 
    		xDir = target.position.x > transform.position.x ? 1 : -1;

    	attemptMove<Player>(xDir, yDir);
    }

    protected override void onCantMove<T>(T component) 
    {
    	Player hitPlayer = component as Player;
    	hitPlayer.takeDamage(playerDamage);
    	animator.SetTrigger("enemyAttack");
        SoundManager.instance.randomizeSfx(attackSound1, attackSound2);
    }
}
