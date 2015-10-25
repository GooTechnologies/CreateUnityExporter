using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
public class GooCreate : MonoBehaviour {
	
	private	static	GooCreate	mInstance;
	
	public string	username = "";
	public string 	importToken = "";
	public string 	projectId = "";
	public bool 	uploadToCreate = true;
	
	public void OnEnable() {
		mInstance = this;
	}
	
	public void Awake() {
		mInstance = this;
	}	
	
	private static bool CheckForInstance() {
		if (mInstance == null) {
			Debug.LogError( "GooCreate: Please add the GooCreate component to one of your GameObjects to be able to export" );
			return false;
		}
		return true;
	}
	
	public static string UserName {
		get {
			return CheckForInstance() ? mInstance.username : string.Empty;
		}
	}
	
	public static string ImportToken {
		get {
			return CheckForInstance() ? mInstance.importToken : string.Empty;
		}
	}
	
	public static string ProjectID {
		get {
			return CheckForInstance() ? mInstance.projectId : string.Empty;
		}
	}
	
	public static bool UploadToCreate {
		get {
			return CheckForInstance() ? mInstance.uploadToCreate : false;
		}
	}
}
