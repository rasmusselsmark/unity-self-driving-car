using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	// the game object camera should look at
	public Transform Target;

	// Camera offset from the target position
	public Vector3 CameraOffset;

	// optionally look above target  
	public Vector3 TargetOffset;

	// Camera speeds
	[Range(0, 10)]
	public float lerpPositionMultiplier = 1f;

	[Range(0, 10)]
	public float lerpRotationMultiplier = 1f;

	// We use a rigidbody to prevent the camera from going in walls but it means sometime it can get stuck
	Rigidbody rb;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		// normalise velocity so it doesn't jump too far
		this.rb.velocity.Normalize();

		// Save transform locally
		Quaternion curRot = transform.rotation;
		Vector3 tPos = Target.position + Target.TransformDirection(CameraOffset);

		// Look at the target
		transform.LookAt(Target.position + TargetOffset);

		// // Keep the camera above the target y position
		// if (tPos.y < target.position.y)
		// {
		// 	tPos.y = target.position.y;
		// }

		// Set transform with lerp
		transform.position = Vector3.Lerp(transform.position, tPos, Time.fixedDeltaTime * lerpPositionMultiplier);
		transform.rotation = Quaternion.Lerp(curRot, transform.rotation, Time.fixedDeltaTime * lerpRotationMultiplier);

		// Keep camera above the y:0.5f to prevent camera going underground
		// if (transform.position.y < 0.5f)
		// {
		// 	transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
		// }
	}
}
