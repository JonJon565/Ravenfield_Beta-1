using UnityEngine;
using UnityEngine.UI;

public class Binoculars : ScopedWeapon
{
	private int lastRangeReading;

	private Action findRangeAction = new Action(0.3f);

	public Canvas rangeCanvas;

	public Text rangeText;

	protected override void Update()
	{
		base.Update();
		if (findRangeAction.TrueDone() && scope.activeInHierarchy)
		{
			Ray ray = new Ray(configuration.muzzle.position, configuration.muzzle.forward);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 999f))
			{
				lastRangeReading = Mathf.RoundToInt(hitInfo.distance);
			}
			else
			{
				lastRangeReading = 999;
			}
			rangeText.text = lastRangeReading.ToString();
			findRangeAction.Start();
		}
	}

	public override void Fire(Vector3 direction, bool useMuzzleDirection)
	{
	}
}
