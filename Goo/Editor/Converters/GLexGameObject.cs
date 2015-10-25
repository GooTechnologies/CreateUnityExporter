using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GLexGameObject {
	private string _keystring;
	private List<GLexComponent> mComponents;
	private GLexComponent[] mComponentsAsArray;
	private GameObject mGameObject;
	private GLexData _glexData;
	
	public GLexGameObject(GameObject gameObject, GLexData data) : base() {
		mGameObject = gameObject;
		mComponents = new List<GLexComponent>();
		_glexData = data;
		_keystring = NamesUtil.GenerateUniqueId();
		
//		if (Settings == null) {
//			AddSettings();
//		}
	}

	public GLexData GlexData {
		get {
			return this._glexData;
		}
	}
	
	public void AddSettings() {
		if (mGameObject != null) {
			mGameObject.AddComponent<GLexGameObjectSettings>();
		}
		ResetSettingsExportName();
	}

	public void ResetSettingsExportName() {
		Settings.UniqueName = NamesUtil.GenerateUniqueId();
	}
	
	public void AddComponent(GLexComponent compInterface) {
		mComponents.Add(compInterface);
		compInterface.GlexGameObject = this;
	}
	
	public void PrepareForExport() {
		mComponentsAsArray = mComponents.ToArray();
	}
	
	public GLexGameObjectSettings Settings {
		get {
			if (mGameObject != null) {
				return mGameObject.GetComponent<GLexGameObjectSettings>();
			}
			else {
				return null;
			}
		}
	}
	
	public GameObject GameObject {
		get {
			return mGameObject;
		}
	}
	
	public GLexComponent[] Components {
		get {
			return mComponentsAsArray;
		}
	}
	
	public string[] ComponentIDs {
		get {
			List<string> IDs = new List<string>();
			foreach (GLexComponent component in mComponents) {
				IDs.Add(component.Id);
			}
			return IDs.ToArray();
		}
	}
	
	// template interface starts here
	public string Id {
		get { 
			return _keystring + GLexConfig.GetExtensionFor("entity");
		}
	}
	
	public string License {
		get {
			return "PRIVATE";
		}
	}
	
	public string ExportDate {
		get {
			return System.DateTime.Now.ToString(GLexConfig.TimestampFormat);
		}
	}
	
	public string Name {
		get {
			if (mGameObject != null) {
				return mGameObject.name;
			}
			else {
				return "unnamed";
			}
		}
	}
	
	public bool HasParent {
		get {
			return mGameObject != null && mGameObject.transform.parent != null;
		}
	}
	
	public string ParentName {
		get {
			if (HasParent) {
				return mGameObject.transform.parent.name;	
			}
			else {
				return string.Empty;
			}
		}
	}
	
	public bool HasChildren {
		get {
			return mGameObject != null && mGameObject.transform.childCount > 0;
		}
	}
	
	public List<GLexGameObject> Children {
		get {
			var lst = new List<GLexGameObject>();
			
			if (HasChildren) {
				for (int i = 0; i < mGameObject.transform.childCount; ++i) {
					var glexGO = _glexData.FindObject(mGameObject.transform.GetChild(i).gameObject);
					if (glexGO != null) {
						lst.Add(glexGO);
					}
				}
			}
			
			return lst;
		}
	}
	
	public List<KeyValuePair<int, GLexGameObject>> NumberedChildren {
		get {
			return EnumerableUtil.Indexify(Children);
		}
	}
}
