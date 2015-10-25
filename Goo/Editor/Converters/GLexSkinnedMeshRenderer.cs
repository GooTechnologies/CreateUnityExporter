using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GLexSkinnedMeshRenderer : GLexComponent {
	
	private static	List<GLexSkinnedMeshRenderer>		mSkinnedMeshRenderers;
	private	static	GLexSkinnedMeshRenderer[]			mSkinnedMeshRenderersAsArray;
	new protected 	SkinnedMeshRenderer					mComponent;
	protected 		GLexMesh							mMesh;
	protected		GLexBone[]							mBones;
	protected		GLexBone							mRootBone;
	private GLexMaterial _material;
	private int _submeshIndex;
	
	
	public GLexSkinnedMeshRenderer() : base() {
		mSkinnedMeshRenderers.Add(this);
	}

	public static void Reset() {
		mSkinnedMeshRenderers = new List<GLexSkinnedMeshRenderer>();
	}
	
	public static void StaticPrepareForExport() {
		mSkinnedMeshRenderersAsArray = mSkinnedMeshRenderers.ToArray();
	}
	
	// overrides
	public override void AssociateWithComponent(object obj) {
		base.AssociateWithComponent(obj);
		mComponent = (SkinnedMeshRenderer)obj;
		
		_material = GLexMaterial.Get(mComponent.sharedMaterials[0], mComponent);
		_submeshIndex = 0;
		
		mMesh = GLexMesh.Get(mComponent.sharedMesh);
	}
	
	public override void PrepareForExport() {
		base.PrepareForExport();
		
		if (_submeshIndex == 0 && mMesh.SubMeshCount > 1) {
			for (int i = 1; i < mMesh.SubMeshCount; ++i) {
				var dummyObject = new GameObject("Submesh#" + i);
				dummyObject.transform.parent = mComponent.transform;
				
				var submeshObject = new GLexGameObject(dummyObject, _glexGameObject.GlexData);
				_glexGameObject.GlexData.AddGameObject(submeshObject);
				
				var submeshRenderer = (GLexSkinnedMeshRenderer)System.Activator.CreateInstance(GetType());
				submeshRenderer.SetupSubmeshRenderer(this, i);
				submeshObject.AddComponent(submeshRenderer);
				_glexGameObject.GlexData.AddComponent(submeshRenderer);
				
				GooExporter.RemoveAfterExport(dummyObject);
			}
		}
	}
	
	private void SetupSubmeshRenderer(GLexSkinnedMeshRenderer parent, int submeshIndex) {
		mComponent = parent.mComponent;
		mMesh = parent.mMesh;
		_submeshIndex = submeshIndex;
		_material = GLexMaterial.Get(parent.mComponent.sharedMaterials[submeshIndex], mComponent);
	}
	
	// template interface starts here
	public bool IsSkinnedMeshRenderer {
		get {
			return true;
		}
	}
	
	public string MeshId {
		get {
			return mMesh.GetSubMeshId(_submeshIndex);
		}
	}
	
	public GLexMaterial Material {
		get {
			return _material;
		}
	}
	
	public bool CastShadows {
		get {
			return mComponent.shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.On;
		}
	}
	
	public bool ReceiveShadows {
		get {
			return mComponent.receiveShadows;
		}
	}
	
	public string CullMode {
		get {
			return mComponent.isPartOfStaticBatch ? "Static" : "Dynamic";
		}
	}
	
	public bool UpdateOffScreen {
		get {
			return mComponent.updateWhenOffscreen;
		}
	}

	
	// static
	
	public static GLexSkinnedMeshRenderer[] SkinnedMeshesAsArray {
		get {
			return mSkinnedMeshRenderersAsArray;
		}
	}
}
