using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEditor;
using System.Reflection;

public class GLex : EditorWindow {
	public static GLex Instance { get; private set; }
	
	// Export options
	private string _gooUserName;
	private string _exportDirectory;
	private	bool _exportMeshes = true;
	private	bool _exportTextures = true;
	private	bool _exportAnimations = true;
	private	bool _cleanDirectory;
	private bool _bundleProject;
	private bool _uploadToCreate;
	private string _importToken = "";
	private string _projectId = "";
	private GLexData mData;
	
	// GUI state
	private bool _exportOptionsFoldout = true;
	private bool _gooCreateOptionsFoldout = true;
	private int _exportMode = 0;
	private List<Transform> selectedTransforms = new List<Transform>();
	private List<GameObject> _toExport = new List<GameObject>();

	public bool ExportAnimations {
		get {
			return this._exportAnimations;
		}
		set {
			_exportAnimations = value;
		}
	}

	public bool ExportMeshes {
		get {
			return this._exportMeshes;
		}
	}

	public bool ExportTextures {
		get {
			return this._exportTextures;
		}
	}

	[MenuItem("Goo/Open Exporter Window...")]
	static void OpenExportWindow() {
		EditorWindow.GetWindow<GLex>();
	}
	
	public GLex() {
		title = "Goo Exporter";
		LoadSettings();
	}
	
	private void LoadSettings() {
		_gooUserName = EditorPrefs.GetString("GooExporter.UserName");
		_exportDirectory = EditorPrefs.GetString("GooExporter.ExportDirectory");
		_exportMeshes = EditorPrefs.GetBool("GooExporter.ExportMeshes");
		_exportTextures = EditorPrefs.GetBool("GooExporter.ExportTextures");
		_exportAnimations = EditorPrefs.GetBool("GooExporter.ExportAnimations");
		_cleanDirectory = EditorPrefs.GetBool("GooExporter.CleanDirectory");
		_bundleProject = EditorPrefs.GetBool("GooExporter.BundleProject");
	}
	
	private void SaveSettings() {
		EditorPrefs.SetString("GooExporter.UserName", _gooUserName);
		EditorPrefs.SetString("GooExporter.ExportDirectory", _exportDirectory);
		EditorPrefs.SetBool("GooExporter.ExportMeshes", _exportMeshes);
		EditorPrefs.SetBool("GooExporter.ExportTextures", _exportTextures);
		EditorPrefs.SetBool("GooExporter.ExportAnimations", _exportAnimations);
		EditorPrefs.SetBool("GooExporter.CleanDirectory", _cleanDirectory);
		EditorPrefs.SetBool("GooExporter.BundleProject", _bundleProject);
	}
	
	private bool ExportWholeScene {
		get {
			return _exportMode == 0;	
		}
	}

	public bool BundleProject {
		get {
			return this._bundleProject;
		}
	}
	
	private void Foldout(ref bool state, string caption, Action doInnerContents) {
		GUILayout.BeginVertical("box");
		
		state = EditorGUILayout.Foldout(state, caption);
		if (state) {
			GUILayout.BeginHorizontal();
			GUILayout.Space(16);
			GUILayout.BeginVertical();
			
			doInnerContents();
			
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
		
		GUILayout.EndVertical();
	}
	
	private void OnGUI() {
		bool okToExport = true;
		
		_exportMode = EditorGUILayout.Popup("Export Mode", _exportMode, new string[] {"Entire Scene", "Just Selection"});
		
		Foldout(ref _exportOptionsFoldout, "Export Options", ExportOptionsGUI);
		Foldout(ref _gooCreateOptionsFoldout, "Goo Create Options", GooCreateOptionsGUI);
		
		// Save anything we changed
		if (GUI.changed) {
			SaveSettings();
		}
		
		// Sanity check options
		if (string.IsNullOrEmpty(_gooUserName.Trim())) {
			EditorGUILayout.HelpBox("You must specify a non-empty username.", MessageType.Error);
			okToExport = false;
		}
		if (string.IsNullOrEmpty(_exportDirectory) || !Directory.Exists(_exportDirectory)) {
			EditorGUILayout.HelpBox("You must select an output directory.", MessageType.Error);
			okToExport = false;
		}
		if (ExportWholeScene == false) {
			if (Selection.transforms.Length == 0) {
				EditorGUILayout.HelpBox("You must select at least one object to export.", MessageType.Error);
				okToExport = false;
			}
		}
	
		if (UploadToCreate) {
			
			if (string.IsNullOrEmpty(_importToken.Trim())) {
				EditorGUILayout.HelpBox("You must provide an import token to upload to goo create.", MessageType.Error);
				okToExport = false;
			}
			else if (string.IsNullOrEmpty(_projectId.Trim())) {
				EditorGUILayout.HelpBox("You must provide project id to upload to goo create.", MessageType.Error);
				okToExport = false;
			}
		}
		
		GUI.enabled = okToExport;
		bool exportClicked = GUILayout.Button("Export");
		GUI.enabled = true;
		if (exportClicked) {
			
			if (SetupSceneForExport()) {
				
				Export();
				RestoreSceneAfterExport();
			}
		}
	}
	
	private void ExportOptionsGUI() {
		
		_gooUserName = EditorGUILayout.TextField("Goo username", _gooUserName);
		
		GUILayout.BeginHorizontal();
		_exportDirectory = EditorGUILayout.TextField("Output directory", _exportDirectory);
		if (GUILayout.Button("...", GUILayout.ExpandWidth(false))) {
			_exportDirectory = EditorUtility.SaveFolderPanel("Select output directory", _exportDirectory, string.Empty);
		}
		GUILayout.EndHorizontal();
		
		_cleanDirectory = EditorGUILayout.Toggle("Clean output directory", _cleanDirectory);
		_bundleProject = EditorGUILayout.Toggle("Bundle project", _bundleProject);
		//_exportMeshes = EditorGUILayout.Toggle("Export meshes", _exportMeshes);
		//_exportTextures = EditorGUILayout.Toggle("Export textures", _exportTextures);
		//_exportAnimations = EditorGUILayout.Toggle("Export animations", _exportAnimations);
		
		_exportMeshes = _exportTextures = _exportAnimations = true;
	}
	
	private bool SetupSceneForExport() {
		
		_toExport.Clear();
		
		if (ExportWholeScene) {
			GameObject[] gameObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
		
			foreach (GameObject go in gameObjects) {

				if (go.transform.parent == null) {// && go.transform != mExportTransform) {
					_toExport.Add(go);
				}
			}
		}
		else {
			//Should probably look if each object is added through relation to already added to avoid duplicates?
			selectedTransforms.Clear();
							
			foreach (var transform in Selection.transforms) {
				selectedTransforms.Add(transform);
			}

			for (int i=0; i<selectedTransforms.Count; ++i) {
				_toExport.Add(selectedTransforms[i].gameObject);
			}
		}
		
		hasCamera = false;
		
		if (HasOrphanCamera()) {
			return true;
		}
		else {
		 	
			if (!hasCamera) {
				EditorUtility.DisplayDialog("No Camera", "Your export must contain a camera", "ok");
			}
			else {
				EditorUtility.DisplayDialog("Camera cannot have parent", "Your camera cannot be a child of another object", "ok");
			}
			return false; 
		}
	}
	
	bool hasCamera = false;
	
	private bool HasOrphanCamera() {
		for (int i=0; i<_toExport.Count; ++i) {
			Camera cam = _toExport[i].GetComponentInChildren<Camera>();
			
			if (cam != null) {
				hasCamera = true;
				
				if (cam.transform.parent == null) {
					return true;
				}
			}
		}
		return false;
	}
	
	private void RestoreSceneAfterExport() {
		selectedTransforms.Clear();
		GLexData.RemoveAddedSettings();
	}
	
	private void GooCreateOptionsGUI() {
		_uploadToCreate = EditorGUILayout.Toggle("Upload to Goo Create ", _uploadToCreate);
		
		if (_uploadToCreate) {
			_importToken = EditorGUILayout.TextField("Import Token ", _importToken);
			_projectId = EditorGUILayout.TextField("Project Id ", _projectId);
		}
	}
	
	// export
	private void Export() {
		Instance = this;
		_canceled = false;
	
		try {
			// load config json
			GLexConfig.Load();
			
			// create exporter and setup paths
			Type exporterType = Type.GetType((string)GLexConfig.ExporterType, true);
			Exporter exporter = Activator.CreateInstance(exporterType) as Exporter;					
			
			exporter.SetupPaths(_exportDirectory);
			
			// recurse scene graph
			mData = new GLexData();
			
			if (!GLexConfig.ExportSceneGameObject) {
				mData.Scene = new GLexScene(mData);
				mData.Project = new GooProject(mData, PlayerSettings.productName);
			}
				
			for (int i = 0; i < _toExport.Count; ++i) {
				if (!RecurseSceneGraph(_toExport[i].transform, null)) {
					return;
				}
			}
			
			EditorUtility.DisplayProgressBar("Preparing for export", string.Empty, 0);
			mData.PrepareForExport();
			
			// export
			if (exporter.Export(mData)) {
			
				if (!exporter.WithoutErrors) {
					Debug.LogError("One or more errors occurred during export: " + exporter.Error);
				}
				else {
					EditorUtility.ClearProgressBar();
					EditorUtility.DisplayDialog("Export successful!", "Exported to " + GLexConfig.BasePath, "ok");
					Debug.Log("Exported to " + GLexConfig.BasePath);
				}
			}
			else {
				Debug.LogError("User cancelled export");
			}
		}
		finally {
			EditorUtility.ClearProgressBar();
		}
	}
	
	// recurse the graph
	bool _canceled = false;

	private bool RecurseSceneGraph(Transform transform, Transform parent) {
		if (_canceled) {
			return false;
		}
		
		if (EditorUtility.DisplayCancelableProgressBar("Collecting Objects", transform.name, 0)) {
			Debug.LogError("User cancelled export");
			_canceled = true;
			return false;
		}
		
		bool active = GLexConfig.GetOption(GLexConfig.RECURSEINACTIVESCHILDREN) ? transform.gameObject.activeSelf : transform.gameObject.activeInHierarchy;
		
		if (!GLexConfig.ExportLeafWithOnlyTransform &&
			transform.GetComponents<Component>().Length == 1 &&
			transform.childCount == 0) {
			active = false;
		}
		
		if (transform.GetComponent<GLexGameObjectSettings>() != null && 
			transform.GetComponent<GLexGameObjectSettings>().export == false) {
			active = false;
		}
		
		if (active) {
			GLexGameObject gameObject = new GLexGameObject(transform.gameObject, mData);
			mData.AddGameObject(gameObject);
			
			foreach (KeyValuePair<string,string> componentAndConverters in GLexConfig.Converters) {
				var unityComponent = transform.GetComponent(componentAndConverters.Key);
				
				if (unityComponent != null) {
					Type coConverterType = Type.GetType(componentAndConverters.Value, true);
					GLexComponent component = Activator.CreateInstance(coConverterType) as GLexComponent;
					
					component.AssociateWithGameObject(transform.gameObject);
					component.AssociateWithComponent(unityComponent);
					
					mData.AddComponent(component);
					gameObject.AddComponent(component);
				}
			}
		}

		// traverse children
		if (active || GLexConfig.GetOption(GLexConfig.RECURSEINACTIVESCHILDREN)) {
			for (int c = 0; c < transform.childCount; c++) {
				RecurseSceneGraph(transform.GetChild(c), transform);
			}
		}
		
		return true;
	}

	// helpers
	
	private void ClearConsole() {
		Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
	 
		Type type = assembly.GetType("UnityEditorInternal.LogEntries");
		MethodInfo method = type.GetMethod("Clear");
		method.Invoke(new object(), null);
	}
	
	public string GetProjectName() {
		string[] s = Application.dataPath.Split('/');
		string projectName = s[s.Length - 2];
		Debug.Log("project = " + projectName);
		return projectName;
	}
	
	public string UserName {
		get {
			return _gooUserName;
		}
	}
	
	public string ImportToken {
		get {
			return this._importToken;
		}
	}

	public string ProjectId {
		get {
			return this._projectId;
		}
	}
	
	public bool UploadToCreate {
		get {
			return this._uploadToCreate;
		}
	}
	
	public bool CleanDirectory {
		get {
			return this._cleanDirectory;
		}
	}	
}
