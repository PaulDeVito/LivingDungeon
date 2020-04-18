using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public abstract class MovingObject : MonoBehaviour
{

	public float moveAnimationTime = 0.1f;
	public LayerMask blockingLayer;

	private BoxCollider boxCollider;
	private Rigidbody rigidbody;
	public float inverseAnimationTime;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        rigidbody = GetComponent<Rigidbody>();
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
				RaycastHit hit = checkMove(direction);
				if (hit.transform == null)
				{
					availableDirections.Add(direction);
				} else {
					Debug.Log(hit.transform.gameObject + "(" + hit.transform.position + ") at direction " + direction);
				}
			}

			return availableDirections;
		}

		protected RaycastHit checkMove(Vector2 direction)
		{
			Vector3 start = transform.position;
			RaycastHit hit;
			Physics.Linecast(start, start + (Vector3)direction, out hit, blockingLayer);
			return hit;
		}

    protected bool move(int xDir, int yDir, out RaycastHit hit)
    {
    	Vector3 start = transform.position;
    	Vector3 end = start + new Vector3(xDir, yDir, 0);

    	boxCollider.enabled = false;
    	Physics.Linecast(start, end, out hit, blockingLayer);
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
			float originalZ = transform.position.z;
    	float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
    	while(sqrRemainingDistance > float.Epsilon)
    	{
				if (transform.position.z != originalZ)
				{
					end = new Vector3(end.x, end.y, transform.parent.position.z);
				}
    		Vector3 newPosition = Vector3.MoveTowards(rigidbody.position, end, inverseAnimationTime*Time.deltaTime);
    		rigidbody.MovePosition(newPosition);
    		sqrRemainingDistance = (transform.position - end).sqrMagnitude;
    		yield return null;
    	}
    }

    protected virtual bool attemptMove<T>(int xDir, int yDir)
    	where T : Component
    {
    	RaycastHit hit;
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
