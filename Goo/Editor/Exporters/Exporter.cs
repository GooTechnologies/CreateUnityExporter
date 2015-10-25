using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using Antlr.StringTemplate;
using Antlr.StringTemplate.Language;

public class Exporter {
	protected 	string mErrorMessage = string.Empty;
	private StringTemplateGroup _templateGroup;
	
	public Exporter() {
	}
	
	public void SetupPaths(string pExportDirectory) {
		GLexConfig.BasePath = pExportDirectory;
		Directory.CreateDirectory(GLexConfig.BasePath);
	}
	
	public virtual bool Export(GLexData data) {
		Debug.Log("Exporter.Export: Please override the Export method");
		return true;
	}

	public StringTemplate LoadTemplate(string name) {
		if (_templateGroup == null) {
			_templateGroup = new StringTemplateGroup("MG", GLexConfig.TemplatePath, typeof(DefaultTemplateLexer));
			_templateGroup.RegisterAttributeRenderer(typeof(bool), TemplateRenderers.BoolRenderer.Instance);
			_templateGroup.RegisterAttributeRenderer(typeof(Color), TemplateRenderers.ColorRenderer.Instance);
			_templateGroup.RegisterAttributeRenderer(typeof(DateTime), TemplateRenderers.DateTimeRenderer.Instance);
			_templateGroup.RegisterAttributeRenderer(typeof(Vector3), TemplateRenderers.VectorRenderer.Instance);
			_templateGroup.RegisterAttributeRenderer(typeof(Vector2), TemplateRenderers.VectorRenderer.Instance);
			_templateGroup.RegisterAttributeRenderer(typeof(Matrix4x4), TemplateRenderers.MatrixRenderer.Instance);
		}

		Debug.Log (name);
		return _templateGroup.GetInstanceOf(GLexConfig.GetTemplate(name));
	}
	
	public string SaveText(string content, string path) {
		return SaveBinary(new UTF8Encoding(true).GetBytes(content), path);
	}
	
	public string SaveBinary(byte[] content, string path) {
		Debug.Log( "START Exporter.SaveBinary: " + path );

		FileStream fileStream = File.Create(path);
		fileStream.Write(content, 0, content.Length);
		fileStream.Close();
		
		Debug.Log( "END Exporter.SaveBinary: " + path );
		
		return path;
	}
	
	public static void Copy(string asset, string toFolder) {
		string target = toFolder + asset.Substring(asset.LastIndexOf("/") + 1);
		File.Delete(target);
		File.Copy(asset, target);
	}
	
	public string CleanJSON(StringTemplate json) {
		return Regex.Replace(json.ToString(), @",(\s*)([\},\]])", @"$1$2", RegexOptions.Multiline);
	}
	
	public string CleanJSON(string json) {
		return Regex.Replace(json, @",(\s*)([\},\]])", @"$1$2", RegexOptions.Multiline);
	}
		
	public bool WithoutErrors {
		get {
			return string.IsNullOrEmpty(mErrorMessage);
		}
	}
	
	public string Error {
		get {
			return mErrorMessage;
		}
		set {
			mErrorMessage = value;
		}
	}
	
	protected void CleanDirectory(string path) {
		foreach (var file in Directory.GetFiles(path)) {
			File.Delete(file);
		}
	}
}
