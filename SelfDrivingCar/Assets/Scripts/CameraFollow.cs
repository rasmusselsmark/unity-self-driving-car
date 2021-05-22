using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	// the game object camera is looking at
	public Transform Target;

	// Camera offset from the target position
	public Vector3 CameraOffset;

	// optionally look e.g. above target
	public Vector3 TargetOffset;

	// Camera speeds
	[Range(0, 10)]
	public float LerpPositionMultiplier = 1f;

	[Range(0, 10)]
	public float LerpRotationMultiplier = 1f;

	// We use a rigidbody to prevent the camera from going in walls but it means sometime it can get stuck
	Rigidbody rb;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		// normalise velocity so it doesn't jump too far
		rb.velocity.Normalize();

		// Save transform locally
		Quaternion curRot = transform.rotation;
		Vector3 tPos = Target.position + Target.TransformDirection(CameraOffset);

		// Look at the target
		transform.LookAt(Target.position + TargetOffset);

		// Set transform with lerp
		transform.position = Vector3.Lerp(transform.position, tPos, Time.fixedDeltaTime * LerpPositionMultiplier);
		transform.rotation = Quaternion.Lerp(curRot, transform.rotation, Time.fixedDeltaTime * LerpRotationMultiplier);
	}
}
