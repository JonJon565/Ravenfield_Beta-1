using UnityEngine;

public class AlternatingMountedWeapon : MountedWeapon
{
	public Transform[] muzzles;

	private int currentMuzzle;

	protected override void SpawnProjectile(Vector3 direction)
	{
		Transform transform = muzzles[currentMuzzle];
		currentMuzzle = (currentMuzzle + 1) % muzzles.Length;
		Quaternion rotation = Quaternion.LookRotation(direction + Random.insideUnitSphere * configuration.spread);
		((GameObject)Object.Instantiate(configuration.projectilePrefab, transform.position, rotation)).GetComponent<Projectile>().source = user;
	}

	public override Vector3 MuzzlePosition()
	{
		return muzzles[currentMuzzle].position;
	}
}
