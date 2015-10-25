using UnityEngine;
using System.Collections;
using System;

public class GooBinarySubMeshProxy {
	private GooBinaryMesh _parentMesh;
	private int _subMeshIndex;
	
	public GooBinaryMesh ParentMesh {
		get {
			return this._parentMesh;
		}
	}

	public int SubMeshIndex {
		get {
			return this._subMeshIndex;
		}
	}
	
	public GooBinarySubMeshProxy(GooBinaryMesh mesh, int subMeshIndex) {
		_parentMesh = mesh;
		_subMeshIndex = subMeshIndex;
	}
	
	public GooBinaryPointer IndicesPtr {
		get {
			return _parentMesh.GetIndexPtr(_subMeshIndex);
		}
	}
	
	public int IndexCount {
		get {
			return _parentMesh.GetIndexCount(_subMeshIndex);
		}
	}
	
	public string Id {
		get {
			return _parentMesh.GlexMesh.GetSubMeshId(_subMeshIndex);
		}
	}
	
	// The rest of these just get values directly from the base mesh binary
	public int SkinningComponentCount {
		get {
			return _parentMesh.SkinningComponentCount;
		}
	}
	
	public string BinaryId {
		get {
			return _parentMesh.BinaryId;
		}
	}
	
	public bool HasVertexColors {
		get {
			return _parentMesh.HasVertexColors;
		}
	}
	
	public bool IsSkinned {
		get {
			return _parentMesh.IsSkinned;
		}
	}
	
	public string License {
		get {
			return _parentMesh.License;
		}
	}
	
	public DateTime ExportDate {
		get {
			return _parentMesh.ExportDate;
		}
	}
	
	public bool IsBinary {
		get {
			return _parentMesh.IsBinary;
		}
	}
	
	public int VertexCount {
		get {
			return _parentMesh.VertexCount;
		}
	}
	
	public GooBinaryPointer PositionsPtr {
		get {
			return _parentMesh.PositionsPtr;
		}
	}
	
	public GooBinaryPointer NormalsPtr {
		get {
			return _parentMesh.NormalsPtr;
		}
	}
	
	public GooBinaryPointer ColorsPtr {
		get {
			return _parentMesh.ColorsPtr;
		}
	}
	
	public GooBinaryPointer TexCoords0Ptr {
		get {
			return _parentMesh.TexCoords0Ptr;
		}
	}
	
	public GooBinaryPointer TexCoords1Ptr {
		get {
			return _parentMesh.TexCoords1Ptr;
		}
	}
	
	public GooBinaryPointer BoneIndicesPtr {
		get {
			return _parentMesh.BoneIndicesPtr;
		}
	}
	
	public GooBinaryPointer BoneWeightsPtr {
		get {
			return _parentMesh.BoneWeightsPtr;
		}
	}
	
	public Bounds BoundingBox {
		get {
			return _parentMesh.BoundingBox;
		}
	}
}
