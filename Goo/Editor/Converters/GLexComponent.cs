using UnityEngine;
using System.Collections;
using System;

public abstract class GLexComponent {
	private			string			mKeystring;
	protected 		GameObject 		mGameObject;
	protected 		System.Object	mComponent;
	
	protected GLexGameObject _glexGameObject;

	public GLexGameObject GlexGameObject {
		get {
			return this._glexGameObject;
		}
		set {
			_glexGameObject = value;
		}
	}	
	
	public GLexComponent() {
		mKeystring = NamesUtil.GenerateUniqueId();
	}
	
	public void AssociateWithGameObject(GameObject obj) {
		mGameObject = obj;
	}

	public virtual void AssociateWithComponent(object obj) {
		mComponent = obj;	
	}
	
	public virtual void PrepareForExport() {
	}
	
	// template interface starts here
	
	public virtual string Name {
		get {
			return Id;
		}
	}
	
	protected virtual string IdExtension {
		get {
			return string.Empty;
		}
	}
	
	public string Id {
		get {
			return mKeystring + IdExtension;
		}
	}
	
	public string License {
		get {
			return "PRIVATE";
		}
	}
	
	public DateTime ExportDate {
		get {
			return DateTime.Now;
			//return System.DateTime.Now.ToString(GLexConfig.TimestampFormat);
		}
	}
	
	public bool HasParent {
		get {
			if( !GLexConfig.CameraCanHaveParent && mGameObject.GetComponent<Camera>() != null ) {
				return false;
			}
			
			if( !GLexConfig.ExportSceneGameObject && mGameObject.transform.parent != null && mGameObject.transform.parent.GetComponent<GLexSceneSettings>() != null ) {
				return false;
			}
			
			return mGameObject.transform.parent.GetComponent<GLexGameObjectSettings>() != null;
		}
	}
	
	public string ParentId {
		get {
			if( HasParent ) {
				return mGameObject.transform.parent.GetComponent<GLexGameObjectSettings>().UniqueName;
			} else {
				Debug.LogWarning( "Converter.ParentName: No parent available, please check HasParent before printing parent name!" );
				return string.Empty;
			}
		}
	}
	
	public string AssociatedGameObject {
		get {
			return mGameObject.GetComponent<GLexGameObjectSettings>().UniqueName;
		}
	}
	
	public System.Object Component {
		get {
			return mComponent;
		}
	}
	
	public GLexGameObject Entity {
		get {
			return GlexGameObject;
		}
	}
}
