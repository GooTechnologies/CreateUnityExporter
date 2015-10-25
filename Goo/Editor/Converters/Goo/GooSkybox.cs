using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class GooSkybox : GLexComponent {
	public static readonly List<GooSkybox>	AllSkyboxes = new List<GooSkybox>();
	private Skybox _skybox;
	private GLexTexture _top;
	private GLexTexture _bottom;
	private GLexTexture _left;
	private GLexTexture _right;
	private GLexTexture _front;
	private GLexTexture _back;
	
	public static void Reset() {
		AllSkyboxes.Clear();
	}
	
	public GooSkybox(Skybox pSkybox) {
		_skybox = pSkybox;
		
		_top = GetSkyboxSide("_UpTex");
		_bottom = GetSkyboxSide("_DownTex");
		_left = GetSkyboxSide("_LeftTex");
		_right = GetSkyboxSide("_RightTex");
		_front = GetSkyboxSide("_FrontTex");
		_back = GetSkyboxSide("_BackTex");
		
		AllSkyboxes.Add(this);
	}
	
	private GLexTexture GetSkyboxSide(string texName) {
		if (_skybox.material != null) {
			var tex = _skybox.material.GetTexture(texName) as Texture2D;
			if (tex != null) {
				return GLexTexture.Get(tex);
			}
		}
		return null;
	}
	
	protected override string IdExtension {
		get {
			return ".skybox";
		}
	}
	
	public override string Name {
		get {
			return (_skybox != null && _skybox.gameObject != null) ? _skybox.gameObject.name : string.Empty;
		}
	}

	public GLexTexture Back {
		get {
			return this._back;
		}
	}

	public GLexTexture Bottom {
		get {
			return this._bottom;
		}
	}

	public GLexTexture Front {
		get {
			return this._front;
		}
	}

	public GLexTexture Left {
		get {
			return this._left;
		}
	}

	public GLexTexture Right {
		get {
			return this._right;
		}
	}

	public GLexTexture Top {
		get {
			return this._top;
		}
	}
}
