using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{

	public float moveAnimationTime = 0.1f;
	public LayerMask blockingLayer;

	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;
	public float inverseAnimationTime;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseAnimationTime = 1f / moveAnimationTime;
    }

    protected bool move(int xDir, int yDir, out RaycastHit2D hit)
    {
    	Vector2 start = transform.position;
    	Vector2 end = start + new Vector2(xDir, yDir);

    	boxCollider.enabled = false;
    	hit = Physics2D.Linecast(start, end, blockingLayer);
    	boxCollider.enabled = true;

    	if(hit.transform == null)
    	{
    		StartCoroutine(smoothMovement(end));
    		return true;
    	}

    	return false;

    }

    protected IEnumerator smoothMovement(Vector3 end)
    {
    	float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
    	while(sqrRemainingDistance > float.Epsilon)
    	{
    		Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseAnimationTime*Time.deltaTime);
    		rb2D.MovePosition(newPosition);
    		sqrRemainingDistance = (transform.position - end).sqrMagnitude;
    		yield return null;
    	}
    }

    protected virtual bool attemptMove<T>(int xDir, int yDir)
    	where T : Component
    {
    	RaycastHit2D hit;
    	bool canMove = move(xDir, yDir, out hit);

    	if (hit.transform == null)
    		return canMove;

    	T hitComponent = hit.transform.GetComponent<T>();
    	if(!canMove && hitComponent != null)
    		onCantMove(hitComponent);

        return canMove;
    }

    protected abstract void onCantMove<T> (T component)
    	where T : Component;
}
