using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GooBinaryMesh {
	private GooBinaryRange[] _indexRanges;
	private GooBinaryRange _vtxPositionsRange;
	private GooBinaryRange _vtxColorsRange;
	private GooBinaryRange _vtxNormalsRange;
	private GooBinaryRange _vtxUV0Range;
	private GooBinaryRange _vtxUV1Range;
	private GooBinaryRange _vtxBoneIndicesRange;
	private GooBinaryRange _vtxBoneWeightsRange;
	
	private GLexMesh		mGlexMesh;
	private Mesh			mMesh;
	private GooBinary		mBinary;
	
	private GooBinarySubMeshProxy[] _submeshProxies;
	
	public string Name {
		get {
			return mGlexMesh.Name;
		}
	}

	public GLexMesh GlexMesh {
		get {
			return this.mGlexMesh;
		}
	}
	
	public GooBinaryMesh(GLexMesh glexMesh) {
		mGlexMesh = glexMesh;
		mMesh = mGlexMesh.Mesh;
		
		_submeshProxies = new GooBinarySubMeshProxy[mMesh.subMeshCount];
		for (int i = 0; i < _submeshProxies.Length; ++i) {
			_submeshProxies[i] = new GooBinarySubMeshProxy(this, i);
		}
	}
	
	public void AddToBinary(GooBinary binary) {
		mBinary = binary;
		
		SaveVertexBasics();
		if (HasVertexColors) {
			SaveVertexColors();
		}
		if (IsSkinned) {
			SaveSkinningData();
		}
		
		SaveIndices();
	}
	
	private void SaveIndices() {
		_indexRanges = new GooBinaryRange[mMesh.subMeshCount];
		
		for (int submeshIdx = 0; submeshIdx < mMesh.subMeshCount; ++submeshIdx) {
			var indices = mMesh.GetTriangles(submeshIdx);
			
			var range = mBinary.AllocateRange(indices.Length * sizeof(ushort));
			for (int i = 0; i < indices.Length; i += 3) {
				// Note reversed order
				range.Write((ushort)indices[i]);
				range.Write((ushort)indices[i + 2]);
				range.Write((ushort)indices[i + 1]);
			}
			
			_indexRanges[submeshIdx] = range;
		}
	}
	
	private void SaveVertexBasics() {
		_vtxPositionsRange = SaveGeometry(mMesh.vertices);
		_vtxNormalsRange = SaveGeometry(mMesh.normals);
		_vtxUV0Range = SaveUVs(mMesh.uv);
		_vtxUV1Range = SaveUVs(mMesh.uv2);
	}
	
	private GooBinaryRange SaveGeometry(Vector3[] pGeometry) {
		var range = mBinary.AllocateRange(pGeometry.Length * sizeof(float) * 3);
		for (int i = 0; i < mMesh.vertices.Length; ++i) {
			range.Write(pGeometry[i].x);
			range.Write(pGeometry[i].y);
			range.Write(-pGeometry[i].z); // Note inverted
		}
		return range;
	}
	
	private GooBinaryRange SaveUVs(Vector2[] pUVs) {
		var range = mBinary.AllocateRange(pUVs.Length * sizeof(float) * 2);
		for (int i = 0; i < pUVs.Length; ++i) {
			range.Write(pUVs[i].x);
			range.Write(pUVs[i].y);
		}
		return range;
	}
	
	private void SaveVertexColors() {
		_vtxColorsRange = mBinary.AllocateRange(mMesh.colors.Length * sizeof(float) * 4);
		foreach (var color in mMesh.colors) {
			
			// TEMP HACK
			_vtxColorsRange.Write(1.0f); _vtxColorsRange.Write(1.0f); _vtxColorsRange.Write(1.0f); _vtxColorsRange.Write(1.0f);
			
			/*
			_vtxColorsRange.Write(color.r);
			_vtxColorsRange.Write(color.g);
			_vtxColorsRange.Write(color.b);
			_vtxColorsRange.Write(color.a);
			*/
		}
	}
	
	private void SaveSkinningData() {
		int weightCount = SkinningComponentCount;
		
		_vtxBoneWeightsRange = mBinary.AllocateRange(mMesh.boneWeights.Length * weightCount * sizeof(float));
		_vtxBoneIndicesRange = mBinary.AllocateRange(mMesh.boneWeights.Length * weightCount * sizeof(byte));
		
		foreach (var weight in mMesh.boneWeights) {
			_vtxBoneWeightsRange.Write(weight.weight0);
			_vtxBoneIndicesRange.Write((byte)weight.boneIndex0);
			
			if (weightCount >= 2) {
				_vtxBoneWeightsRange.Write(weight.weight1);
				_vtxBoneIndicesRange.Write((byte)weight.boneIndex1);
				
				if (weightCount >= 4) {
					_vtxBoneWeightsRange.Write(weight.weight2);
					_vtxBoneWeightsRange.Write(weight.weight3);
					_vtxBoneIndicesRange.Write((byte)weight.boneIndex2);
					_vtxBoneIndicesRange.Write((byte)weight.boneIndex3);
				}
			}
		}
	}
	
	public int SkinningComponentCount {
		get {
			return QualitySettings.blendWeights == BlendWeights.OneBone ? 1 : QualitySettings.blendWeights == BlendWeights.TwoBones ? 2 : 4;
		}
	}
	
	public string BinaryId {
		get {
			return mBinary.BinaryId;
		}
	}
	
	public bool HasVertexColors {
		get {
			return mGlexMesh.HasVertexColors;
		}
	}
	
	public bool IsSkinned {
		get {
			return mGlexMesh.IsSkinned;
		}
	}
	
	public string License {
		get {
			return mGlexMesh.License;
		}
	}
	
	public DateTime ExportDate {
		get {
			return mGlexMesh.ExportDate;
		}
	}
	
	public bool IsBinary {
		get {
			return true;
		}
	}
	
	public int SubMeshCount {
		get {
			return mMesh.subMeshCount;
		}
	}
	
	public GooBinarySubMeshProxy[] SubMeshes {
		get {
			return _submeshProxies;
		}
	}
	
	public int GetIndexCount(int pSubMesh) {
		return mMesh.GetTriangles(pSubMesh).Length;
	}
	
	public int VertexCount {
		get {
			return mMesh.vertices.Length;
		}
	}
	
	public GooBinaryPointer GetIndexPtr(int pSubMesh) {
		if (_indexRanges == null || pSubMesh < 0 || pSubMesh >= _indexRanges.Length) {
			return null;
		}
		return new GooBinaryPointer(_indexRanges[pSubMesh], typeof(ushort));
	}
	
	public GooBinaryPointer PositionsPtr {
		get {
			return _vtxPositionsRange == null ? null : new GooBinaryPointer(_vtxPositionsRange, typeof(float));
		}
	}
	
	public GooBinaryPointer NormalsPtr {
		get {
			return _vtxNormalsRange == null ? null : new GooBinaryPointer(_vtxNormalsRange, typeof(float));
		}
	}
	
	public GooBinaryPointer ColorsPtr {
		get {
			return _vtxColorsRange == null ? null : new GooBinaryPointer(_vtxColorsRange, typeof(float));
		}
	}
	
	public GooBinaryPointer TexCoords0Ptr {
		get {
			return _vtxUV0Range == null ? null : new GooBinaryPointer(_vtxUV0Range, typeof(float));
		}
	}
	
	public GooBinaryPointer TexCoords1Ptr {
		get {
			return _vtxUV1Range == null ? null : new GooBinaryPointer(_vtxUV1Range, typeof(float));
		}
	}
	
	public GooBinaryPointer BoneIndicesPtr {
		get {
			return _vtxBoneIndicesRange == null ? null : new GooBinaryPointer(_vtxBoneIndicesRange, typeof(byte));
		}
	}
	
	public GooBinaryPointer BoneWeightsPtr {
		get {
			return _vtxBoneWeightsRange == null ? null : new GooBinaryPointer(_vtxBoneWeightsRange, typeof(float));
		}
	}
	
	public Bounds BoundingBox {
		get {
			Bounds bounds = mMesh.bounds;
			bounds.Expand(mGlexMesh.Scale - Vector3.one);
			return bounds;
		}
	}
}
