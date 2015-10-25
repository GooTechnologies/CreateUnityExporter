using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GLexAnimation : GLexComponent {
	private static List<GLexAnimation>		mAnimations;
	new protected Animation 		mComponent;
	private GameObject				mTarget;
	private List<GLexAnimationState> _states = new List<GLexAnimationState>();
	private string _layerId;

	public string LayerId {
		get {
			return this._layerId;
		}
	}
	
	public GLexAnimation() : base() {
		mAnimations.Add(this);
		
		_layerId = NamesUtil.GenerateUniqueId();
	}

	public static void Reset() {
		mAnimations = new List<GLexAnimation>();
	}
	
	// overrides
	
	public override void AssociateWithComponent(object obj) {
		base.AssociateWithComponent(obj);
		mComponent = (Animation)obj;
			
		//List<GLexAnimationClip> clips = new List<GLexAnimationClip>();
		foreach (AnimationState state in mComponent) {
			GLexAnimationClip clip;
				
			if (GLexAnimationClip.Exists(state.clip)) {
				clip = GLexAnimationClip.Get(state.clip);
			}
			else {
				clip = new GLexAnimationClip(this, state.clip);
			}
			
			//clips.Add(clip);
			_states.Add(new GLexAnimationState(clip));
		}

		if (mGameObject.GetComponent<Renderer>() != null) {
			mTarget = mGameObject.GetComponent<Renderer>().gameObject;
		}
		else {
			mTarget = mComponent.gameObject;
		}
	}
	
	protected override string IdExtension {
		get {
			return GLexConfig.GetExtensionFor("animation");
		}
	}
	

	// template interface starts here
	
	public bool IsAnimation {
		get { return true; }
	}
	
	public override string Name {
		get {
			return mGameObject.GetComponent<GLexGameObjectSettings>().UniqueName;
		}
	}
	
	public GameObject Target {
		get {
			return mTarget;
		}
	}
	
	public GooSkeleton Skeleton {
		get {
			if (mComponent != null) {
				return GooSkeleton.Get(mComponent.GetComponentInChildren<SkinnedMeshRenderer>());
			}
			return null;
		}
	}
	
	public GLexAnimationState DefaultState {
		get {
			return _states.Count > 0 ? _states[0] : null;
		}
	}
	
	public List<GLexAnimationState> States {
		get {
			return _states;
		}
	}
	
	public List<KeyValuePair<int, GLexAnimationState>> NumberedStates {
		get {
			return EnumerableUtil.Indexify(_states);
		}
	}
	
	// static
	
	public static List<GLexAnimation> Animations {
		get {
			return mAnimations;
		}
	}
}
