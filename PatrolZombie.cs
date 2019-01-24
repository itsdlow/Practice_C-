using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolZombie : MonoBehaviour {

	public Transform[] patrolPoints;
	int patrolIndex;

	public float viewRadius;
	public float viewAngle;


	Transform targetPatrolPoint;
	FieldOfViewUI fovUI;

	NavMeshAgent agent;
	GameObject playerObj;

	bool aggro;
	bool wanderCooldown;
	public float wanderTimer = 2.0f;

	Animator anim;

	// Use this for initialization
	void Start () {
		playerObj = GameObject.FindGameObjectWithTag ("Player");
		aggro = false;

		targetPatrolPoint = patrolPoints [patrolIndex];

		fovUI = GetComponent<FieldOfViewUI> ();
		fovUI.viewAngle = viewAngle;
		fovUI.viewRadius = viewRadius;


		agent = GetComponent<NavMeshAgent> ();

		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		anim.SetFloat ("Forward", agent.velocity.magnitude);

		if (aggro == false)
		{
			Patrol ();

			if (hasLineOfSight () && hasFieldOfView ())
			{
				aggro = true;
				viewAngle = 360;
				fovUI.viewAngle = viewAngle;
			}
		}
		else
		{
			if (hasLineOfSight () && hasFieldOfView ())
			{
				agent.SetDestination (playerObj.transform.position);
			}
			else
			{
				agent.SetDestination (transform.position);
			}
		}
	}


	void Patrol()
	{
		agent.SetDestination (targetPatrolPoint.position);

		float distToTarget = Vector3.Distance (targetPatrolPoint.position, transform.position);

		if (distToTarget < 1.0f)
		{
			patrolIndex += 1;

			if (patrolIndex >= patrolPoints.Length)
			{
				patrolIndex = 0;
			}
			targetPatrolPoint = patrolPoints [patrolIndex];
		}
	}

	bool hasLineOfSight()
	{
		bool output = false;

		RaycastHit hit;
		Vector3 origin = new Vector3(transform.position.x, 1, transform.position.z);
		Vector3 playerPos = new Vector3 (playerObj.transform.position.x, 1, playerObj.transform.position.z);
		Vector3 direction = playerPos - origin;

		if (Physics.Raycast (origin, direction, out hit, viewRadius)) {

			// Get the object that was hit
			GameObject objHit = hit.transform.gameObject;

			// Check the tag
			if (objHit.tag == "Player") {
				output = true;
			}
		}
		return output;

	}

	bool hasFieldOfView()
	{
		bool output = false;
		
		Vector3 dirToPlayer = playerObj.transform.position - transform.position;

		float angle = Vector3.Angle (transform.forward, dirToPlayer);

		if (Mathf.Abs (angle) < viewAngle)
		{
			output = true;
		}
		return output;
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			Player player = other.gameObject.GetComponent<Player> ();

			player.YouDied ();

		}
	}

}
