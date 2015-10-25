using UnityEngine;
using System.Collections;

public class GLexLight : GLexComponent {
	
	new protected Light mComponent;
	protected GLexTexture mCookieTexture;
	
	public GLexLight() : base() {
	}
	
	// overrides
	public override void AssociateWithComponent(object obj) {
		base.AssociateWithComponent(obj);
		mComponent = (Light)obj;
			
		if (mComponent.cookie != null) {
			mCookieTexture = GLexTexture.Get(mComponent.cookie as Texture2D);
		}
	}

	public bool IsLight {
		get { return true; }
	}
	
	public bool IsPointLight {
		get {
			return mComponent.type == LightType.Point;
		}
	}

	public bool IsDirectionalLight {
		get {
			return mComponent.type == LightType.Directional || mComponent.type == LightType.Area;
		}
	}
	
	public bool IsSpotLight {
		get {
			return mComponent.type == LightType.Spot;
		}
	}
	
	public bool IsAreaLight {
		get {
			return mComponent.type == LightType.Area;
		}
	}
	
	public string TypeOfLight {
		get {
			return GLexConfig.TransformLightType(mComponent.type);
		}
	}
	
	public bool IsCastingShadows {
		get {
			return mComponent.shadows != LightShadows.None;
		}
	}
	
	public string[] Color {
		get {
			Color c = mComponent.color;
			return new string[] { c.r.ToString(GLexConfig.MediumPrecision), c.g.ToString(GLexConfig.MediumPrecision), c.b.ToString(GLexConfig.MediumPrecision) };
		}
	}

	public string[] Color32 {
		get {
			Color c = mComponent.color;
			return new string[] { c.r.ToString(GLexConfig.MediumPrecision), c.g.ToString(GLexConfig.MediumPrecision), c.b.ToString(GLexConfig.MediumPrecision), c.a.ToString(GLexConfig.MediumPrecision) };
		}
	}
	
	//dunno.
	public float Exponent {
		get {
			return 1f;
		}
	}
	
	public float Intensity {
		get {
			return mComponent.intensity;
		}
	}
	
	public float Range {
		get {
			return mComponent.range;
		}
	}
	
	public float SpecularIntensity {
		get {
			return 0.2f;
		}
		
	}
	
	public float Darkness {
		get {
			return 0.2f;
		}
	}
	
	public float Far {
		get {
			return 100;
		}
	}
	
	public float Near {
		get {
			return 0.1f;	
		}
	}
	
	public float Fov {
		
		get {
			if (IsSpotLight) {
				return float.Parse(SpotAngle);
			}
			else {
				return 180f;
			}
		}
	}
	
	public string LightCookieTextureRef {
		get {	
			return mCookieTexture.Id;
		}	
	}
	
	public string[] ShadowResolution {
		get {
			//Vector2 res =QualitySettings.s//QualitySettings.shadowProjection;//mComponent.light.//QualitySettings.//Application.//QualitySettings.GetQualityLevel()
			Vector2 p = Vector2.one;
			return new string[]{ (p.x).ToString(GLexConfig.MediumPrecision), (p.y).ToString(GLexConfig.MediumPrecision)};
		}
	}
	
	public string SpotAngle {
		get {
			return mComponent.spotAngle.ToString(GLexConfig.HighPrecision);
		}
	}
	
	public string[] Direction {
		get {
			Vector3 p = mComponent.transform.forward;
			return new string[] { (p.x).ToString(GLexConfig.MediumPrecision), (p.y).ToString(GLexConfig.MediumPrecision), (-p.z).ToString(GLexConfig.MediumPrecision) };
		}
	}
	
	public bool LightCookieEnabled {
		get {
			
			return mComponent.type == LightType.Directional && mComponent.cookie != null;
		}
	}
	
	public float Penumbra {
		get {
			return mComponent.shadowSoftness;
		}
	}

	public string ShadowType {
		get {
			return (mComponent.shadows == LightShadows.Hard ? "Basic" : "PCF");	
		}
	}
	
	public float Size {
		get {
			return 1;//find out.
		}
	}
}
