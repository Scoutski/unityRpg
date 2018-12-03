using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public Rigidbody2D rigidBody;
	public float moveSpeed;

	public Animator animator;

	public static PlayerController instance;

	public string areaTransitionName;

	private Vector3 bottomLeftLimit;
	private Vector3 topRightLimit;

	public bool canMove = true;

	void Start () {
		if (instance == null) {
			instance = this;
		} else {
			if (instance != this) {
				Destroy(gameObject);
			}
		}

		DontDestroyOnLoad(gameObject);
	}
	
	void Update () {
		float horizontalInput = Input.GetAxisRaw("Horizontal");
		float verticalInput = Input.GetAxisRaw("Vertical");
		if (canMove) {
			rigidBody.velocity = new Vector2(
				horizontalInput * moveSpeed,
				verticalInput * moveSpeed);			
		} else {
			rigidBody.velocity = Vector2.zero;
		}

		animator.SetFloat("moveX", rigidBody.velocity.x);
		animator.SetFloat("moveY", rigidBody.velocity.y);

		if (horizontalInput == 1 || horizontalInput == -1 || verticalInput == 1 || verticalInput == -1) {
			if (canMove) {
				animator.SetFloat("lastMoveX", horizontalInput);
				animator.SetFloat("lastMoveY", verticalInput);
			}
		}

		// Keep the player inside the bounds of the tilemap.
		transform.position = new Vector3(
			Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x),
			Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y),
			transform.position.z);
	}

	public void SetBounds(Vector3 bottomLeft, Vector3 topRight) {
		bottomLeftLimit = bottomLeft + new Vector3(.5f, .9f, 0f);
		topRightLimit = topRight + new Vector3(-.5f, -.9f, 0f);
	}
}
