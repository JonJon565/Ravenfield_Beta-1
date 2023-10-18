using UnityEngine;

public class Spring
{
	private float spring;

	private float drag;

	public Vector3 position;

	public Vector3 velocity;

	private Vector3 min;

	private Vector3 max;

	public Spring(float spring, float drag, Vector3 min, Vector3 max)
	{
		this.spring = spring;
		this.drag = drag;
		position = Vector3.zero;
		velocity = Vector3.zero;
		this.min = min;
		this.max = max;
	}

	public void AddVelocity(Vector3 delta)
	{
		velocity += delta;
	}

	public void Update()
	{
		velocity -= (position * spring + velocity * drag) * Time.deltaTime;
		position = Vector3.Min(Vector3.Max(position + velocity * Time.deltaTime, min), max);
	}
}
