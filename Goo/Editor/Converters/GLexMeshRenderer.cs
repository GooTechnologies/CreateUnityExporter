using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GLexMeshRenderer : GLexComponent {
	new protected MeshRenderer mComponent;
	private GLexMaterial _material;
	private GLexMesh _mesh;
	private int _submeshIndex;
	
	public GLexMeshRenderer() : base() {
	}
	
	// overrides
	
	public override void AssociateWithComponent(object obj) {
		base.AssociateWithComponent(obj);
		mComponent = (MeshRenderer)obj;
			
		var meshFilter = mComponent.gameObject.GetComponent<MeshFilter>();
		if (meshFilter != null) {
			Vector3 scale = meshFilter.transform.lossyScale;
			_mesh = GLexMesh.Get(meshFilter.sharedMesh);
			_mesh.Scale = scale;
		}
			
		_material = GLexMaterial.Get(mComponent.sharedMaterials[0], mComponent);
		_submeshIndex = 0;
	}
	
	public override void PrepareForExport() {
		base.PrepareForExport();
		
		if (_submeshIndex == 0 && _mesh.SubMeshCount > 1) {
			for (int i = 1; i < _mesh.SubMeshCount; ++i) {
				var dummyObject = new GameObject("Submesh#" + i);
				dummyObject.transform.parent = mComponent.transform;
					
				var submeshObject = new GLexGameObject(dummyObject, _glexGameObject.GlexData);
				_glexGameObject.GlexData.AddGameObject(submeshObject);
				
				var submeshRenderer = new GLexMeshRenderer(this, i);
				submeshObject.AddComponent(submeshRenderer);
				_glexGameObject.GlexData.AddComponent(submeshRenderer);
				
				GooExporter.RemoveAfterExport(dummyObject);
			}
		}
	}
	
	private GLexMeshRenderer(GLexMeshRenderer parentRenderer, int subMesh) {
		mComponent = parentRenderer.mComponent;
		_mesh = parentRenderer._mesh;
		_submeshIndex = subMesh;
		_material = GLexMaterial.Get(parentRenderer.mComponent.sharedMaterials[subMesh], mComponent);
	}

	// template interface starts here
	
	public bool IsMeshRenderer {
		get {
			return true;
		}
	}
	
	public bool IsMeshFilter {
		get {
			return true;
		}
	}
	
	public string MeshId {
		get {
			return _mesh.GetSubMeshId(_submeshIndex);
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
}
