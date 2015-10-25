using UnityEngine;
using System.Collections;

public class GLexCamera : GLexComponent {
	new protected 	Camera		mComponent;
	private GooSkybox _skybox;
	
	// overrides
	
	public override void AssociateWithComponent(object obj) {
		base.AssociateWithComponent(obj);
		mComponent = (Camera)obj;
			
		if (mComponent.clearFlags == CameraClearFlags.Skybox) {
			var skyboxComponent = mComponent.gameObject.GetComponent<Skybox>();
			if (skyboxComponent != null) {
				_skybox = new GooSkybox(skyboxComponent);
			}
		}
	}
	
	// antlr interface starts here
	
	public bool IsCamera {
		get {
			return true;
		}
	}

	public Camera UnityCamera {
		get {
			return this.mComponent;
		}
	}
	
	public bool IsPerspective {
		get {
			return !mComponent.orthographic;
		}
	}
	
	public float AspectRatio {
		get {
			return mComponent.aspect;
		}
	}
	
	public string ProjectionMode {
		get {
			return mComponent.orthographic ? "Parallel" : "Perspective";
		}
	}
	
	public float OrthographicSize {
		get {
			return mComponent.orthographicSize;
		}
	}
	
	public string FOV {
		get {
			return mComponent.fieldOfView.ToString();
		}
	}
	
	public string Near {
		get {
			return mComponent.nearClipPlane.ToString();
		}
	}
	
	public string Far {
		get {
			return mComponent.farClipPlane.ToString();
		}
	}
	
	public GooSkybox Skybox {
		get {
			return _skybox;
		}
	}
}
