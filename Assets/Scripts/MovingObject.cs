using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


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

		protected List<Vector2> getAllDirections()
		{
			List<Vector2> directions = new List<Vector2>();
			directions.Add(new Vector2(1, 0));
			directions.Add(new Vector2(-1, 0));
			directions.Add(new Vector2(0, 1));
			directions.Add(new Vector2(0, -1));

			return directions;
		}

		protected List<Vector2> getAvailableDirections()
		{
			List<Vector2> availableDirections = new List<Vector2>();
			foreach(Vector2 direction in getAllDirections())
			{
				RaycastHit2D hit = checkMove(direction);
				if (hit.transform == null)
				{
					availableDirections.Add(direction);
				} else {
					Debug.Log(hit.transform.gameObject + "(" + hit.transform.position + ") at direction " + direction);
				}
			}

			return availableDirections;
		}

		protected RaycastHit2D checkMove(Vector2 direction)
		{
			Vector3 start = transform.position;
			return Physics2D.Linecast(start, start + (Vector3)direction);
		}

    protected bool move(int xDir, int yDir, out RaycastHit2D hit)
    {
    	Vector3 start = transform.position;
    	Vector3 end = start + new Vector3(xDir, yDir, 0);

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
