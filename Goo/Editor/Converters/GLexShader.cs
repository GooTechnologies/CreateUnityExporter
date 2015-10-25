using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GLexShader : GLexComponent {

	private static List<GLexShader>	mShaders;
	private static GLexShader[]		mShadersAsArray;
	
	private Shader					mShader;
	
	public GLexShader( Shader shader ) : base() {
		mShader = shader;
		mShaders.Add( this );
	}
	
	public static void Reset() {
		mShaders = new List<GLexShader>();
	}
	
	new public static void PrepareForExport() {
		mShadersAsArray = mShaders.ToArray();
	}
	
	public Shader Shader {
		get {
			return mShader;
		}
	}
	
	public override string Name {
		get {
			return NamesUtil.CleanShader( mShader.name );
		}
	}
	
	protected override string IdExtension {
		get {
			return GLexConfig.GetExtensionFor("shader");
		}
	}
	
	// static 

	public static GLexShader[] ShadersAsArray {
		get {
			return mShadersAsArray;
		}
	}
	
	public static bool Exists( Shader shader ) {
		if( shader != null ) {
			foreach( GLexShader glexShader in mShaders ) {
				if( glexShader.Shader == shader ) {
					return true;
				}
			}
			
			return false;
		} else {
			return true;		// return true for null shaders to avoid creation of empty GLexShader
		}
	}
	
	public static GLexShader Get( Shader shader ) {
		if( Exists( shader )) {
			foreach( GLexShader glexShader in mShaders ) {
				if( glexShader.Shader == shader ) {
					return glexShader;
				}
			}
		} else {
			Debug.LogError( "GLexShader.Get: Trying to get " + shader.name + " that hasn't been added yet. Please check so it .Exists() first!" );
		}
		return null;
	}
}
