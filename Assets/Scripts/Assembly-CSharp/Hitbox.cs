using UnityEngine;

public class Hitbox : MonoBehaviour
{
	public const int LAYER = 8;

	public const int RAGDOLL_LAYER = 10;

	private const float RIGIDBODY_HIT_FORCE = 0.01f;

	public Hurtable parent;

	public float multiplier = 1f;

	public void ProjectileHit(Projectile p, Vector3 position)
	{
		parent.Damage(p.Damage() * multiplier, p.BalanceDamage(), position, p.configuration.impactForce * p.transform.forward);
	}

	public void RigidbodyHit(Rigidbody r, Vector3 position)
	{
		parent.Damage(20f, 200f, position, r.velocity * r.mass * 0.01f);
	}
}
