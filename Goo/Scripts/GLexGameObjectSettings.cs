using UnityEngine;
using System.Collections;

public class GLexGameObjectSettings : MonoBehaviour {
	public enum BillboardType {
		none,
		faceCamera,
		faceCameraAndRotate,
		zAxisOnly,
		xAxisOnly,
		yAxisOnly
	}
	
	public 	bool			export = true;
	public  string			tags = "";
	public 	BillboardType	isBillboard = BillboardType.none;
	
	private string 		mUniqueName = string.Empty;
	
	public string UniqueName {
		get {
			return mUniqueName;
		}
		set {
			mUniqueName = value;
		}
	}
}

