using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Santa : MonoBehaviour{

	public bool controlEnabled;
	public float jumpPower;

	public  Rigidbody spineRb;
	public UnityEngine.Events.UnityEvent onPlayerJump;

	private Rigidbody[ ] rbs;

	private void Start() {
		ToggleRigidBodies( controlEnabled );
	}

	private void Update() {
		if (controlEnabled) {
			if (Input.GetKeyDown( KeyCode.Space )
				|| Input.GetMouseButtonDown(0)) {
				Jump();
			}
		}
	}

	private int jumpCount;
	private float lastJumpTime;
	private void Jump() {

		float pow = jumpPower;
		float jumpTime = Time.timeSinceLevelLoad;
		if(jumpTime - lastJumpTime > 0.5f
			|| spineRb.velocity.y < 0) {
			jumpCount = 0;
		}

		if (jumpCount < 2) {
			jumpCount++;
		}
		
		pow /= jumpCount;

		Vector3 up = Vector3.up;

		AddForceToRagdoll( up * pow );
		SFXManager.inst.PlayJumpSound();

		lastJumpTime = Time.timeSinceLevelLoad;

		if(onPlayerJump != null) {
			onPlayerJump.Invoke();
		}
	}

	public void ToggleRigidBodies(bool active) {
		if (rbs == null) {
			rbs = this.GetComponentsInChildren<Rigidbody>();
		}

		foreach(Rigidbody body in rbs) {
			body.isKinematic = !active;
		}
	}

	private void AddForceToRagdoll(Vector3 force) {
		if(rbs == null) {
			rbs = this.GetComponentsInChildren<Rigidbody>();
		}

		foreach(Rigidbody body in rbs) {
			body.AddForce( force );
		}
	}
	
	public void ToggleControl(bool toggleOn) {
		controlEnabled = toggleOn;
	}
}