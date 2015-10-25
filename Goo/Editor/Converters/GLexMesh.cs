using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GLexMesh : GLexComponent {

	protected static List<GLexMesh>		mMeshes;
	protected static GLexMesh[]			mMeshAsArray;
	protected Mesh		mMesh;
	protected Vector3	mCenter;
	private Vector3 mScale;


	private string[] _submeshIds;
	
	public GLexMesh(Mesh mesh) : base() {
		mMesh = mesh;
		mMeshes.Add(this);
		
		_submeshIds = new string[mMesh.subMeshCount];
		for (int i = 0; i < _submeshIds.Length; ++i) {
			_submeshIds[i] = NamesUtil.GenerateUniqueId() + IdExtension;
		}
	}
	
	public static void Reset() {
		mMeshes = new List<GLexMesh>();
	}
	
	new public static void PrepareForExport() {
		mMeshAsArray = mMeshes.ToArray();
	}
	
	public Mesh Mesh {
		get {
			return mMesh;
		}
	}
	
	public int SubMeshCount {
		get {
			return mMesh.subMeshCount;
		}
	}
	
	public string GetSubMeshId(int submesh) {
		return _submeshIds[submesh];
	}
	
	protected override string IdExtension {
		get {
			return GLexConfig.GetExtensionFor("mesh");
		}
	}

	// antlr interface starts here
	
	public override string Name {
		get { return mMesh.name; }
	}
	
	public string IndexCount {
		get { return mMesh.triangles.Length.ToString(); }
	}

	public int FacesCount {
		get { return mMesh.triangles.Length / 3; }
	}

	public string VertexCount {
		get { return mMesh.vertexCount.ToString(); }
	}
	
	public bool HasVertexColors {
		get {
			return mMesh.colors.Length > 0;
		}
	}
	public bool IsSkinned {
		get {
			return mMesh.bindposes.Length > 0;
		}
	}
	
	public int NumberOfBoneInfluences {
		get {
			if (QualitySettings.blendWeights == BlendWeights.OneBone) {
				return 1;
			}
			else if (QualitySettings.blendWeights == BlendWeights.TwoBones) {
				return 2;
			}
			else {
				return 4;
			}
		}
	}
	
	// static

	public static GLexMesh[] MeshesAsArray {
		get {
			return mMeshAsArray;
		}
	}

	public Vector3 Scale {
		get {
			return mScale;
		}
		set {
			mScale = value;
		}
	}

	public static bool Exists(string name) {
		foreach (GLexMesh mesh in mMeshes) {
			if (mesh.Name == name) {
				return true;
			}
		}
		return false;
	}
	
	public static GLexMesh Get(Mesh nativeMesh) {
		foreach (GLexMesh mesh in mMeshes) {
			if (mesh.Mesh == nativeMesh) {
				return mesh;
			}
		}
		
		return new GLexMesh(nativeMesh);
	}
}
