using UnityEngine;
using UnityEngine.Rendering;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(FirstPersonController))]
public class FpsActorController : ActorController
{
	private const float DEATH_TO_LOADOUT_TIME = 2f;

	private const int USE_LAYER_MASK = 2048;

	private const float MAX_USE_DISTANCE = 3f;

	private const float SEAT_CAMERA_OFFSET_UP = 0.85f;

	private const float SEAT_CAMERA_OFFSET_FORWARD = 0.2f;

	private const float EXIT_VEHICLE_PAD_UP = 0.8f;

	public const float HELICOPTER_FOV = 75f;

	public const float HELICOPTER_ZOOM_FOV = 50f;

	public const float DEFAULT_FOV = 60f;

	public const float DEFAULT_ZOOM_FOV = 45f;

	private const float CAMERA_RETURN_SPEED = 400f;

	private const float FINE_AIM_FOV = 30f;

	private const float FINE_AIM_MULTIPLIER = 0.3f;

	private const float CROUCH_HEIGHT = 0.5f;

	private const float STAND_HEIGHT = 1.8f;

	private const float HELICOPTER_SENSITIVITY = 0.25f;

	public static FpsActorController instance;

	public Camera fpCamera;

	public Transform fpCameraParent;

	public Camera tpCamera;

	public PlayerFpParent fpParent;

	public Transform weaponParent;

	private CharacterController characterController;

	private FirstPersonController controller;

	private Renderer[] thirdpersonRenderers;

	private Vector3 fpCameraParentOffset;

	private Vector3 actorLocalOrigin;

	private bool inputEnabled = true;

	private bool mouseViewLocked;

	private void Awake()
	{
		instance = this;
		controller = GetComponent<FirstPersonController>();
		characterController = GetComponent<CharacterController>();
		thirdpersonRenderers = actor.ragdoll.AnimatedRenderers();
		fpCameraParent = fpCamera.transform.parent;
		fpCameraParentOffset = fpCameraParent.transform.localPosition;
		EndCrouch();
	}

	private void Start()
	{
		SceneryCamera.instance.camera.enabled = true;
		actorLocalOrigin = actor.transform.localPosition;
		Invoke("OpenLoadoutWhileDead", 0.5f);
		DisableInput();
	}

	public override bool Fire()
	{
		if (IngameMenuUi.IsOpen())
		{
			return false;
		}
		return Input.GetButton("Fire1") && !LoadoutUi.IsOpen();
	}

	public override bool Aiming()
	{
		return Input.GetButton("Fire2") && !LoadoutUi.IsOpen();
	}

	public override bool Reload()
	{
		return Input.GetButton("Reload") && !LoadoutUi.IsOpen();
	}

	public override bool OnGround()
	{
		return controller.OnGround();
	}

	public override bool ProjectToGround()
	{
		return false;
	}

	public override Vector3 Velocity()
	{
		return controller.Velocity();
	}

	public override Vector3 SwimInput()
	{
		return tpCamera.transform.forward * Input.GetAxis("Vertical") + tpCamera.transform.right * Input.GetAxis("Horizontal");
	}

	public override Vector3 FacingDirection()
	{
		return fpCamera.transform.forward;
	}

	public override Vector2 BoatInput()
	{
		return CarInput();
	}

	public override Vector2 CarInput()
	{
		return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
	}

	public override Vector4 HelicopterInput()
	{
		if (Aiming())
		{
			return new Vector4(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f, 0f);
		}
		return new Vector4(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), GameManager.instance.mouseSensitivity * Input.GetAxis("Mouse X") * 50f * 0.25f, GameManager.instance.mouseSensitivity * Input.GetAxis("Mouse Y") * 75f * 0.25f);
	}

	public override bool UseMuzzleDirection()
	{
		return true;
	}

	public override void ReceivedDamage(float damage, float balanceDamage, Vector3 force)
	{
		if (balanceDamage > 5f)
		{
			fpParent.ApplyScreenshake(balanceDamage / 6f, Mathf.CeilToInt(balanceDamage / 20f));
		}
	}

	public override void DisableInput()
	{
		characterController.enabled = false;
		controller.inputEnabled = false;
		inputEnabled = false;
	}

	public override void EnableInput()
	{
		characterController.enabled = true;
		controller.inputEnabled = true;
		inputEnabled = true;
	}

	public override void StartSeated(Seat seat)
	{
		controller.DisableCharacterController();
		controller.SetMouseEnabled(seat.type != Seat.Type.Pilot);
		mouseViewLocked = seat.type == Seat.Type.Pilot;
		fpCameraParent.parent = seat.transform;
		fpCameraParent.localPosition = Vector3.up * 0.85f + Vector3.forward * 0.2f;
		fpCameraParent.localRotation = Quaternion.identity;
		if (!seat.CanUseWeapon())
		{
			if (seat.vehicle.GetType() == typeof(Helicopter))
			{
				fpParent.SetFov(75f, 50f);
			}
			else
			{
				fpParent.SetFov(60f, 45f);
			}
		}
		if (!seat.CanUseWeapon())
		{
			HideFpModel();
		}
	}

	public override void EndSeated(Vector3 exitPosition, Quaternion flatFacing)
	{
		controller.EnableCharacterController();
		controller.SetMouseEnabled(true);
		mouseViewLocked = false;
		base.transform.position = exitPosition + 0.8f * Vector3.up;
		base.transform.rotation = flatFacing;
		fpCameraParent.parent = base.transform;
		fpCameraParent.localPosition = fpCameraParentOffset;
		fpCameraParent.localRotation = Quaternion.identity;
		SetupWeaponFov(actor.activeWeapon);
		ShowFpModel();
		actor.transform.position = exitPosition;
	}

	public override void StartRagdoll()
	{
		ThirdPersonCamera();
	}

	public override void GettingUp()
	{
		base.transform.position = actor.ragdoll.Position() + Vector3.up * characterController.height / 2f;
		actor.transform.localPosition = actorLocalOrigin;
		Debug.DrawRay(base.transform.position, Vector3.up * 100f, Color.green, 100f);
	}

	public override void EndRagdoll()
	{
		FirstPersonCamera();
	}

	public override void Die()
	{
		ThirdPersonCamera();
		Invoke("OpenLoadoutWhileDead", 2f);
	}

	private void OpenLoadoutWhileDead()
	{
		if (actor.dead)
		{
			OpenLoadout();
		}
	}

	public void OpenLoadout()
	{
		LoadoutUi.Show();
		controller.SetMouseEnabled(false);
	}

	public void CloseLoadout()
	{
		LoadoutUi.Hide();
		controller.SetMouseEnabled(true);
	}

	public override void SpawnAt(Vector3 position)
	{
		SceneryCamera.instance.camera.enabled = false;
		EnableInput();
		controller.transform.position = position + Vector3.up * (characterController.height / 2f);
		controller.ResetVelocity();
		controller.SetMouseEnabled(true);
		FirstPersonCamera();
	}

	public override void ApplyRecoil(Vector3 impulse)
	{
		fpParent.ApplyRecoil(impulse);
		Weapon activeWeapon = actor.activeWeapon;
		fpParent.ApplyWeaponSnap(activeWeapon.configuration.snapMagnitude, activeWeapon.configuration.snapDuration, activeWeapon.configuration.snapFrequency);
	}

	public override float Lean()
	{
		return Input.GetAxis("Lean");
	}

	private void HideFpModel()
	{
		if (actor.HasUnholsteredWeapon())
		{
			actor.activeWeapon.Hide();
		}
	}

	private void ShowFpModel()
	{
		if (actor.HasUnholsteredWeapon())
		{
			actor.activeWeapon.Show();
		}
	}

	private void ThirdPersonCamera()
	{
		fpCamera.enabled = false;
		tpCamera.enabled = true;
		Renderer[] array = thirdpersonRenderers;
		foreach (Renderer renderer in array)
		{
			renderer.shadowCastingMode = ShadowCastingMode.On;
		}
	}

	private void FirstPersonCamera()
	{
		fpCamera.enabled = true;
		tpCamera.enabled = false;
		Renderer[] array = thirdpersonRenderers;
		foreach (Renderer renderer in array)
		{
			renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
		}
	}

	private void Update()
	{
		fpParent.lean = Lean();
		bool flag = actor.IsAiming();
		if (flag && actor.HasUnholsteredWeapon() && actor.activeWeapon.configuration.aimFov < 30f)
		{
			controller.SetMouseSensitivityMultiplier(0.3f * GameManager.instance.mouseSensitivity);
		}
		else
		{
			controller.SetMouseSensitivityMultiplier(GameManager.instance.mouseSensitivity);
		}
		if (flag)
		{
			fpParent.Aim();
		}
		else
		{
			fpParent.StopAim();
		}
		if (mouseViewLocked)
		{
			controller.SetMouseEnabled(flag);
			if (!flag)
			{
				fpCameraParent.transform.localRotation = Quaternion.RotateTowards(fpCameraParent.transform.localRotation, Quaternion.identity, Time.deltaTime * 400f);
			}
		}
		if (Input.GetButtonDown("Loadout"))
		{
			if (LoadoutUi.IsOpen())
			{
				CloseLoadout();
			}
			else
			{
				OpenLoadout();
			}
		}
		if (Input.GetKeyDown(KeyCode.K))
		{
			actor.Damage(200f, 200f, actor.CenterPosition(), Vector3.zero);
		}
		if (Input.GetKeyDown(KeyCode.O))
		{
			ActorManager.instance.debug = !ActorManager.instance.debug;
		}
		if (Input.GetKeyDown(KeyCode.CapsLock))
		{
			if (Time.timeScale < 1f)
			{
				Time.timeScale = 1f;
			}
			else if (!Input.GetKey(KeyCode.LeftShift))
			{
				Time.timeScale = 0.1f;
			}
			else
			{
				Time.timeScale = 0.33f;
			}
			Time.fixedDeltaTime = Time.timeScale / 60f;
		}
		if (inputEnabled)
		{
			UpdateInput();
		}
		if (!Input.GetButtonDown("Use"))
		{
			return;
		}
		if (!actor.IsSeated())
		{
			if (actor.CanEnterSeat())
			{
				SampleUseRay();
			}
		}
		else
		{
			actor.LeaveSeat();
		}
	}

	private void UpdateInput()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			actor.SwitchWeapon(0);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			actor.SwitchWeapon(1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			actor.SwitchWeapon(2);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			actor.SwitchWeapon(3);
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			actor.SwitchWeapon(4);
		}
	}

	private void SampleUseRay()
	{
		Ray ray = ((!actor.fallenOver) ? new Ray(fpCamera.transform.position, fpCamera.transform.forward) : new Ray(actor.CenterPosition(), tpCamera.transform.forward + tpCamera.transform.up * 0.2f));
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 3f, 2048) && hitInfo.collider.gameObject.layer == 11)
		{
			Seat component = hitInfo.collider.GetComponent<Seat>();
			actor.EnterSeat(component);
		}
	}

	private void LateUpdate()
	{
		if (tpCamera.enabled)
		{
			tpCamera.transform.rotation = fpCamera.transform.rotation;
			if (!actor.dead)
			{
				tpCamera.transform.position = actor.CenterPosition() - tpCamera.transform.forward * 3f + Vector3.up * 0.5f;
			}
		}
	}

	public override SpawnPoint SelectedSpawnPoint()
	{
		SpawnPoint spawnPoint = LoadoutUi.SelectedSpawnPoint();
		if (spawnPoint == null || spawnPoint.owner != actor.team)
		{
			return null;
		}
		return spawnPoint;
	}

	public override Transform WeaponParent()
	{
		return weaponParent;
	}

	public override void SwitchedToWeapon(Weapon weapon)
	{
		SetupWeaponFov(weapon);
	}

	private void SetupWeaponFov(Weapon weapon)
	{
		if (weapon != null)
		{
			fpParent.SetFov(60f, weapon.configuration.aimFov);
		}
		else
		{
			fpParent.SetFov(60f, 45f);
		}
	}

	public override WeaponManager.LoadoutSet GetLoadout()
	{
		return LoadoutUi.instance.loadout;
	}

	public override bool Crouch()
	{
		return Input.GetButton("Crouch");
	}

	public override void StartCrouch()
	{
		characterController.height = 0.5f;
	}

	public override void EndCrouch()
	{
		characterController.height = 1.8f;
		characterController.transform.position = characterController.transform.position + Vector3.up * 1.3f / 2f;
	}

	public override bool IsGroupedUp()
	{
		return false;
	}
}
