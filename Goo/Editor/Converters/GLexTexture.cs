using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class GLexTexture : GLexComponent {
	
	public enum Filter {
		NEAREST,
		LINEAR,
		NEAREST_MIPMAP_NEAREST,
		LINEAR_MIPMAP_NEAREST,
		NEAREST_MIPMAP_LINEAR,
		LINEAR_MIPMAP_LINEAR
	}

	private static List<GLexTexture>	mTextures;
	private static GLexTexture[]		mTexturesAsArray;
	private Texture2D					mTexture;
	private TextureImporter 			mImporter;
	private byte[] _dataBytes;
	private string _dataBinaryKeystring;
	
	private GLexTexture(Texture2D texture) : base() {
		mTexture = texture;
		mImporter = (TextureImporter)AssetImporter.GetAtPath(OriginalURL);
		
		// texture need to be readable for export
		if (!mImporter.isReadable) {
			Debug.LogWarning("GLexTexture.Construct: Setting texture " + Name + " as Readable, or export will fail!");

			mImporter.isReadable = true;
			AssetDatabase.ImportAsset(OriginalURL);
			mImporter = (TextureImporter)AssetImporter.GetAtPath(OriginalURL);
		}
		
		if (IsARGB32orRGB24) {
			_dataBytes = mTexture.EncodeToPNG();
		}
		else {
			_dataBytes = new JPGEncoder(mTexture, GLexConfig.JPEGQuality).GetBytes();
		}
		_dataBinaryKeystring = NamesUtil.GenerateBinaryId(_dataBytes, GLex.Instance.UserName);
		
		mTextures.Add(this);
	}
	
	public static void Reset() {
		mTextures = new List<GLexTexture>();
	}
	
	new public static void PrepareForExport() {
		mTexturesAsArray = mTextures.ToArray();
	}
	
	public Texture2D Texture {
		get {
			return mTexture;
		}
	}
	
	public void SaveBinaryData() {
		var outPath = Path.Combine(GLexConfig.GetPathFor("image"), BinaryId);
		if (!File.Exists(outPath)) {
			// Because the filename is a hash of its contents, we know we don't need to actually write out the file if it already exists
			File.WriteAllBytes(outPath, _dataBytes);
		}
	}
	
	private bool IsARGB32orRGB24 {
		get {
			return mImporter.textureFormat == TextureImporterFormat.ARGB32 || mImporter.textureFormat == TextureImporterFormat.RGB24;
		}
	}
	
	protected override string IdExtension {
		get {
			return GLexConfig.GetExtensionFor("texture");
		}
	}
	
	// antler interface starts here
	
	public override string Name {
		get {
			return mTexture.name;
		}
	}
	
	public string BinaryId {
		get {
			return _dataBinaryKeystring + BinaryIdExtension;
		}
	}

	public string BinaryFileFormat {
		get {
			if (IsARGB32orRGB24) {
				return "RGBA";
			}
			else {
				return "RGB";
			}
		}
	}

	public string BinaryIdExtension {
		get {
			return IsARGB32orRGB24 ? ".png" : ".jpg";
		}
	}

	public bool IsJPG {
		get {
			return !IsARGB32orRGB24;
		}
	}
	
	public bool IsPNG {
		get {
			return IsARGB32orRGB24;
		}
	}
	
	public string OriginalURL {
		get {
			return AssetDatabase.GetAssetPath(mTexture);
		}
	}
	
	public string RealURL {
		get {
			return Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/') + 1) + OriginalURL;
		}
	}
	
	public string Wrap {
		get {
			return GLexConfig.TransformWrapMode(mTexture.wrapMode);
		}
	}

	public string MagFilter {
		get {
			if (mTexture.filterMode == FilterMode.Point) {
				return GLexConfig.TransformTextureFilter(Filter.NEAREST);
			}
			return  GLexConfig.TransformTextureFilter(Filter.LINEAR);
		}
	}

	public string MinFilter {
		get {
			if (mTexture.mipmapCount == 1) {
				if (mTexture.filterMode == FilterMode.Point) {
					return GLexConfig.TransformTextureFilter(Filter.NEAREST);
				}
				return GLexConfig.TransformTextureFilter(Filter.LINEAR);
			}
			else {
				if (mTexture.filterMode == FilterMode.Point) {
					return GLexConfig.TransformTextureFilter(Filter.NEAREST_MIPMAP_NEAREST);
				}
				else if (mTexture.filterMode == FilterMode.Bilinear) {
					return GLexConfig.TransformTextureFilter(Filter.LINEAR_MIPMAP_NEAREST);
				}
				return GLexConfig.TransformTextureFilter(Filter.LINEAR_MIPMAP_LINEAR);
			}
		}
	}
	
	public string AnisotropyLevel {
		get {
			// The exported format bizarrely does not support disabling anisotropic filtering, so we need to set it to at least 1...
			return Mathf.Max(1, mImporter.anisoLevel).ToString();
		}
	}
	
	public string Offset {
		get {
			return "0, 0";
		}
	}
	
	public string Scaling {
		get {
			return "1, 1";
		}
	}
	
	// static 
	public static GLexTexture[] TexturesAsArray {
		get {
			return mTexturesAsArray;
		}
	}
	
	public static bool Exists(Texture2D texture) {
		if (texture != null) {
			foreach (GLexTexture glexTexture in mTextures) {
				if (glexTexture.Texture == texture) {
					return true;
				}
			}
			
			return false;
		}
		else {
			return true;		// return true for null textures to avoid creation of empty GLexTexture
		}
	}
	
	public static GLexTexture Get(Texture2D texture) {
		if (texture == null) {
			return null;
		}
		else {
			foreach (GLexTexture glexTexture in mTextures) {
				if (glexTexture.Texture == texture) {
					return glexTexture;
				}
			}
			
			return new GLexTexture(texture);
		}
	}
}
