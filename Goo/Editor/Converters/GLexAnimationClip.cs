using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class GLexAnimationClip : GLexComponent {
	private static List<GLexAnimationClip> mClips;
	public static void Reset() {
		mClips = new List<GLexAnimationClip>();
	}
	
	private	GLexAnimation _animation;
	private AnimationClip _clip;
	private GooBinary _binary;
	
	private Dictionary<GLexBone, GooAnimationClipChannel> _boneChannels = new Dictionary<GLexBone, GooAnimationClipChannel>();
	
	public int KeyframeCount {
		get {
			return (int)(_clip.length / GLexConfig.AnimationBakeInterval) + 1;
		}
	}

	public GLexAnimation Animation {
		get {
			return this._animation;
		}
	}
	
	public GLexAnimationClip(GLexAnimation animation, AnimationClip clip) : base() {
		_animation = animation;
		_clip = clip;
		
		BuildCurveData();
		
		mClips.Add(this);
	}
	
	public float GetTimeAtKeyframe(int pFrame) {
		return pFrame * GLexConfig.AnimationBakeInterval;
	}

	
	public void AddToBinary(GooBinary binary) {
		_binary = binary;
		
		foreach (var kvp in _boneChannels) {
			kvp.Value.AddToBinary(_binary);
		}
	}
	
	private GooAnimationClipChannel GetChannelForBone(GLexBone pBone) {
		GooAnimationClipChannel channel;
		if (!_boneChannels.TryGetValue(pBone, out channel)) {
			channel = new GooAnimationClipChannel(this, pBone);
			_boneChannels.Add(pBone, channel);
		}
		return channel;
	}

	private void BuildCurveData() {
		var skeleton = _animation.Skeleton;
		if (skeleton == null) {
			Debug.LogError("Animation has no skeleton");
			return;
		}
		
		var curveData = AnimationUtility.GetAllCurves(_clip, true);
		foreach (var data in curveData) {
			var targetBone = skeleton.FindBone(data.path);
			if (targetBone == null) {
				Debug.LogWarning("Curve data refers to nonexistent bone " + data.path);
				continue;
			}
			
			var channel = GetChannelForBone(targetBone);
			channel.AddCurveData(data);
		}
	}
	
	public AnimationClip Clip {
		get {
			return _clip;
		}
	}
	
	protected override string IdExtension {
		get {
			return GLexConfig.GetExtensionFor("animationClip");
		}
	}

	
	// antlr interface starts here
	public IEnumerable<GooAnimationClipChannel> Channels {
		get {
			return _boneChannels.Values;
		}
	}

	public GooBinary Binary {
		get {
			return this._binary;
		}
	}
	
	// static 
	public static List<GLexAnimationClip> AllClips {
		get {
			return mClips;
		}
	}
	
	public static bool Exists(AnimationClip clip) {
		if (clip != null) {
			foreach (GLexAnimationClip glexClip in mClips) {
				if (glexClip.Clip == clip) {
					return true;
				}
			}
			return false;
		}
		else {
			return true;		// return true for null textures to avoid creation of empty GLexTexture
		}
	}
	
	public static GLexAnimationClip Get(AnimationClip clip) {
		if (Exists(clip)) {
			foreach (GLexAnimationClip glexClip in mClips) {
				if (glexClip.Clip == clip) {
					return glexClip;
				}
			}
		}
		else {
			Debug.LogError("GLexAnimationClip.Get: Trying to get " + clip.name + " that hasn't been added yet. Please check so it .Exists() first!");
		}
		return null;
	}	
}
