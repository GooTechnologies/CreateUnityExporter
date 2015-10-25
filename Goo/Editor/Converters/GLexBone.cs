using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GLexBone {

	private static	List<GLexBone>		mBones;
	private static 	GLexBone[]			mBonesAsArray;
	
	private string				_id;
	private int					mIndex;
	private int 				mParentIndex;
	private Transform 			mTransform;
	private Matrix4x4			mBindPose;
	private SkinnedMeshRenderer	mRenderer;
	
	public string Id {
		get {
			return this._id;
		}
	}

	public Transform Transform {
		get {
			return this.mTransform;
		}
	}

	public Matrix4x4 BindPose {
		get {
			return this.mBindPose;
		}
	}
	
	public GLexBone( int index, Transform transform, SkinnedMeshRenderer renderer ) {
		_id = NamesUtil.GenerateUniqueId();
		
		mIndex     = index;
		mTransform = transform; 
		mRenderer  = renderer;
		mBindPose  = renderer.sharedMesh.bindposes[ mIndex ];
		
		mBones.Add( this );
	}
	
	public static void Reset() {
		mBones = new List<GLexBone>();
	}
	
	public static void PrepareForExport() {
		mBonesAsArray = mBones.ToArray();
	}
	
	public static int GetParentIndexOf( GLexBone bone ) {
		if( bone.mTransform.parent != null ) {
			foreach( GLexBone parent in mBones ) {
				if( bone.mTransform.parent == parent.mTransform ) {
					return parent.Index;
				}
			}
			return -32768;
		} else {
			return -32768;
		}
	}

	// antlr interface starts here
	
	public int Index {
		get {
			return mIndex;
		}
	}
	
	public int ParentIndex {
		get {
			return mParentIndex;
		}
		set {
			mParentIndex = value;
		}
	}
	
	public Vector3 Position {
		get {
			return Translation;
		}
	}
	
	public Vector3 Translation {
		get {
			Vector3 p = mTransform.localPosition;
			p.z = -p.z;
			return p;
		}
	}
	
	public Vector3 RotationAsEuler {
		get {
			Vector3 r = mTransform.localEulerAngles;
			r.z = -r.z;
			return r;
		}
	}
	
	public Quaternion RotationAsQuaternion {
		get {
			Quaternion q = mTransform.localRotation;
			q.z = -q.z;
			q.w = -q.w;
			return q;
		}
	}
	
	public Vector3 Scale {
		get {
			return mTransform.localScale;
		}
	}
			
			
	public Matrix4x4 InverseBindPoseMatrix {
		get {
			// Remember to flip Z axis
			var mx = mBindPose;
			mx[2, 3] = -mx[2, 3];
			return mx;
		}
	}
	
	
	public string Name {
		get {
			return mTransform.name; // mTransform.GetComponent<GLexGameObjectSettings>().UniqueName;
		}
	}
	
	public string ParentName {
		get {
			if( mTransform.parent != null && mTransform.parent.GetComponent<GLexGameObjectSettings>() != null ) {
				return mTransform.parent.GetComponent<GLexGameObjectSettings>().UniqueName;
			} else {
				return "noParentPleaseCheckIsRootBone";
			}
		}
	}
	
	public bool IsRootBone {
		get {
			return mTransform == mRenderer.rootBone;
		}
	}
	
	// static
	
	public static GLexBone[] BonesAsArray {
		get {
			return mBonesAsArray;
		}
	}
}
