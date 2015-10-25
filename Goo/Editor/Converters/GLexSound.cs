using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class GLexSound : GLexComponent {
	
	new protected AudioClip mComponent;
	private bool _loops;
	private float _volume;
	private AudioImporter mImporter;
	private byte[] _dataBytes;
	private string _dataBinaryId;

	public GLexSound(AudioClip pClip, float pVolume, bool pLooping) : base() {
			
		mComponent = pClip;
		mImporter = (AudioImporter)AssetImporter.GetAtPath(OriginalURL);
		
		_volume = pVolume;
		_loops = pLooping;
		
		_dataBytes = File.ReadAllBytes(AssetDatabase.GetAssetPath(mComponent));
		_dataBinaryId = NamesUtil.GenerateBinaryId(_dataBytes, GLex.Instance.UserName);
	}
			
	public string OriginalURL {
		get {
			return AssetDatabase.GetAssetPath(mComponent);
		}
	}
	
	public float Duration {
		get {
			return mComponent.length;	
		}
	}
	
	public string Format {
		get {
//			return mImporter.format == AudioImporterFormat.Native ? "wav" : "mp3";
			return "wav";
		}
	}
	
	protected override string IdExtension {
		get {
			return GLexConfig.GetExtensionFor("sound");
		}
	}
	
	public float Volume {
		get {
			return _volume;
		}
	}
	
	public bool IsLooping {
		get {
			return _loops;
		}
	}
	
	public string SoundRef {
		get {
			return _dataBinaryId;	
		}
	}
	
	public string BinaryId {
		get {
			return _dataBinaryId + BinaryIdExtension;
		}
	}

	public string BinaryIdExtension {
		get {
//			return mImporter.format == AudioImporterFormat.Compressed ? ".mp3" : ".wav";
			return "wav";
		}
	}
	
	public void SaveBinaryData() {
		File.WriteAllBytes(Path.Combine(GLexConfig.GetPathFor("sound"), BinaryId), _dataBytes);
	}
}
