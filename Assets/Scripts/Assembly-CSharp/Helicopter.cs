using UnityEngine;

public class Helicopter : Vehicle
{
	private const float ROTOR_SPEED = 1000f;

	private const float ROTOR_SPEED_GAIN = 0.3f;

	private const float MAX_VOLUME = 0.5f;

	public Transform rotor;

	private Renderer solidRotor;

	private Renderer blurredRotor;

	public float rotorForce = 5f;

	public float manouverability = 1f;

	public float counterForceMultiplier = 0.3f;

	private float rotorSpeed;

	protected override void Awake()
	{
		base.Awake();
		solidRotor = rotor.GetComponent<Renderer>();
		blurredRotor = rotor.GetChild(0).GetComponent<Renderer>();
	}

	private void Update()
	{
		audio.volume = rotorSpeed * 0.5f;
		audio.pitch = rotorSpeed;
		if (HasDriver())
		{
			rotorSpeed = Mathf.Clamp01(rotorSpeed + Time.deltaTime * 0.3f);
			if (base.transform.up.y < 0f)
			{
				Damage(Time.deltaTime * 30f);
			}
		}
		else
		{
			rotorSpeed = Mathf.Clamp01(rotorSpeed - Time.deltaTime * 0.3f);
		}
		bool flag = rotorSpeed > 0.8f;
		solidRotor.enabled = !flag;
		blurredRotor.enabled = flag;
		rotor.Rotate(Vector3.forward * 1000f * rotorSpeed * Time.deltaTime);
	}

	protected override void DriverEntered()
	{
		base.DriverEntered();
	}

	protected override void DriverExited()
	{
		base.DriverExited();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (HasDriver())
		{
			Vector4 vector = Vehicle.Clamp4(Driver().controller.HelicopterInput()) * rotorSpeed;
			float y = vector.y;
			Vector3 torque = new Vector3(vector.w, vector.x, 0f - vector.z) * manouverability;
			Vector3 normalized = (base.transform.up + Vector3.forward * 0.1f).normalized;
			float t = Mathf.Clamp01(0f - Vector3.Dot(normalized, rigidbody.velocity.normalized));
			float num = 1f + Mathf.Lerp(0f, counterForceMultiplier, t);
			rigidbody.AddForce(normalized * (y * rotorForce * num - Physics.gravity.y - 0.5f), ForceMode.Acceleration);
			rigidbody.AddRelativeTorque(torque, ForceMode.Acceleration);
		}
	}

	public override void Die()
	{
		base.Die();
		rotor.gameObject.SetActive(false);
		audio.Stop();
	}
}
