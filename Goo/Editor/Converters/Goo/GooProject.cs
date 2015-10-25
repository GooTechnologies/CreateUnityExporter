using UnityEngine;
using System.Collections;

public class GooProject : GLexComponent {
	private GLexData _glexData;
	private string _name;
	
	public GooProject(GLexData data, string name) {
		_glexData = data;
		_name = name;
	}
	
	public GLexScene MainScene {
		get {
			return _glexData.Scene;
		}
	}
	
	protected override string IdExtension {
		get {
			return ".project"; //GLexConfig.GetExtensionFor("project");
		}
	}
	
	public override string Name {
		get {
			return _name;
		}
	}
}
