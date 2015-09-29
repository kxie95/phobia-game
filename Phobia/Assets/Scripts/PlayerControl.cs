﻿using UnityEngine;
using UnityEngine.Networking;

public class PlayerControl : NetworkBehaviour
{
	private const float DOOR_JUMP = 2;

	public float speed = 6f;            // The speed that the player will move at.


    private Vector3 movement;                   // The vector to store the direction of the player's movement.
    private Vector3 cameraPosition = new Vector3 (0, 26, -17);

    private int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
    private float camRayLength = 100f;          // The length of the ray from the camera into the scene.

    private Transform mainCameraTransform;

    void Awake ()
	{
        this.mainCameraTransform = Camera.main.GetComponentInParent<Transform>();
    }


    void FixedUpdate()
    {
        if (IsMine())
        {
            // Store the input axes.
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            floorMask = LayerMask.GetMask("Floor");

            // Move the player around the scene.
            Move(h, v);

            // Turn the player to face the mouse cursor.
            Turning();
        }
	}


	void Move (float h, float v)
	{
		// Set the movement vector based on the axis input.
		movement.Set (h, 0f, v);
		
		// Normalise the movement vector and make it proportional to the speed per second.
		movement = movement.normalized * speed * Time.deltaTime;

        // Move the player to it's current position plus the movement.
        GetComponent<Rigidbody>().MovePosition (transform.position + movement);
	}


    void Turning()
    {
        // Create a ray from the mouse cursor on screen in the direction of the camera.
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Create a RaycastHit variable to store information about what was hit by the ray.
        RaycastHit floorHit;

        // Perform the raycast and if it hits something on the floor layer...
        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
        {
            // Create a vector from the player to the point on the floor the raycast from the mouse hit.
            Vector3 playerToMouse = floorHit.point - transform.position;

            // Ensure the vector is entirely along the floor plane.
            playerToMouse.y = 0f;

            // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
            Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);

            // Set the player's rotation to this new rotation.
            GetComponent<Rigidbody>().MoveRotation(newRotatation);
        }
    }
	
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Door"))
		{
            DoorControl doorMono = other.gameObject.GetComponent<DoorControl>();
            Debug.Log("Door fired: " + doorMono.gameObject.GetInstanceID());
            Vector3 goalPos = doorMono.goalDoor.transform.position + doorMono.goalDoor.transform.forward * DOOR_JUMP;
            goalPos.y = 0;
            this.transform.position = goalPos;
            if (IsLocalPlayer()) {
                GetComponent<NetworkTransform>().InvokeSyncEvent(0, null);
            }
            if (IsMine()) {
                mainCameraTransform.position = (doorMono.goalRoom.transform.position) + cameraPosition;
            }
		}
	}

    override public void OnStartClient()
    {
        Debug.Log("started client");
    }

    override public void OnStartLocalPlayer()
    {
        Debug.Log("started local player");
    }

    public bool IsMine()
    {
        return GetComponentInParent<NetworkIdentity>() == null || this.GetComponentInParent<NetworkIdentity>().isLocalPlayer;
    }

    public bool IsLocalPlayer()
    {
        return GetComponentInParent<NetworkIdentity>() != null && this.GetComponentInParent<NetworkIdentity>().isLocalPlayer;
    }
}