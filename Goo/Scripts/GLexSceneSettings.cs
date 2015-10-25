using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class GLexSceneSettings : MonoBehaviour {
	
	private static GLexSceneSettings	mInstance;
	
	public string 						customFileName = "";
	public string[]						exportPaths;
	
	private Hashtable					mUniqueNames;
	private int							mUniqueNameCounter;
	
	public void SetAsCurrent() {
		mInstance = this;
	}
	
	public void Update() {
		transform.localPosition = Vector3.zero;
		transform.eulerAngles   = Vector3.zero;
		transform.localScale    = Vector3.one;
	}

	public static string FileName {
		get {
			if( mInstance == null || string.IsNullOrEmpty( mInstance.customFileName )) {
				return Application.loadedLevelName;
			} else {
				return mInstance.customFileName;
			}
		}
	}
	
	public static string[] ExportPaths {
		get {
			if( mInstance == null || mInstance.exportPaths == null ) {
				return new string[0];
			} else {
				return mInstance.exportPaths;
			}
		}
	}
}
