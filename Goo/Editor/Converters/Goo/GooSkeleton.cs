using UnityEngine;
using System.Collections.Generic;

public class GooSkeleton : GLexComponent {
	private static readonly Dictionary<SkinnedMeshRenderer, GooSkeleton> _allSkeletons = new Dictionary<SkinnedMeshRenderer, GooSkeleton>();
	
	public static IEnumerable<GooSkeleton> AllSkeletons {
		get {
			return _allSkeletons.Values;
		}
	}
	
	public static void Reset() {
		_allSkeletons.Clear();
	}
	
	public static GooSkeleton Get(SkinnedMeshRenderer pMeshRenderer) {
		if (pMeshRenderer == null) {
			return null;
		}
		
		GooSkeleton skel;
		if (!_allSkeletons.TryGetValue(pMeshRenderer, out skel)) {
			skel = new GooSkeleton(pMeshRenderer);
			_allSkeletons.Add(pMeshRenderer, skel);
		}
		return skel;
	}
	
	private SkinnedMeshRenderer _mesh;
	private List<GLexBone> _bones = new List<GLexBone>();
	
	private GooSkeleton(SkinnedMeshRenderer mesh) {
		_mesh = mesh;
		
		CreateBones();
	}
	
	public GLexBone FindBone(string pName) {
		var nameBits = pName.Split('/');
		
		foreach (var bone in _bones) {
			if (bone.Name == nameBits[nameBits.Length - 1]) {
				return bone;
			}
		}
		Debug.LogWarning("Can't find bone " + pName);
		return null;
	}
	
	private void CreateBones() {
		_bones.Clear();
		
		int index = 0;
		foreach (Transform bone in _mesh.bones) {
			var glexBone = new GLexBone(index++, bone, _mesh);
			_bones.Add(glexBone);

			if (!GLexConfig.ExportBoneAsGameObject) {
				GLexData.Instance.Remove(bone, bone.gameObject); 
			}
		}
		
		foreach (var bone in _bones) {
			bone.ParentIndex = GLexBone.GetParentIndexOf(bone);
		}
	}
	
	public int NumberOfBoneInfluences {
		get {
			if (_mesh.quality == SkinQuality.Auto) {
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
			else if (_mesh.quality == SkinQuality.Bone1) {
				return 1;
			}
			else if (_mesh.quality == SkinQuality.Bone2) {
				return 2;
			}
			else {
				return 4;
			}
		}
	}
	
	public List<GLexBone> Bones {
		get {
			return _bones;
		}
	}
	
	protected override string IdExtension {
		get {
			return GLexConfig.GetExtensionFor("skeleton");
		}
	}
}
