﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
	public int playerDamage;
	public int range;
	public float wanderSpeed;
	public float chaseSpeed;
	public static float speedMultiplier;

	private Animator animator;
	private Transform target;
	private Player player;
	private bool skipMove;
	private bool attacking;
	private bool moving;

    public AudioClip attackSound1;
    public AudioClip attackSound2;

    void Start()
    {
      	GameManager.instance.addEnemyToList(this);
      	animator = GetComponent<Animator>();
	  	player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
      	target = GameObject.FindGameObjectWithTag("Player").transform;
		speedMultiplier = wanderSpeed;
		moving = false;
		attacking = false;
    	base.Start();
    }

	void Update()
	{
		if (!moving) {
			StartCoroutine(startMove());
		}
	}
	
	IEnumerator startMove()
	{
		moving = true;
		Move();
		yield return new WaitForSeconds(GameManager.baseEnemySpeed * speedMultiplier);
		// yield return new WaitForSeconds(moveAnimationTime);
		moving = false;
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

    public void Move()
    {
			searchForPlayer();
			int yDir = 0;
			int xDir = 0;
			if (attacking)
			{
				speedMultiplier = chaseSpeed;
				Vector3 offset = target.position - transform.position;
				float xMagnitude = Mathf.Abs(offset.x);
				float yMagnitude = Mathf.Abs(offset.y);
				if (yMagnitude > float.Epsilon)
				{
					yDir = (int) (offset.y / yMagnitude);
				}

				if (xMagnitude > float.Epsilon)
				{
					xDir = (int) (offset.x / xMagnitude);
				}

				if (xDir != 0 && yDir != 0)
				{
					if (Random.Range(0,2) == 0)
					{
						xDir = 0;
					}
					else {
						yDir = 0;
					}
				}
			} else {
				speedMultiplier = wanderSpeed;
				int randomMotion = Random.Range(0,8);
				switch (randomMotion)
				{
					case 0:
						xDir = -1;
						break;
					case 1:
						xDir = 1;
						break;
					case 2:
						yDir = -1;
						break;
					case 3:
						yDir = 1;
						break;
					default:
						break;
				}
			}

			Vector3 newLocation = new Vector3(transform.position.x + xDir, transform.position.y + yDir, 0);
			if (newLocation != player.destinationTile || !player.moving)
			{
				attemptMove<Player>(xDir, yDir);
			}
    }

		public void searchForPlayer()
		{
			if (transform.position.z != target.position.z)
			{
				attacking = false;
				return;
			}
			RaycastHit hit;
			Physics.Linecast(transform.position, target.position, out hit, blockingLayer);
			if (hit.transform == target)
			{
				float dist = Vector3.Distance(target.position, transform.position);
				attacking = (dist < range);
			}
		}

    protected override void onCantMove<T>(T component)
    {
    	Player hitPlayer = component as Player;
    	hitPlayer.takeDamage(playerDamage);
    	animator.SetTrigger("enemyAttack");
      SoundManager.instance.randomizeSfx(attackSound1, attackSound2);
    }
}
