﻿using System.Collections;
using System.Collections.Generic;
using Prime31;
using UnityEngine;

public class Movments : MonoBehaviour
{


	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;

	private int direction = -1;
	private float maxDistance = 10f;
	private float minDistance = 10f;
	private float initPosition;
	private float currentPosition;

	[HideInInspector] private float normalizedHorizontalSpeed = 0;

	private CharacterController2D _controller;
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;


	void Awake()
	{
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();

		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;
		initPosition = transform.position.x;
	}


	#region Event Listeners

	void onControllerCollider(RaycastHit2D hit)
	{
		// bail out on plain old ground hits cause they arent very interesting
		if (hit.normal.y == 1f)
			return;

		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		//Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
	}


	void onTriggerEnterEvent(Collider2D col)
	{
		Debug.Log("onTriggerEnterEvent: " + col.gameObject.name);
	}


	void onTriggerExitEvent(Collider2D col)
	{
		Debug.Log("onTriggerExitEvent: " + col.gameObject.name);
	}

	#endregion


	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
		
		EnemyMoveDirection();
		currentPosition = transform.position.x;
		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor =
			_controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed,
			Time.deltaTime * smoothedMovementFactor);

		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

		// if holding down bump up our movement amount and turn off one way platform detection for a frame.
		// this lets us jump down through one way platforms
		if (_controller.isGrounded && Input.GetKey(KeyCode.DownArrow))
		{
			_velocity.y *= 3f;
			_controller.ignoreOneWayPlatformsThisFrame = true;
		}
		_controller.move(_velocity * Time.deltaTime);

		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;
	}
	
	private void EnemyMoveDirection()
	{
		if (_controller.isGrounded)
			_velocity.y = 0;

		switch (direction)
		{
			case 1: // move left
				//Move left if the current position is more than the initial position
				if (currentPosition > initPosition - maxDistance)
				{
					print("Current Position: " + currentPosition + " ***** init+max: " + initPosition + maxDistance);
					normalizedHorizontalSpeed = -1;
					if (transform.localScale.x > 0f)
						transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

					if (_controller.isGrounded)
						_animator.Play(Animator.StringToHash("Walk"));

				}
				
				else
				{
					direction = -1;
				}
				

				break;
			case -1: //MOVE RIGHT ... if total distance is less than 20 + 5
				if (currentPosition < initPosition + maxDistance)
				{
					print("Current Position: " + currentPosition + " ***** init+max: " + initPosition + maxDistance);
					normalizedHorizontalSpeed = 1;
					if (transform.localScale.x < 0f)
						transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

					if (_controller.isGrounded)
						_animator.Play(Animator.StringToHash("Walk"));

				}
				else
				{
					direction = 1;
				}
				
				break;
		}
	}

	private void OnCollisionExit2D(Collision2D other)
	{
		Debug.Log(other.gameObject.tag);
	}
}



/*
// Update is called once per frame
void Update ()
{
	switch (direction)
	{
		case 1:
			print("ENEMY MOVING LEFT!!");
			// Moving Left
			if( transform.position.x > minDist)
			{
				normalizedHorizontalSpeed = -1;
				if (transform.position.x > 0f)
					transform.position = new Vector2(-transform.position.x, transform.position.y);
					_animator.Play(Animator.StringToHash("Walk"));
			}
			else
			{
				direction = -1;
			}
			break;
		case -1:
			print("ENEMY MOVING RIGHT!!");
			//Moving Right
			if(transform.position.x < maxDist)
			{
				normalizedHorizontalSpeed = 1;
				if (transform.position.x < 0f)
					transform.position = new Vector2(-transform.position.x, transform.position.y);
				_animator.Play(Animator.StringToHash("Walk"));
			}
			else
			{
				direction = 1;
			}
			break;
	}
}
}*/