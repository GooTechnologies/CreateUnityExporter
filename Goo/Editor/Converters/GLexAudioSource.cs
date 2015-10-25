using UnityEngine;
using System.Collections;
using Antlr.StringTemplate;
using Antlr.StringTemplate.Language;
using System.Collections.Generic;

public class GLexAudioSource : GLexComponent {
	
	new protected AudioSource mComponent;
	private GLexSound mClip;
	private static List<GLexSound>		mClips = new List<GLexSound>();
	private static GLexSound[]			mClipsAsArray;
	
	public GLexAudioSource() : base() {
	}

	public static GLexSound[] ClipsAsArray {
		get {
			PrepareForExport();
			return mClipsAsArray;
		}
	}
	
	// overrides
	public override void AssociateWithComponent(object obj) {
		base.AssociateWithComponent(obj);
		mComponent = (AudioSource)obj;
			
		if (mComponent.clip != null) {
			mClip = new GLexSound(mComponent.clip, mComponent.volume, mComponent.loop);
			mClips.Add(mClip);
		}
	}
	
	public float Volume {
		get {
			return mComponent.volume;
		}
	}
	
	public bool IsLooping {
		get {
			return mComponent.loop;
		}
	}
	
	public bool HasAssociatedClip {
		get {
			return mComponent.clip != null;	
		}
	}
	
	public string SoundRef {
		get {
			return mClip.Id;
		}	
	}
	
	public bool IsAudioSource {
		get { return true; }
	}
	
	public static void Reset() {
		mClips.Clear();
	}
	
	new public static void PrepareForExport() {
		mClipsAsArray = mClips.ToArray();
	}
}
