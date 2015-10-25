using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class GLexScene : GLexComponent {
	new protected GLexSceneSettings mComponent;
	private GLexData _glexData;
	private GooEnvironment _environment;
	
	public GLexScene(GLexData data) : base() {
		_glexData = data;
		_environment = new GooEnvironment(this);
	}
	
	// overrides
	
	public override void AssociateWithComponent(object obj) {
		base.AssociateWithComponent(obj);
		mComponent = (GLexSceneSettings)obj;	
	}

	public GooEnvironment Environment {
		get {
			return this._environment;
		}
	}	
	// template interface starts here
	
	public string[] AmbientColor {
		get {
			return new string[] {
				RenderSettings.ambientLight.r.ToString(),
				RenderSettings.ambientLight.g.ToString(),
				RenderSettings.ambientLight.b.ToString(),
				"1.0"
			};
		}
	}
	
	public string[] BackgroundColor {
		get {
			Camera camera = mGameObject.GetComponentInChildren<Camera>();
			if (camera != null) {
				return new string[] {
					camera.backgroundColor.r.ToString(),
					camera.backgroundColor.g.ToString(),
					camera.backgroundColor.b.ToString(),
					camera.backgroundColor.a.ToString()
				};
			}
			else {
				return new string[] { "0.3", "0.3", "0.3", "1.0" };
			}
		}
	}
	
	public GLexCamera DefaultCamera {
		get {
			foreach (var comp in _glexData.Components) {
				var cam = comp as GLexCamera;
				if (cam != null) {
					return cam;
				}
			}
			
			Debug.LogError("No default camera");
			return null;
		}
	}
	
	public bool HasFog {
		get {
			return RenderSettings.fog == true;
		}
	}
	
	public bool HasSkybox {
		get {
			Camera camera = mGameObject.GetComponentInChildren<Camera>();
			if (camera != null) {
				return camera.GetComponent<Skybox>() != null;
			}
			return false;
		}
	}
	
	public string[] SkyboxTextures {
		get {
			if (HasSkybox) {
				Camera camera = mGameObject.GetComponentInChildren<Camera>();
				if (camera != null) {
					Skybox skybox = camera.GetComponent<Skybox>();
					List<GLexTexture> textures = new List<GLexTexture>();
					List<string> textureNames = new List<string>() {
						"_FrontTex",
						"_BackTex",
						"_LeftTex",
						"_RightTex",
						"_UpTex",
						"_DownTex"
					};
					
					// TODO: this should be 
					foreach (string textureName in textureNames) {
						Texture2D texture = skybox.material.GetTexture(textureName) as Texture2D;
						textures.Add(GLexTexture.Get(texture));
					}

					List<string> texturePaths = new List<string>();
					foreach (GLexTexture texture in textures) {
						texturePaths.Add(texture.Id);
					}
					
					return texturePaths.ToArray();
				}
			} 
			return new string[] { "skyboxNotAvailable" };
		}
	}
	
	public override string Name {
		get {
			return Path.GetFileNameWithoutExtension(EditorApplication.currentScene);
		}
	}

	protected override string IdExtension {
		get {
			return GLexConfig.GetExtensionFor("scene");
		}
	}

}
