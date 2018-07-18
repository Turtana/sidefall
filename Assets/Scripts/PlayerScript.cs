using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

	public GameObject PlayerCamera;
	public float movementSpeed;
	public float cameraSpeed;
	public float jumpForce;
	public float gravityStrength;
	public GameObject aimBall;
	public Sprite crosshair;
	public Sprite crosshair_gray;
	public Image crosshair_ui;
	public GameObject feet;

	private Rigidbody rb;
	private bool hasJumped; // This really means "mid-air"...
	private float sprintDoubling;
	private float personalGravity;
	private Vector3 localvel;
	private Vector4 localCameraRot;
	private GameObject current_grav_tag;

	private bool rotating;
	private Quaternion rotOriginal;
	private Quaternion rotDestination;
	private float rotatingCount;

	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		rb = GetComponent<Rigidbody>();
		hasJumped = false;
		rotating = false;
		sprintDoubling = 1;
		personalGravity = -1.0f;
		crosshair_ui.sprite = crosshair;
		rotatingCount = 0;
	}

	void FixedUpdate () {
		if (!rotating) {
			float movement_x = Input.GetAxis("Horizontal") * Time.deltaTime * 100.0f * sprintDoubling; // Camera + movement handling in different rotations
	        float movement_z = Input.GetAxis("Vertical") * Time.deltaTime * 100.0f * sprintDoubling;
			float mouse_x = Input.GetAxis("Mouse X");
			float mouse_y = Input.GetAxis("Mouse Y");

			localvel = transform.InverseTransformDirection(rb.velocity);
			Vector3 movement = new Vector3 (movement_x * movementSpeed, localvel.y, movement_z * movementSpeed);
			rb.velocity = transform.rotation * movement;
			rb.velocity += transform.up * personalGravity * gravityStrength;

			localCameraRot = PlayerCamera.transform.localRotation.eulerAngles;
			float cameraX = localCameraRot.x - mouse_y * cameraSpeed;
			if (cameraX >= 270 || cameraX <= 90) { // No flips today
				PlayerCamera.transform.Rotate(-mouse_y * cameraSpeed, 0, 0);
			}
			transform.Rotate(0, mouse_x * cameraSpeed, 0);
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			Cursor.lockState = CursorLockMode.None;
		}
		if (Input.GetKeyDown(KeyCode.Space) && !rotating) {
 			jump();
 		}
		if (Input.GetKey(KeyCode.LeftShift)) {
 			sprintDoubling = 2;
 		} else {
			sprintDoubling = 1;
		}
	}

	void Update() { // Shoot gravity tag
		if (Input.GetMouseButtonDown(0) && !hasJumped) {
			Vector3 centrum = new Vector3 (Screen.width / 2, Screen.height / 2, 0);
			Ray ray = Camera.main.ScreenPointToRay(centrum);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) { // checks if the player has clicked an collision box
				hasJumped = true;
				crosshair_ui.sprite = crosshair_gray;
				if (current_grav_tag) {
					Destroy(current_grav_tag);
				}
				current_grav_tag = Instantiate(aimBall, hit.point + hit.normal / 100, Quaternion.LookRotation(hit.normal));
				rotateGravity(hit.normal); // This side UP
			}
		}

		if (rotating) {
			rotatingCount += Time.deltaTime;

			transform.rotation = Quaternion.Slerp(rotOriginal, rotDestination, rotatingCount * 2); // Half a second
			PlayerCamera.transform.LookAt(current_grav_tag.transform.position, transform.up); // Camera handling

			if (rotatingCount >= 1) {
				rotatingCount = 0;
				rotating = false;
				transform.Rotate(0, PlayerCamera.transform.localEulerAngles.y, 0); // Turn player according to the camera
				PlayerCamera.transform.localRotation = Quaternion.Euler(PlayerCamera.transform.eulerAngles.x, 0, 0); // Vice versa
				PlayerCamera.transform.LookAt(current_grav_tag.transform.position, transform.up);
			}
		}
	}

	private void rotateGravity(Vector3 direction) {
		rotating = true;
		if (direction.z < -0.1) {
			rotDestination = Quaternion.Euler(-90, 0, 0); // Direction handling
		} else if (direction.z > 0.1) {
			rotDestination = Quaternion.Euler(90, 0, 0);
		}
		if (direction.y > 0.1) {
			rotDestination = Quaternion.Euler(0, 0, 0);
		} else if (direction.y < -0.1) {
			rotDestination = Quaternion.Euler(0, 0, 180);
		}
		if (direction.x < -0.1) {
			rotDestination = Quaternion.Euler(0, 0, 90);
		} else if (direction.x > 0.1) {
			rotDestination = Quaternion.Euler(0, 0, -90);
		}
		rotOriginal = transform.rotation;
		rb.velocity = Vector3.zero;
	}

	private void jump () {
		if (!hasJumped) {
			rb.velocity += transform.up * jumpForce;
		}
	}

	public void feetOnTheGround (bool isHe) {
		if (!rotating) {
			if (isHe) {
				hasJumped = false;
				crosshair_ui.sprite = crosshair;
			} else {
				hasJumped = true;
				crosshair_ui.sprite = crosshair_gray;
			}
		}
	}
}
