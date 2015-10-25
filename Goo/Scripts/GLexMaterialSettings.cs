using UnityEngine;
using System.Collections;
using System.Reflection;

public class GLexMaterialSettings : MonoBehaviour {

	public enum Blending {
		NoBlending = 0,
		AdditiveBlending,
		SubtractiveBlending,
		MultiplyBlending,
		CustomBlending
	}
	
	public enum BlendEquation {
		AddEquation = 0,
		SubtractEquation,
		ReverseSubtractEquation
	}
	
	public enum BlendSrc {
		SrcAlphaFactor = 0, 
		ZeroFactor,
		OneFactor,
		SrcColorFactor,
		OneMinusSrcColorFactor,
		OneMinusSrcAlphaFactor,
		OneMinusDstAlphaFactor,
		DstColorFactor,
		OneMinusDstColorFactor,
		SrcAlphaSaturateFactor,
		DstAlphaFactor
	}
	
	public enum BlendDst {
		OneMinusSrcAlphaFactor = 0,
		SrcAlphaFactor,
		ZeroFactor,
		OneFactor,
		SrcColorFactor,
		OneMinusSrcColorFactor,
		OneMinusDstAlphaFactor,
		DstColorFactor,
		OneMinusDstColorFactor,
		SrcAlphaSaturateFactor,
		DstAlphaFactor
	}
	
	public enum CullFace {
		Back = 0,
		Front,
		FrontAndBack
	}
	
	public enum FrontFace {
		CCW = 0,
		CW
	}
	
	public Blending 		blending         = Blending.NoBlending;
	public BlendEquation 	blendEquation    = BlendEquation.AddEquation;
	public BlendSrc 		blendSource      = BlendSrc.SrcAlphaFactor;
	public BlendDst 		blendDestination = BlendDst.OneMinusSrcAlphaFactor;
	
	public bool				culling  = true;
	public CullFace         cullFace = CullFace.Back;
	public FrontFace		frontFace = FrontFace.CCW;
	
	public bool				depthTest = true;
	public bool				depthWrite = true;

	public bool				offsetEnabled = false;
	public float			offsetFactor = 1.0f;
	public float			offsetUnits = 1.0f;

	public bool				wireframe = false;
	public bool				flat = false;
	
	public float			zSortOffset = 0f;
	
	public bool 			renderQueueOverrideEnabled = false;
	public int				renderQueue = 0; 
	public int				renderQueueOffset = 0;
	
	public bool Equals( GLexMaterialSettings mat ) {
		bool equals = true;
		
		equals &= blending          		 == mat.blending;
		equals &= blendEquation     		 == mat.blendEquation;
		equals &= blendSource       		 == mat.blendSource;
		equals &= blendDestination  		 == mat.blendDestination;
		equals &= culling           		 == mat.culling;
		equals &= cullFace          		 == mat.cullFace;
		equals &= frontFace         		 == mat.frontFace;
		equals &= depthTest         		 == mat.depthTest;
		equals &= depthWrite        		 == mat.depthWrite;
		equals &= offsetEnabled	    		 == mat.offsetEnabled;
		equals &= offsetFactor      		 == mat.offsetFactor;
		equals &= offsetUnits       		 == mat.offsetUnits;
		equals &= wireframe         		 == mat.wireframe;
		equals &= flat              		 == mat.flat;
		equals &= zSortOffset       		 == mat.zSortOffset;
		equals &= renderQueueOverrideEnabled == mat.renderQueueOverrideEnabled;
		equals &= renderQueue 				 == mat.renderQueue;
		equals &= renderQueueOffset 		 == mat.renderQueueOffset;
	
		return equals;
	}
	
	public bool IsDefault {
		get {
			bool isDefault = true;
			
			isDefault &= blending          			== Blending.NoBlending;
			isDefault &= blendEquation     			== BlendEquation.AddEquation;
			isDefault &= blendSource       			== BlendSrc.SrcAlphaFactor;
			isDefault &= blendDestination  			== BlendDst.OneMinusSrcAlphaFactor;
			isDefault &= culling           			== true;
			isDefault &= cullFace          			== CullFace.Back;
			isDefault &= frontFace         			== FrontFace.CCW;
			isDefault &= depthTest         			== true;
			isDefault &= depthWrite        			== true;
			isDefault &= offsetEnabled     			== false;
			isDefault &= offsetFactor      			== 1f;
			isDefault &= offsetUnits       			== 1f;
			isDefault &= wireframe         			== false;
			isDefault &= flat              			== false;
			isDefault &= zSortOffset       			== 0f;
			isDefault &= renderQueueOverrideEnabled == false;
			isDefault &= renderQueue 				== 0;
			isDefault &= renderQueueOffset 			== 0;
			
			return isDefault;
		}
	}
	
	public string GetSettingAsString( string setting ) {
		FieldInfo field = this.GetType().GetField( setting );
		if( field != null ) {
			if( field.FieldType == typeof( bool )) {
				return field.GetValue( this ).ToString().Equals( "True" ) ? "true" : "false";
			} else {
				return field.GetValue( this ).ToString();
			}
		} else {
			Debug.LogError( "GLexMaterialSetting.GetSettingAsString: Field " + setting + " does not exist, return blank" );
		}
		return "noSuchField-" + setting;
	}
}
