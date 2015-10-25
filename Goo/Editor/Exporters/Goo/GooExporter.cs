using UnityEngine;
using System;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using Ionic.Zip;

using Antlr.StringTemplate;
using Antlr.StringTemplate.Language;
using UnityEditor;
using System.Text;

public class GooExporter : Exporter {
	
	private static List<Component>	mComponentsToRemoveAfterExport = new List<Component>();
	private static List<GameObject>	mObjectsToRemoveAfterExport = new List<GameObject>();
	
//	private string createBaseURL = "http://localhost:8000/";
	private string createBaseURL = "http://create.dev.gooengine.com/";
//	private string createBaseURL = "http://create.gootechnologies.com/";
	
	
	private Dictionary<string, string> _exportedFiles = new Dictionary<string, string>();
	
	public GooExporter() : base() {
		mComponentsToRemoveAfterExport.Clear();
		mObjectsToRemoveAfterExport.Clear();
		GooSkinnedMeshRenderer.Reset();
	}
	
	private bool ValidateSettings() {
		// check so we're connected to Goo
		if (GooCreate.ImportToken.Length == 0) {
			return false;
		}
	
		WWW www;
		if (createBaseURL.IndexOf("dev") != -1) {
			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers.Add("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("googuest:goosecret")));
	
			www = new WWW(createBaseURL + "api/import?token=" + GooCreate.ImportToken, null, headers);
		}
		else {
			www = new WWW(createBaseURL + "api/import?token=" + GooCreate.ImportToken);
		}
		
		while (!www.isDone) {
		}
		
		if (!string.IsNullOrEmpty(www.error)) {
			Debug.LogError("GooExporter: " + www.error);
			return false;
		}
		
		Hashtable response = MiniJSON.jsonDecode(www.text) as Hashtable;
		if (response != null && response.ContainsKey("message") && ((string)response["message"]).Equals("Token OK.")) {
			Debug.Log("GooExporter: Valid Importer Token");
		}
		else {
			Debug.LogError("GooExporter: Invalid Importer Token, please login to Goo Create to generate a valid token!");
			return false;
		}
		
		return true;
	}
	
	private struct ExportAction {
		public string Description;
		public Action Action;
	}
	
	private Queue<ExportAction> _exportQueue = new Queue<ExportAction>();
	
	private void AddExportAction(string pDesc, Action pAction) {
		///*
		_exportQueue.Enqueue(new ExportAction {
			Description = pDesc,
			Action = pAction
		});
		//*/
		
		//pAction();
	}
	
	private bool ExecuteExportActions() {
		int totalCount = _exportQueue.Count;
		while (_exportQueue.Count > 0) {
			var act = _exportQueue.Dequeue();
			if(EditorUtility.DisplayCancelableProgressBar("Exporting", act.Description + "...", (float)(totalCount - _exportQueue.Count) / totalCount))
			{
				return false;	
			}
			
			act.Action();
		}
		
		return true;
	}
	
	public override bool Export(GLexData data) {
		/*
		// Uncomment this to re-enable settings validation.
		// Right now it's just obnoxious and the results aren't used for anything.
		if (!ValidateSettings()) {
			return;
		}
		*/
		
		_exportedFiles.Clear();
		_exportQueue.Clear();
		
		if (GLex.Instance.CleanDirectory) {
			AddExportAction("Cleaning output directory", () => CleanDirectory(GLexConfig.BasePath));
		}
		
		// save project
//		AddExportAction("Exporting project", delegate {
//			StringTemplate projectTemplate = LoadTemplate("project");
//			projectTemplate.SetAttribute("project", data.Project);
//			AddExportedJson(projectTemplate, data.Project.Id);
//		});
		
		// environment
		AddExportAction("Exporting environment", delegate {
			var environment = data.Scene.Environment;
			var envTemplate = LoadTemplate("environment");
			envTemplate.SetAttribute("env", environment);
			AddExportedJson(envTemplate, environment.Id);
		});
		
		// save scene
		AddExportAction("Exporting scene", delegate {
			StringTemplate scene = LoadTemplate("scene");
			scene.SetAttribute("entities", EnumerableUtil.Indexify(data.TopGameObjects));
			scene.SetAttribute("scene", data.Scene);
			scene.SetAttribute("exporter", this);
			AddExportedJson(scene, data.Scene.Id);
		});

		// save postfx
		AddExportAction("Exporting postfx", delegate {
			StringTemplate postfx = LoadTemplate("posteffects");
			//			postfx.SetAttribute("entities", EnumerableUtil.Indexify(data.GameObjects));
			//			postfx.SetAttribute("scene", data.Scene);
			//			postfx.SetAttribute("exporter", this);
			AddExportedJson(postfx, "403694af24c54b6c93434ee2f8ce6f0b.posteffects");
		});

		// save entities
		foreach (var goConverter in data.GameObjects) {
			var exportedObject = goConverter; // Copy the reference
			
			AddExportAction(goConverter.Name, delegate {
				var entityTemplate = LoadTemplate("entity");
				entityTemplate.SetAttribute("entity", exportedObject);
				entityTemplate.SetAttribute("components", exportedObject.Components);
				AddExportedJson(entityTemplate, exportedObject.Id);
			});
		}
		
		// save materials
		foreach (var matConverter in data.Materials) {
			var exportedObject = matConverter; // Copy the reference
			
			AddExportAction(matConverter.Name, delegate {
				StringTemplate material = LoadTemplate("material");
				material.SetAttribute("material", exportedObject);
				AddExportedJson(material, exportedObject.Id);
			});
		}
		
		// save textures
		foreach (GLexTexture texConverter in data.Textures) {
			var exportedObject = texConverter; // Copy the reference
			
			AddExportAction(texConverter.Name, delegate {
				exportedObject.SaveBinaryData();
				
				var texture = LoadTemplate("texture");
				texture.SetAttribute("texture", exportedObject);
				AddExportedJson(texture, exportedObject.Id);
			});
		}
		
		// save sounds
		foreach (var soundConverter in data.SoundClips) {
			var exportedObject = soundConverter; // Copy the reference
			
			AddExportAction(soundConverter.Name, delegate {
				exportedObject.SaveBinaryData();
				
				var soundTemplate = LoadTemplate("sound");
				soundTemplate.SetAttribute("sound", exportedObject);
				AddExportedJson(soundTemplate, exportedObject.Id);
			});
		}
		
		// save meshes 
		if (GLex.Instance.ExportMeshes) {
			var binary = new GooBinary();
			
			var converters = new List<GooBinaryMesh>();
			foreach (var glexMesh in data.Meshes) {
				converters.Add(new GooBinaryMesh(glexMesh));
			}
			
			for (int i = 0; i < converters.Count; ++i) {
				var meshConverter = converters[i];
				
				AddExportAction(meshConverter.Name + " (binary)", delegate {
					meshConverter.AddToBinary(binary);
				});
			}
			
			AddExportAction("Saving mesh binaries", () => binary.SaveToDisk());
			
			for (int i = 0; i < converters.Count; ++i) {
				foreach (var subMeshProxy in converters[i].SubMeshes) {
					var proxyCopy = subMeshProxy;
					
					AddExportAction(converters[i].Name + "#" + subMeshProxy.SubMeshIndex, delegate {
						StringTemplate mesh = LoadTemplate("mesh");
						mesh.SetAttribute("mesh", proxyCopy);
						AddExportedJson(mesh, proxyCopy.Id);
					});
				}
			}
		}
		
		// save skeletons
		ExportObjects(GooSkeleton.AllSkeletons, "skeleton", "skeleton");
		
		// save animations 
		ExportObjects(data.Animations, "animation", "animation");
		
		// save states
		ExportObjects(data.AnimationStates, "animationState", "state");
		
		// save clips 
		var clipBinary = new GooBinary();
		
		foreach (var glexClip in data.AnimationClips) {
			var exportedClip = glexClip;
			AddExportAction(exportedClip.Name, delegate {
				exportedClip.AddToBinary(clipBinary);
			});
		}
		
		AddExportAction("Saving clip binaries", () => clipBinary.SaveToDisk());
		
		foreach (var glexClip in data.AnimationClips) {
			var exportedClip = glexClip;
			AddExportAction(exportedClip.Name, delegate {
				var template = LoadTemplate("animationClip");
				template.SetAttribute("clip", exportedClip);
				AddExportedJson(template, exportedClip.Id);
			});
		}
		
		// skyboxes
		ExportObjects(GooSkybox.AllSkyboxes, "skybox", "skybox");
		
		// Do it all
		if(!ExecuteExportActions())
			return false;
		
		// send to create or bundle
		if (GLex.Instance.UploadToCreate) {
			if(SaveExportedFilesAsBundle())
				SendToCreate();
			else
				return false;
		}
		else if (GLex.Instance.BundleProject) {
			if(!SaveExportedFilesAsBundle()) {
				return false;
			} else {
				//		string directory = Path.Combine (GLexConfig.BasePath, "root.bundle");
				//		string zip = Path.Combine (GLexConfig.BasePath, GLexSceneSettings.FileName + ".zip");
				string directory = GLexConfig.BasePathWithoutSceneName;
				string zip = GLexConfig.BasePathWithoutSceneName + "/bundle_" + GLexSceneSettings.FileName + ".zip";
				Compress(directory, zip);
			}
		}
		else {
			SaveExportedFiles();
		}
		
		// remove temporary components
		foreach (Component component in mComponentsToRemoveAfterExport) {
			UnityEngine.Object.DestroyImmediate(component);
		}
		foreach (var obj in mObjectsToRemoveAfterExport) {
			UnityEngine.Object.DestroyImmediate(obj);
		}
		
		return true;
	}
	
	private void ExportObjects<T>(IEnumerable<T> pObjects, string pTemplateId, string pObjectKey) where T : GLexComponent {
		if (pObjects == null) {
			Debug.LogError(typeof(T).ToString() + ": list is null");
			return;
		}
		
		foreach (var obj in pObjects) {
			var exportedObject = obj; // Copy the reference
			
			AddExportAction(obj.Name, delegate {
				var template = LoadTemplate(pTemplateId);
				template.SetAttribute(pObjectKey, exportedObject);
				AddExportedJson(template, exportedObject.Id);
			});
		}
	}
		
	private void SaveExportedFiles() {
		// Save everything as files
		int i = 0;
		foreach (var kvp in _exportedFiles) {
			EditorUtility.DisplayProgressBar("Exporting", "Saving " + kvp.Key, (float)(i++) / _exportedFiles.Count);
			SaveText(CleanJSON(kvp.Value), Path.Combine(GLexConfig.BasePath, kvp.Key));
		}
	}
	
	private bool SaveExportedFilesAsBundle() {
		var bundleTextBuilder = new StringBuilder();
		
		int i = 0;
		bundleTextBuilder.AppendLine("{");
		foreach (var kvp in _exportedFiles) {
			
			if(EditorUtility.DisplayCancelableProgressBar("Bundling project", kvp.Key, (float)(i++) / _exportedFiles.Count))
				return false;
			
			bundleTextBuilder.AppendFormat("\t\"{0}\": ", kvp.Key);
			bundleTextBuilder.Append(kvp.Value.Trim().Replace("\n", "\n\t"));
			bundleTextBuilder.AppendLine(",");
		}
		bundleTextBuilder.AppendLine("}");
		
		var bundleText = bundleTextBuilder.ToString();
		
		EditorUtility.DisplayProgressBar("Writing bundle to disk", string.Empty, 0);
		SaveText(CleanJSON(bundleText), Path.Combine(GLexConfig.BasePath, "root.bundle"));

		return true;
	}
	
	protected void AddExportedJson(string pContent, string pObjectId) {
		_exportedFiles.Add(pObjectId, CleanJSON(pContent));
	}
		
	protected void AddExportedJson(StringTemplate pContent, string pObjectId) {
		if (_exportedFiles.ContainsKey(pObjectId)) {
			Debug.LogError("Duplicate file: " + pObjectId);
			throw new ArgumentException();
		}
		
		_exportedFiles.Add(pObjectId, CleanJSON(pContent.ToString()));
	}
	
	private void SendToCreate() {
		string directory = GLexConfig.BasePathWithoutSceneName;
		string zip = GLexConfig.BasePathWithoutSceneName + GLexSceneSettings.FileName + ".zip";
	
		Compress(directory, zip);
		
		EditorUtility.DisplayProgressBar("Preparing to upload", string.Empty, 0);
		WWWForm form = new WWWForm();
		form.AddBinaryData("zip", File.ReadAllBytes(zip));
		form.AddField("token", GLex.Instance.ImportToken);
		
		if (GLex.Instance.ProjectId.Length > 0) {
			form.AddField("project_id", GLex.Instance.ProjectId);
		}

		WWW www;
		if (createBaseURL.IndexOf("dev") != -1) {
			www = new WWW("http://googuest:goosecret@create.dev.gooengine.com/api/import", form);
		}
		else {
			www = new WWW(createBaseURL + "api/import", form);
		}
		
		while (!www.isDone) {
				EditorUtility.DisplayProgressBar("Uploading to Goo Create","Sending files", www.progress);
		}
		
		if (!string.IsNullOrEmpty(www.error)) {
			Debug.LogError("Error occured while trying to upload to goo create: " + www.error);
		}
		else {
			Hashtable response = MiniJSON.jsonDecode(www.text) as Hashtable;
			if (response != null) {
				string status = response.ContainsKey("status") ? Convert.ToString(response["status"]) : "";
				
				if (!status.Equals("200")) {
					EditorUtility.DisplayDialog("Upload failed",  response["message"].ToString(), "ok");
					Debug.LogError("GooExporter.SendToCreate: " + response["message"]);
				}
			}
			else {
				EditorUtility.DisplayDialog("Upload failed",  www.text, "ok");
				Debug.LogError("Error occured while trying to upload to goo create: " + www.text);
			}
		}
	}

	private void Compress(string directory, string zipFileName) {
		FileInfo[] fileInfos = new DirectoryInfo(directory).GetFiles("*.*", SearchOption.AllDirectories);
		Debug.Log (fileInfos.Length);

		using (ZipFile zip = new ZipFile()) {
			for(int k=0;k<fileInfos.Length;k++) {
				FileInfo fileInfo = fileInfos[k];
//				Debug.Log (k);
//				Debug.Log ("--------- " + fileInfo.FullName + " : " + k);
//				Debug.Log ((File.GetAttributes(fileInfo.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden);
				EditorUtility.DisplayProgressBar("Compressing project", "", (float)(k) / fileInfos.Length);

				if ((File.GetAttributes(fileInfo.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden) {
					string relativePath = fileInfo.FullName.Substring(directory.Length);
					if (relativePath.LastIndexOf(Path.DirectorySeparatorChar) != -1) {
						relativePath = relativePath.Substring(0, relativePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
					}
					else {
						relativePath = "";
					}
	
					zip.AddFile(fileInfo.FullName, relativePath);
				}
			}
//			Debug.Log ("--- All files added ---");
			EditorUtility.DisplayProgressBar("Saving compressed project", "Writing file", 0.4f);
			zip.Save(zipFileName);
		}
	}
	
	private void CopyStream(Stream input, Stream output) {     
		byte[] buffer = new byte[32768];     
		long TempPos = input.Position;     
		while (true) {         
			int read = input.Read(buffer, 0, buffer.Length);         
			if (read <= 0) {
				break;
			}         
			output.Write(buffer, 0, read);     
		}     
		input.Position = TempPos; 
	}
	
	private void InsertString(string val, Stream output) {
		byte[] buffer = System.Text.Encoding.UTF8.GetBytes(val);
		output.Write(buffer, 0, buffer.Length);
	}
	
	// stats
	
	public string CharacterCount 		{ get { return "100"; } }

	public string CharacterPoseCount 	{ get { return "200"; } }

	public string ControlSetPlugCount 	{ get { return "300"; } }

	public string GeometryCount 		{ get { return "400"; } }

	public string MaterialCount 		{ get { return "500"; } }

	public string NodeCount 			{ get { return "600"; } }

	public string PoseCount 			{ get { return "700"; } }

	public string TextureCount 			{ get { return "800"; } }
	
	// static
	
	public static void RemoveAfterExport(Component component) {
		mComponentsToRemoveAfterExport.Add(component);
	}

	public static void RemoveAfterExport(GameObject obj) {
		mObjectsToRemoveAfterExport.Add(obj);
	}
}
