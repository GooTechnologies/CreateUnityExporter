using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class GLexConfig {
	
	public const string RECURSEINACTIVESCHILDREN 	= "recurseInactivesChildren";
	public const string SETUNIQUENAMES           	= "setUniqueNames";
	public const string LOWERCASENAMES			 	= "lowerCaseNames";
	public const string REMOVESPACESINNAMES		 	= "removeSpacesInNames";
	public const string FREEZESCALE              	= "freezeScale";
	public const string FREEZEROTATION			 	= "freezeRotation";
	public const string ADDSCENENAMETOPATH       	= "addSceneNameToPath";
	public const string EXPORTSCENEGAMEOBJECT	 	= "exportSceneGameObject";
	public const string CAMERACANHAVEPARENT      	= "cameraCanHaveParent";
	public const string EXPORTBONEASGAMEOBJECT   	= "exportBoneAsGameObject";
	public const string EXPORTLEAFWITHONLYTRANSFORM = "exportLeafWithOnlyTransform";
	public const string BAKEANIMATIONS 				= "bakeAnimations";
	
	public const string TimestampFormat				= "yyyy-MM-dd'T'HH:mm:ss.fffK"; // RFC3339
	
	private static string									mExporterType;
	private static Dictionary<string,string>				mConverters;
	private static string									mTemplatePath;
	private static Hashtable								mTemplates;
	private	static Hashtable								mConfig;
	private static Hashtable								mSettings;
	private static Hashtable								mOptions;
	private static List<string>								mBasePaths;
	private static string									mSelectedBasePath = "";
	private static string									mSelectedBasePathWithoutSceneName = "";
	private static Hashtable								mPaths;
	private static string									mHighPrecision;
	private static string									mMediumPrecision;
	private static string									mLowPrecision;
	private static Dictionary<string, string> 				mUniforms;
	private static Dictionary<string, string> 				mTextureUniforms;
	private static float									mRenderQueueOffset = 0f;
	private static float									mRenderQueueFactor = 1f;
	private static Dictionary<TextureWrapMode, string>		mWrapModeTransform;
	private static Dictionary<GLexTexture.Filter, string> 	mTextureFilterTransform;
	private static Dictionary<string, string>				mComponentTypeTransform;
	private static Dictionary<string, string>				mLightTypeTransform;
	private static Dictionary<string, string>				mAnimationPropertyTransform;
	private static Dictionary<string, string>				mAnimationWrapModeTransform;
	private static Dictionary<string, float>				mValueTransform;

	public static void Load() {
		string path = Application.dataPath + "/Goo/Editor/Config.json";

		Debug.Log (path);
		if( File.Exists( path )) {
			FileStream 	configAsStream = new FileStream( path, FileMode.Open );
			byte[]     	configAsBytes  = new byte[ configAsStream.Length ];
			string     	configAsString = "";
			
			configAsStream.Read( configAsBytes, 0, (int) configAsStream.Length );
			configAsString = Encoding.UTF8.GetString( configAsBytes );
			mConfig        = MiniJSON.jsonDecode( configAsString ) as Hashtable;

			if( mConfig.ContainsKey( "exporter" )) {
				mExporterType = mConfig[ "exporter" ] as string;
			}
			
			if( mConfig.ContainsKey( "converters" )) {
				mConverters = new Dictionary<string, string>();
				foreach( DictionaryEntry entry in mConfig[ "converters" ] as Hashtable ) {
					mConverters.Add((string) entry.Key, (string) entry.Value );
				}
			}
			
			if( mConfig.ContainsKey( "templates" )) {
				mTemplates = mConfig[ "templates" ] as Hashtable;
			}
			
			if( mConfig.ContainsKey( "settings" )) {
				mSettings = mConfig[ "settings" ] as Hashtable;
	
				// lifting these out now as they need to be accessed
				// very fast as they're used many times
				mHighPrecision   = (string) GetSetting( "highPrecision",   "0.#####" );
				mMediumPrecision = (string) GetSetting( "mediumPrecision", "0.###" );
				mLowPrecision    = (string) GetSetting( "lowPrecision",    "0.#" );
			}
			
			if( mConfig.ContainsKey( "options" )) {
				mOptions = mConfig[ "options" ] as Hashtable;
			}
			
			mUniforms = new Dictionary<string, string>();
			if( mConfig.ContainsKey( "uniforms" )) {
				Hashtable uniforms = mConfig[ "uniforms" ] as Hashtable;
				foreach( DictionaryEntry uniform in uniforms ) {
					mUniforms.Add((string) uniform.Key, (string) uniform.Value );
				}
			}
			
			mTextureUniforms = new Dictionary<string, string>();
			if( mConfig.ContainsKey( "textureUniforms" )) {
				Hashtable uniforms = mConfig[ "textureUniforms" ] as Hashtable;
				foreach( DictionaryEntry uniform in uniforms ) {
					mTextureUniforms.Add((string) uniform.Key, (string) uniform.Value );
				}
			}

			if( mConfig.ContainsKey( "renderQueueTransform" )) {
				mRenderQueueOffset = (float) Convert.ToDouble(( mConfig[ "renderQueueTransform" ] as Hashtable ).ContainsKey( "offset" ) ? ( mConfig[ "renderQueueTransform" ] as Hashtable )[ "offset" ] : 0f );
				mRenderQueueFactor = (float) Convert.ToDouble(( mConfig[ "renderQueueTransform" ] as Hashtable ).ContainsKey( "factor" ) ? ( mConfig[ "renderQueueTransform" ] as Hashtable )[ "factor" ] : 1f );
			}
			
			mWrapModeTransform = new Dictionary<TextureWrapMode, string>();
			if( mConfig.ContainsKey( "wrapModeTransform" )) {
				Hashtable wrapModeTransform = mConfig[ "wrapModeTransform" ] as Hashtable;
				mWrapModeTransform.Add( TextureWrapMode.Repeat, wrapModeTransform.ContainsKey( "Repeat" ) ? (string) wrapModeTransform[ "Repeat" ] : "Repeat" );
				mWrapModeTransform.Add( TextureWrapMode.Clamp,  wrapModeTransform.ContainsKey( "Clamp"  ) ? (string) wrapModeTransform[ "Clamp"  ] : "Clamp"  );
			} else {
				mWrapModeTransform.Add( TextureWrapMode.Repeat, "Repeat" );
				mWrapModeTransform.Add( TextureWrapMode.Clamp,  "Clamp"  );
			}
			
			mTextureFilterTransform = new Dictionary<GLexTexture.Filter, string>();
			Hashtable textureFilterTransform = mConfig.ContainsKey( "textureFilterTransform" ) ? mConfig[ "textureFilterTransform" ] as Hashtable : new Hashtable();
			foreach( GLexTexture.Filter filter in Enum.GetValues( typeof( GLexTexture.Filter ))) {
				mTextureFilterTransform.Add( filter, textureFilterTransform.ContainsKey( filter.ToString()) ? (string) textureFilterTransform[ filter.ToString() ] : filter.ToString());
			}
			
			mComponentTypeTransform = new Dictionary<string, string>();
			Hashtable componentTypeTransform = mConfig.ContainsKey( "componentTypeTransform" ) ? mConfig[ "componentTypeTransform" ] as Hashtable : new Hashtable();
			foreach( DictionaryEntry entry in componentTypeTransform ) {
				mComponentTypeTransform.Add((string) entry.Key, (string) entry.Value );
			}
			
			mLightTypeTransform = new Dictionary<string, string>();
			Hashtable lightTypeTransform = mConfig.ContainsKey( "lightTypeTransform" ) ? mConfig[ "lightTypeTransform" ] as Hashtable : new Hashtable();
			foreach( DictionaryEntry entry in lightTypeTransform ) {
				mLightTypeTransform.Add((string) entry.Key, (string) entry.Value );
			}
			
			mAnimationPropertyTransform = new Dictionary<string, string>();
			Hashtable animationPropertyTransform = mConfig.ContainsKey( "animationPropertyTransform" ) ? mConfig[ "animationPropertyTransform" ] as Hashtable : new Hashtable();
			foreach( DictionaryEntry entry in animationPropertyTransform ) {
				mAnimationPropertyTransform.Add((string) entry.Key, (string) entry.Value );
			}
			
			mAnimationWrapModeTransform = new Dictionary<string, string>();
			Hashtable animationWrapModeTransform = mConfig.ContainsKey( "animationWrapModeTransform" ) ? mConfig[ "animationWrapModeTransform" ] as Hashtable : new Hashtable();
			foreach( DictionaryEntry entry in animationWrapModeTransform ) {
				mAnimationWrapModeTransform.Add((string) entry.Key, (string) entry.Value );
			}
			
			mValueTransform = new Dictionary<string, float>();
			Hashtable valueTransform = mConfig.ContainsKey( "valueTransform" ) ? mConfig[ "valueTransform" ] as Hashtable : new Hashtable();  
			foreach( DictionaryEntry entry in valueTransform ) {
				mValueTransform.Add((string) entry.Key, (float) Convert.ToDouble( entry.Value ));
			}
			
			if( mConfig.ContainsKey( "basePaths" )) {
				ArrayList basePaths = mConfig[ "basePaths" ] as ArrayList;
				mBasePaths          = new List<string>();
				
				string[] scenePaths = GLexSceneSettings.ExportPaths;
	
				for( int p = 0; p < scenePaths.Length; p++ ) {
					mBasePaths.Add( scenePaths[ p ] );
				}
				
				for( int p = 0; p < basePaths.Count; p++ ) {
					mBasePaths.Add((string) basePaths[ p ]);
				}
			} else {
				mBasePaths = new List<string>();
			}
			
			if( mConfig.ContainsKey( "templatePath" )) {
				Hashtable templateSettings = mConfig[ "templatePath" ] as Hashtable;
				bool      relativeUnity = true;
				string    templatePath  = "/Editor/GLex/Templates/";
				if( templateSettings.ContainsKey( "relativeUnityProject" )) {
					relativeUnity = Convert.ToBoolean( templateSettings[ "relativeUnityProject" ] );
				}
				if( templateSettings.ContainsKey( "path" )) {
					templatePath = (string) templateSettings[ "path" ];
				}

				Debug.Log ("relativeUnity: " + relativeUnity);
				Debug.Log ("templatePath: " + templatePath);
				if( relativeUnity ) {
					mTemplatePath = Application.dataPath + templatePath;
				} else {
					mTemplatePath = templatePath;
				}
			}
			
			if( mConfig.ContainsKey( "assetPaths" )) {
				mPaths = mConfig[ "assetPaths" ] as Hashtable;
			}
			
			
			configAsStream.Close();
		} else {
			Debug.LogError( "GLexConfig.Construct: Missing GLexConfig.json at " + path );
			return;
		}
	}

	// exporter, converters and templates
	
	public static string ExporterType {
		get {
			return mExporterType;
		}
	}
	
	public static Dictionary<string,string> Converters {
		get {
			return mConverters;
		}
	}
	
	public static string TemplatePath {
		get {
			return mTemplatePath;
		}
	}
	
	public static string GetTemplate( string template ) {
		Debug.Log (template);
		if( mTemplates != null && mTemplates.ContainsKey( template )) {
			Debug.Log ("found template:" + (string) mTemplates[ template ]);
			return (string) mTemplates[ template ];
		}
		
		//Debug.LogError( "GLexConfig.GetTemplate: Missing template " + template + ", returning \"" + template + "\"" );
		
		return template;
	}
	
	// settings
	
	public static System.Object GetSetting( string setting, System.Object defaultValue = null ) {
		if( mSettings != null && mSettings.ContainsKey( setting )) {
			return mSettings[ setting ];
		}
		return defaultValue;
	}

	public static float Scale {
		get {
			return (float) GetSetting( "scale", 1.0f );
		}
	}
	
	public static float JPEGQuality {
		get {
			return (float) Convert.ToDouble( GetSetting( "jpegQuality", 90f ));
		}
	}
	
	public static float AnimationBakeInterval {
		get {
			return (float) Convert.ToDouble( GetSetting( "bakeAnimationInterval", 0.5f ));
		}
	}
	
	public static string HighPrecision   { get { return mHighPrecision;   } }
	public static string MediumPrecision { get { return mMediumPrecision; }	}
	public static string LowPrecision    { get { return mLowPrecision;    } }

	// options
	
	public static bool GetOption( string option, bool defaultValue = false ) {
		if( mOptions != null && mOptions.ContainsKey( option )) {
			return Convert.ToBoolean( mOptions[ option ] );
		}
		return defaultValue;
	}
	
	public static bool CameraCanHaveParent {
		get {
			return GetOption( CAMERACANHAVEPARENT, false );		
		}
	}
	
	public static bool ExportSceneGameObject {
		get {
			return GetOption( EXPORTSCENEGAMEOBJECT, false );		
		}
	}
	
	public static bool AddSceneNameToPath {
		get {
			return GetOption( ADDSCENENAMETOPATH, false );
		}
	}
	
	public static bool ExportBoneAsGameObject {
		get {
			return GetOption( EXPORTBONEASGAMEOBJECT, true );
		}
	}
	
	public static bool ExportLeafWithOnlyTransform {
		get {
			return GetOption( EXPORTLEAFWITHONLYTRANSFORM, true );
		}
	}
	
	public static bool BakeAnimations {
		get {
			return GetOption( BAKEANIMATIONS, false );
		}
	}
	
	// uniforms
	
	public static Dictionary<string, string> Uniforms {
		get {
			return mUniforms;
		}
	}
	
	public static Dictionary<string, string> TextureUniforms {
		get {
			return mTextureUniforms;
		}
	}
	
	public static string GetTextureUniformFor( string key ) {
		if( mTextureUniforms.ContainsKey( key )) {
			return mTextureUniforms[ key ];
		} else {
			Debug.LogError( "GLexConfig.GetTextureUniformFor: Missing uniform name for " + key + " returning \"error\"" );
			return "error";
		}
	}
	
	// transforms
	
	public static int TransformRenderQueue( int queue ) {
		return Mathf.FloorToInt((float) queue * mRenderQueueFactor + mRenderQueueOffset );
	}
	
	public static string TransformWrapMode( TextureWrapMode mode ) {
		return mWrapModeTransform[ mode ];
	}
	
	public static string TransformTextureFilter( GLexTexture.Filter filter ) {
		return mTextureFilterTransform[ filter ];
	}
	
	public static string TransformComponentType( string type ) {
		return mComponentTypeTransform.ContainsKey( type ) ? mComponentTypeTransform[ type ] : type;	
	}
	
	public static string TransformLightType( LightType type ) {
		if( mLightTypeTransform.ContainsKey( type.ToString())) {
			return mLightTypeTransform[ type.ToString() ];
		}
		return type.ToString();
	}
	
	public static string TransformAnimationWrapMode( WrapMode mode ) {
		if( mAnimationWrapModeTransform.ContainsKey( mode.ToString())) {
			return mAnimationWrapModeTransform[ mode.ToString() ];
		}
		return mode.ToString();
	}
	
	public static string TransformAnimationProperty( string property ) {
		if( mAnimationPropertyTransform.ContainsKey( property )) {
			return mAnimationPropertyTransform[ property ];
		}
		return property;
	}

	public static float TransformValue( float originalValue, string transformKey ) {
		return mValueTransform.ContainsKey( transformKey ) ? originalValue * mValueTransform[ transformKey ] : originalValue;
	}
	
	// paths
	
	public static List<string> BasePaths {
		get {
			return mBasePaths;
		}
	}

	public static string BasePath {
		set {
			if( mSelectedBasePath.LastIndexOf( "/" ) != mSelectedBasePath.Length - 1 ) {
				mSelectedBasePath = value + "/";
			} else {
				mSelectedBasePath = value;
			}
			
			mSelectedBasePathWithoutSceneName = mSelectedBasePath;
			
//			if( AddSceneNameToPath ) {
//				mSelectedBasePath += GLexSceneSettings.FileName + "/";
//			}
		}
		get {
			return mSelectedBasePath;
		}
	}
	
	public static string BasePathWithoutSceneName {
		get {
			return mSelectedBasePathWithoutSceneName;
		}
	}
	
	public static List<string> Paths {
		get {
			List<string> paths = new List<string>();
			foreach( DictionaryEntry pathNameAndSetting in mPaths ) {
				paths.Add( GetPathFor( pathNameAndSetting.Key as string ));	
			}
			return paths;
		}
	}
	
	public static string GetPathFor( string assetType ) {
		return mSelectedBasePath;
	}

	public static string GetLocalPathFor( string assetType ) {
		string absolutPath = GetPathFor( assetType );
		return absolutPath.Substring( BasePath.Length );
	}
	
	public static string GetExtensionFor( string assetType ) {
		if( mPaths != null && mPaths.ContainsKey( assetType )) {
			Hashtable setting = mPaths[ assetType ] as Hashtable;
			if( setting.ContainsKey( "extension" )) {
				return "." + ((string) setting[ "extension" ] );
			}
		}
		return ".json";
	}
}
