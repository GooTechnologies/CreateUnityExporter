using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GLexData {
	public static GLexData	Instance { get; private set; }
	
	private List<GLexGameObject> 	mGLexGameObjects;
	private List<GLexGameObject> 	mGLexTopGameObjects;
	private List<GLexComponent>		mGLexComponents;
	private GLexGameObject[]		mGLexGameObjectsAsArray;
	private GLexGameObject[]		mGLexTopGameObjectsAsArray;
	private GLexComponent[]			mGLexComponentsAsArray;
	private GLexScene				mScene;
	private GooProject				_project;
	
	public GLexData() {
		Instance = this;
		
		GLexMaterial    	   .Reset();
		GLexMesh        	   .Reset();
		GLexTexture			   .Reset();
		GLexShader      	   .Reset();
		GLexSkinnedMeshRenderer.Reset();
		GLexBone               .Reset();
		GLexAnimation		   .Reset();
		GLexAnimationClip      .Reset();
		GLexAnimationState.Reset();
		GooSkybox.Reset();
		GLexAudioSource.Reset();
		GooSkeleton.Reset();

		
		mGLexGameObjects = new List<GLexGameObject>();
		mGLexTopGameObjects = new List<GLexGameObject>();
		mGLexComponents = new List<GLexComponent>();
	}
	
	public GLexGameObject FindObject(GameObject pObject) {
		foreach (var obj in mGLexGameObjects) {
			if (obj.GameObject == pObject) {
				return obj;
			}
		}
		return null;
	}
	
	public void AddGameObject(GLexGameObject converter) {
		mGLexGameObjects.Add(converter);
		Debug.Log("Added game object: " + converter.Name);
	}
	
	public void AddComponent(GLexComponent converter) {
		mGLexComponents.Add(converter);
			
		if (converter is GLexScene) {
			if (mScene != null) {
				mScene = converter as GLexScene;
			}
			else {
				Debug.LogError("GLexData.AddComponent: Only one GLexScene allowed in each export!");
			}
		}
	}
	
	public void Remove(System.Object component, GameObject gameObject = null) {
		foreach (GLexComponent glexComponent in mGLexComponents) {
			if (glexComponent.Component == component) {
				mGLexComponents.Remove(glexComponent);
				
				if (gameObject != null) {
					foreach (GLexGameObject glexGameObject in mGLexGameObjects) {
						if (glexGameObject.GameObject == gameObject) {
							mGLexGameObjects.Remove(glexGameObject);
							break;
						}
					}
				}
				break;
			}
		}
	}
	
	private static List<GLexGameObject> _addedSettingsTo = new List<GLexGameObject>();

	public static void RemoveAddedSettings() {
		
		foreach (GLexGameObject go in _addedSettingsTo) {
			GameObject.DestroyImmediate(go.GameObject.GetComponent<GLexGameObjectSettings>());
		}
		
		_addedSettingsTo.Clear();
	}
	
	public void PrepareForExport() {
		if (GLexConfig.GetOption(GLexConfig.SETUNIQUENAMES)) {
			Dictionary<string,int> uniqueNames = new Dictionary<string,int>();

			foreach (GLexGameObject gameObject in mGLexGameObjects) {
				if (gameObject.Settings == null) {
					_addedSettingsTo.Add(gameObject);
					gameObject.AddSettings();					
				}
				else {
					gameObject.ResetSettingsExportName();
				}
			}
			
			foreach (GLexGameObject gameObject in mGLexGameObjects) {
				if (uniqueNames.ContainsKey(gameObject.Settings.UniqueName)) {
					gameObject.Settings.UniqueName = gameObject.Settings.UniqueName + (++uniqueNames[gameObject.Settings.UniqueName]).ToString();
				}
				else {
					uniqueNames.Add(gameObject.Settings.UniqueName, 0);
				}
			}
		}
		
		// Keep preparing objects while new ones are being added
		int prevGameObjectCount = 0, prevComponentCount = 0;
		while (prevGameObjectCount < mGLexGameObjects.Count || prevComponentCount < mGLexComponents.Count) {
			int goStart = prevGameObjectCount;
			int gcStart = prevComponentCount;
			prevGameObjectCount = mGLexGameObjects.Count;
			prevComponentCount = mGLexComponents.Count;
			
			for (int i = goStart; i < prevGameObjectCount; ++i) {
				mGLexGameObjects[i].PrepareForExport();
			}
			for (int i = gcStart; i < prevComponentCount; ++i) {
				mGLexComponents[i].PrepareForExport();
			}
		}

		// find top objects
		foreach (GLexGameObject gameObject in mGLexGameObjects) {
			if (!gameObject.HasParent) {
				mGLexTopGameObjects.Add (gameObject);
			}
		}

		GLexMaterial		   .PrepareForExport();
		GLexMesh    		   .PrepareForExport();
		GLexTexture			   .PrepareForExport();
		GLexShader  		   .PrepareForExport();
		GLexSkinnedMeshRenderer.StaticPrepareForExport();
		GLexBone			   .PrepareForExport();
		// GLexAnimation		   .PrepareForExport();
		// GLexAnimationClip      .PrepareForExport();

		mGLexGameObjectsAsArray = mGLexGameObjects.ToArray();
		mGLexTopGameObjectsAsArray = mGLexTopGameObjects.ToArray();
		mGLexComponentsAsArray = mGLexComponents .ToArray();
		
		mScene.PrepareForExport();
	}
	
	public GLexGameObject[] GameObjects {
		get {
			return mGLexGameObjectsAsArray;
		}
	}

	public GLexGameObject[] TopGameObjects {
		get {
			return mGLexTopGameObjectsAsArray;
		}
	}

	public GLexComponent[] Components {
		get {
			return mGLexComponentsAsArray;
		}
	}
	
	public GLexMaterial[] Materials {
		get {
			return GLexMaterial.MaterialsAsArray;
		}
	}
	
	public GLexTexture[] Textures {
		get {
			return GLexTexture.TexturesAsArray;
		}
	}
	
	public GLexShader[] Shaders {
		get {
			return GLexShader.ShadersAsArray;
		}
	}
	
	public GLexMesh[] Meshes {
		get {
			return GLexMesh.MeshesAsArray;
		}
	}
	
	public GLexScene Scene {
		get {
			return mScene;
		}
		set {
			mScene = value;
		}
	}

	public GooProject Project {
		get {
			return this._project;
		}
		set {
			_project = value;
		}
	}
	
	public List<GLexAnimation> Animations {
		get {
			return GLexAnimation.Animations;
		}
	}
	
	public List<GLexAnimationState> AnimationStates {
		get {
			return GLexAnimationState.AllAnimationStates;
		}
	}
	
	public List<GLexAnimationClip> AnimationClips {
		get {
			return GLexAnimationClip.AllClips;
		}
	}
	
	public GLexSound[] SoundClips {
		get {
			return GLexAudioSource.ClipsAsArray;
		}
	}
	
	public GLexComponent GetConverterOfType(System.Type type) {
		foreach (GLexComponent converter in mGLexComponents) {
			if (converter.GetType() == type) {
				return converter;
			}
		}
		
		Debug.LogError("GLexData.GetConverterOfType: Tried to find converter of type " + type.ToString() + " but couldn't find it!");
		return null;
	}
}
