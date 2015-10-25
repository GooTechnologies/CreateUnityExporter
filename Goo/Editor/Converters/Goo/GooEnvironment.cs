using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class GooEnvironment : GLexComponent {
	private GLexScene _scene;
	
	public GooEnvironment(GLexScene pScene) {
		_scene = pScene;
	}
	
	protected override string IdExtension {
		get {
			return ".environment";
		}
	}
	
	public GooSkybox Skybox {
		get {
			return _scene.DefaultCamera.Skybox;
		}
	}
	
	public Color BackgroundColor {
		get {
			return _scene.DefaultCamera.UnityCamera.backgroundColor;
		}
	}
	
	public Color AmbientColor {
		get {
			return RenderSettings.ambientLight;
		}
	}
	
	public bool IsFogEnabled {
		get {
			return RenderSettings.fog;
		}
	}
	
	public Color FogColor {
		get {
			return RenderSettings.fogColor;
		}
	}
	
	public float FogNear {
		get {
			return RenderSettings.fogStartDistance;
		}
	}
	
	public float FogFar {
		get {
			return RenderSettings.fogEndDistance;
		}
	}
}
