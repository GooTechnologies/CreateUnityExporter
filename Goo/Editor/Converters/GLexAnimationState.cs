using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GLexAnimationState : GLexComponent {
	private static List<GLexAnimationState>	_allAnimationStates = new List<GLexAnimationState>();
	
	private GLexAnimationClip _clip;
	
	public static void Reset() {
		_allAnimationStates.Clear();
	}
	
	public GLexAnimationState(GLexAnimationClip pClip) {
		_clip = pClip;
		_allAnimationStates.Add(this);
	}

	protected override string IdExtension {
		get {
			return GLexConfig.GetExtensionFor("animationState");
		}
	}
	
	public static List<GLexAnimationState> AllAnimationStates {
		get {
			return _allAnimationStates;
		}
	}

	public GLexAnimationClip Clip {
		get {
			return this._clip;
		}
	}
}
