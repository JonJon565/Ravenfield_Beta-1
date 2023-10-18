using System;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
	private const int RESOLUTION = 512;

	[NonSerialized]
	public Camera camera;

	[NonSerialized]
	public RenderTexture minimapRenderTexture;

	private void Awake()
	{
		camera = GetComponent<Camera>();
		minimapRenderTexture = new RenderTexture(512, 512, 0);
		camera.targetTexture = minimapRenderTexture;
		bool fog = RenderSettings.fog;
		RenderSettings.fog = false;
		camera.Render();
		RenderSettings.fog = fog;
		camera.enabled = false;
	}

	public Texture Minimap()
	{
		return minimapRenderTexture;
	}
}
