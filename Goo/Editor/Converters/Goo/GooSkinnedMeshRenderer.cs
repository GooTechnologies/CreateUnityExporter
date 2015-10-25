using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GooSkinnedMeshRenderer : GLexSkinnedMeshRenderer {
	private GooSkeleton _skeleton;
	
	public GooSkinnedMeshRenderer() : base() {
	}
	
	public override void AssociateWithComponent(object obj) {
		base.AssociateWithComponent(obj);
		_skeleton = GooSkeleton.Get((SkinnedMeshRenderer)obj);
	}
	
	public bool IsMeshFilter {
		get {
			return true;
		}
	}
	
	public bool IsMeshRenderer {
		get {
			return true;
		}
	}
	
	public override string Name {
		get {
			return mMesh.Name;
		}
	}
			
	public GooSkeleton Skeleton {
		get {
			return _skeleton;
		}
	}
}
