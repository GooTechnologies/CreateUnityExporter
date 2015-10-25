using UnityEngine;
using System.Collections;

public class GLexTransform : GLexComponent {
	
	new protected Transform mComponent;
	
	public GLexTransform() : base() {
	}

	// overrides
	
	public override void AssociateWithComponent(object obj) {
		base.AssociateWithComponent(obj);
		mComponent = (Transform)obj;
	}
	
	// template interface starts here
	
	public bool IsTransform {
		get { return true; }
	}
	
	public Vector3 Position {
		get {
			return Translation;
		}
	}
	
	public Vector3 Translation {
		get {
			Vector3 p;
			if (!GLexConfig.CameraCanHaveParent && mGameObject.GetComponent<Camera>() != null) {
				p = mComponent.position;
			}
			else {
				p = mComponent.localPosition;
			}
			p.z = -p.z;
			return p;
		}
	}
	
	public Vector3 RotationAsEuler {
		get {
			var euler = mComponent.localEulerAngles;
			euler.x = -euler.x;
			euler.y = -euler.y;
			return euler;
		}
	}
	
	/*
	public string[] RotationAsQuaternion {
		get {
			Quaternion q;
			if (!GLexConfig.CameraCanHaveParent && mGameObject.GetComponent<Camera>() != null) {
				q = mComponent.rotation;
			}
			else {
				q = mComponent.localRotation;
			}
			
			return new string[] {
				GLexConfig.TransformValue(q.x, "rx").ToString(GLexConfig.HighPrecision),
				GLexConfig.TransformValue(q.y, "ry").ToString(GLexConfig.HighPrecision),
				GLexConfig.TransformValue(q.z, "rz").ToString(GLexConfig.HighPrecision),
				GLexConfig.TransformValue(q.w, "rw").ToString(GLexConfig.HighPrecision)  
			};
		}
	}
	*/
	
	public Vector3 Scale {
		get {
			return mComponent.localScale;
		}
	}
}
