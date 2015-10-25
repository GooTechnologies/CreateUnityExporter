using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class GooAnimationClipChannel {
	private GLexAnimationClip _clip;
	private GLexBone _bone;
	private string _id;
	
	// keyframes
	private Vector3[] _translationKeyframes;
	Quaternion[] _rotationKeyframes;
	private Vector3[] _scaleKeyframes;
	
	// binary
	private GooBinaryRange _timesRange;
	private GooBinaryRange _transRange;
	private GooBinaryRange _rotRange;
	private GooBinaryRange _scaleRange;
	
	private bool _needsPostprocess = true;
	
	public GooAnimationClipChannel(GLexAnimationClip pClip, GLexBone pBone) {
		_clip = pClip;
		_bone = pBone;
		_id = NamesUtil.GenerateUniqueId();
		
		ResetCurveData();
	}
	
	private void PostprocessCurveData() {
		if (!_needsPostprocess) {
			return;
		}
		_needsPostprocess = false;
		
		for (int i = 0; i < _clip.KeyframeCount; ++i) {
			_translationKeyframes[i].z *= -1;
			if (!_bone.IsRootBone) {
				_translationKeyframes[i] = _bone.Translation + _translationKeyframes[i];
			}
			
			_rotationKeyframes[i].z *= -1;
			_rotationKeyframes[i].w *= -1;
			_rotationKeyframes[i] = _bone.RotationAsQuaternion * _rotationKeyframes[i];
			
			_scaleKeyframes[i].x *= _bone.Transform.localScale.x;
			_scaleKeyframes[i].y *= _bone.Transform.localScale.y;
			_scaleKeyframes[i].z *= _bone.Transform.localScale.z;
		}
	}
	
	public void AddToBinary(GooBinary pBinary) {
		PostprocessCurveData();
		
		_transRange = pBinary.Add(_translationKeyframes);
		_scaleRange = pBinary.Add(_scaleKeyframes);
		
		_rotRange = pBinary.AllocateRange(sizeof(float) * 4 * _rotationKeyframes.Length);
		foreach (var rotQuat in _rotationKeyframes) {
			_rotRange.Write(rotQuat[0]);
			_rotRange.Write(rotQuat[1]);
			_rotRange.Write(rotQuat[2]);
			_rotRange.Write(rotQuat[3]);
		}
		
		_timesRange = pBinary.AllocateRange(sizeof(float) * _clip.KeyframeCount);
		for (int i = 0; i < _clip.KeyframeCount; ++i) {
			_timesRange.Write(_clip.GetTimeAtKeyframe(i));
		}
	}
	
	private void ResetCurveData() {
		_translationKeyframes = new Vector3[_clip.KeyframeCount];
		_rotationKeyframes = new Quaternion[_clip.KeyframeCount];
		_scaleKeyframes = new Vector3[_clip.KeyframeCount];
		
		for (int i = 0; i < _clip.KeyframeCount; ++i) {
			_translationKeyframes[i] = Vector3.zero;
			_rotationKeyframes[i] = Quaternion.identity;
			_scaleKeyframes[i] = Vector3.one;
		}
		
		_needsPostprocess = true;
	}
	
	public void AddCurveData(AnimationClipCurveData pData) {
		_needsPostprocess = true;
		
		var targetPropertyBits = pData.propertyName.Split('.');
		var target = targetPropertyBits[0];
		int componentIdx = "xyzw".IndexOf(targetPropertyBits[1][0]);
		
		//var skeleton = _clip.Animation.Skeleton;
		//skeleton.
		
		
		
		// Figure out what this curve targets in terms of Goo data
		if (target == "m_LocalPosition") {
			for (int i = 0; i < _clip.KeyframeCount; ++i) {
				_translationKeyframes[i][componentIdx] = pData.curve.Evaluate(_clip.GetTimeAtKeyframe(i));
			}
		}
		else if (target == "m_LocalRotation") {
			for (int i = 0; i < _clip.KeyframeCount; ++i) {
				_rotationKeyframes[i][componentIdx] = pData.curve.Evaluate(_clip.GetTimeAtKeyframe(i));
			}
		}
		else if (target == "m_LocalScale") {
			for (int i = 0; i < _clip.KeyframeCount; ++i) {
				_scaleKeyframes[i][componentIdx] = pData.curve.Evaluate(_clip.GetTimeAtKeyframe(i));
			}
		}
		else {
			Debug.LogError("Curve describes a property we don't understand: " + target);
		}

	}

	public string Id {
		get {
			return this._id + IdExtension;
		}
	}

	public string IdExtension {
		get {
			return GLexConfig.GetExtensionFor("animationChannel"); 
		}
	}
	public GLexBone Bone {
		get {
			return this._bone;
		}
	}
	
	public GooBinaryPointer TimesRef {
		get {
			return new GooBinaryPointer(_timesRange, typeof(float));
		}
	}
	
	public GooBinaryPointer TranslationSamplesRef {
		get {
			return new GooBinaryPointer(_transRange, typeof(float));
		}
	}
	
	public GooBinaryPointer RotationSamplesRef {
		get {
			return new GooBinaryPointer(_rotRange, typeof(float));
		}
	}
	
	public GooBinaryPointer ScaleSamplesRef {
		get {
			return new GooBinaryPointer(_scaleRange, typeof(float));
		}
	}
}
