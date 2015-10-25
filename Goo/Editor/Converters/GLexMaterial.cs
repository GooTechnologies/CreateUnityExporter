using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GLexMaterial : GLexComponent {
	private static readonly Dictionary<string, string> ExportedTextureSlots = new Dictionary<string, string> {
		{ "diffuse", "DIFFUSE_MAP" },
		{ "normal", "NORMAL_MAP" },
		{ "specular", "SPECULAR_MAP" },
		{ "emissive", "EMISSIVE_MAP" },
		{ "AO", "AO_MAP" },
		{ "light", "LIGHT_MAP" },
	};
	
	
	public class Uniform {
		public string Name;
		public string Type;
		public string Value;
		
		public Uniform(Material material, string uniform, string type) {
			Type = type;
			Name = uniform;
			
			if (Type.Equals("float")) {
				float f = material.GetFloat(Name);
				Value = f.ToString(GLexConfig.HighPrecision);
			}
			else if (Type.Equals("color") || Type.Equals("vector3")) {
				Color color = material.GetColor(Name);
				Value = "[ " + color.r.ToString(GLexConfig.MediumPrecision) + ", " + color.g.ToString(GLexConfig.MediumPrecision) + ", " + color.b.ToString(GLexConfig.MediumPrecision) + " ]";
			}
			else if (Type.Equals("color32") || Type.Equals("vector4")) {
				Vector4 color32 = material.GetVector(Name);
				Value = "[ " + color32.x.ToString(GLexConfig.MediumPrecision) + ", " + color32.y.ToString(GLexConfig.MediumPrecision) + ", " + color32.z.ToString(GLexConfig.MediumPrecision) + ", " + color32.z.ToString(GLexConfig.MediumPrecision) + " ]";
			}
		}
	}
	
	private static int									mUniqueId;
	private static List<GLexMaterial>					mMaterials;
	private static GLexMaterial[]						mMaterialsAsArray;
	private	Material									mMaterial;
	private GLexMaterialSettings						mMaterialSettings;
	private string										mUniqueName;
	private List<GLexTexture>							mTextures;
	private GLexShader									mShader;
	
	public GLexMaterial(Material material, Component meshRenderer) {
		mMaterial = material;
		mMaterialSettings = meshRenderer.GetComponent<GLexMaterialSettings>();
		
		mUniqueName = mMaterial.name;
		while (UniqueNameExists( mUniqueName )) {
			mUniqueName = mMaterial.name + (mUniqueId++).ToString();
		}
		
		mMaterials.Add(this);

		AddTextures();
		AddShader();
	}
	
	public static void Reset() {
		mUniqueId = 0;
		mMaterials = new List<GLexMaterial>();
	}
	
	new public static void PrepareForExport() {
		mMaterialsAsArray = mMaterials.ToArray();
	}

	private void AddTextures() {
		mTextures = new List<GLexTexture>();
		
		foreach (KeyValuePair<string, string> uniform in GLexConfig.TextureUniforms) {
			if (mMaterial.HasProperty(uniform.Value)) {
				Texture2D texture = mMaterial.GetTexture(uniform.Value) as Texture2D;
				mTextures.Add(GLexTexture.Get(texture));
			}
		}
	}
	
	private void AddShader() {
		if (!GLexShader.Exists(mMaterial.shader)) {
			mShader = new GLexShader(mMaterial.shader);
		}
		else {
			mShader = GLexShader.Get(mMaterial.shader);
		}
	}
	
	// texture access methods
	
	private bool HasTextureOn(string uniform) {
		if (mMaterial.HasProperty(GLexConfig.GetTextureUniformFor(uniform))) {
			return mMaterial.GetTexture(GLexConfig.GetTextureUniformFor(uniform)) != null;
		}
		else {
			return false;
		}
	}
	
	private Texture2D GetTextureOn(string uniform) {
		if (HasTextureOn(uniform)) {
			return mMaterial.GetTexture(GLexConfig.GetTextureUniformFor(uniform)) as Texture2D;
		}
		else {
			Debug.LogError("GLexMaterial.GetTextureOn: " + uniform + " doesn't exist");
			return null;
		}
	}
	
	private string GetTextureNameOn(string uniform) {
		if (HasTextureOn(uniform)) {
			return GetTextureOn(uniform).name;
		}
		else {
			Debug.LogError("GLexMaterial.GetTextureNameFor: " + uniform + " doesn't exist");
			return "error";
		}
	}
	
	private string GetTextureOffetFor(string uniform) {
		if (HasTextureOn(uniform)) {
			Vector2 offset = mMaterial.GetTextureOffset(uniform);
			return offset.x + ", " + offset.y;
		}
		else {
			Debug.LogError("GLexMaterial.GetOffetFor: " + uniform + " doesn't exist");
			return "error";
		}
	}

	private string GetTextureScalingFor(string uniform) {
		if (HasTextureOn(uniform)) {
			Vector2 scale = mMaterial.GetTextureScale(uniform);
			return scale.x + ", " + scale.y;
		}
		else {
			Debug.LogError("GLexMaterial.GetOffetFor: " + uniform + " doesn't exist");
			return "error";
		}
	}
	
	protected override string IdExtension {
		get {
			return GLexConfig.GetExtensionFor("material");
		}
	}
	
	// template interface starts here

	public override string Name {
		get {
			return NamesUtil.CleanMaterial(mUniqueName);
		}
	}
	
	public string ShaderName {
		get {
			return NamesUtil.CleanShader(mMaterial.shader.name);
		}
	}
	
	public GLexMaterialSettings Settings {
		get {
			return mMaterialSettings;
		}
	}
	
	public KeyValuePair<string, string>[] BoundTextures {
		get {
			var lst = new List<KeyValuePair<string, string>>();
			
			foreach (var kvp in ExportedTextureSlots) {
				var slotName = GLexConfig.GetTextureUniformFor(kvp.Key);
				var slotTexture = mMaterial.GetTexture(slotName);
				Debug.Log ("Texture: " + kvp.Key + " - " + slotName + " = " + slotTexture);
				if (slotTexture == null) {
					continue;
				}
				
				foreach (var tex in mTextures) {
					if (tex != null && tex.Texture == slotTexture) {
						lst.Add(new KeyValuePair<string, string>(kvp.Value, tex.Id));
						break;
					}
				}
			}
			
			return lst.ToArray();
		}
	}
	
	public Uniform[] Uniforms {
		get {
			List<Uniform> uniforms = new List<Uniform>();
		
			foreach (KeyValuePair<string, string> uniformAndType in GLexConfig.Uniforms) {
				string uniform = uniformAndType.Key;
				string type = uniformAndType.Value;
				if (mMaterial.HasProperty(uniform)) {
					uniforms.Add(new Uniform(mMaterial, uniform, type));
				}
			}

			
			return uniforms.ToArray();
		}
	}
	
	public string RenderQueue {
		get {
			int renderQueue = GLexConfig.TransformRenderQueue(mMaterial.renderQueue);
			int renderQueueOffset = 0;
			
			if (mMaterialSettings != null) {
				renderQueueOffset = mMaterialSettings.renderQueueOffset;
			
				if (mMaterialSettings.renderQueueOverrideEnabled) {
					renderQueue = mMaterialSettings.renderQueue;
				}
			}
			
			return (renderQueue + renderQueueOffset).ToString();
		}
	}
	
	public string Blending 			{ get { return GetSetting("blending", GLexMaterialSettings.Blending.NoBlending); } }

	public string BlendEquation 	{ get { return GetSetting("blendEquation", GLexMaterialSettings.BlendEquation.AddEquation); } }

	public string BlendSource 		{ get { return GetSetting("blendSource", GLexMaterialSettings.BlendSrc.SrcAlphaFactor); } }

	public string BlendDestination 	{ get { return GetSetting("blendDestination", GLexMaterialSettings.BlendDst.OneMinusSrcAlphaFactor); } }

	public string Culling 			{ get { return GetSetting("culling", true); } }

	public string CullFace 			{ get { return GetSetting("cullFace", GLexMaterialSettings.CullFace.Back); } }

	public string FrontFace 		{ get { return GetSetting("frontFace", GLexMaterialSettings.FrontFace.CCW); } }

	public string DepthTest 		{ get { return GetSetting("depthTest", true); } }

	public string DepthWrite 		{ get { return GetSetting("depthWrite", true); } }

	public string OffsetEnabled 	{ get { return GetSetting("offsetEnabled", false); } }

	public string OffsetFactor 		{ get { return GetSetting("offsetFactor", 1f); } }

	public string OffsetUnits 		{ get { return GetSetting("offsetUnits", 1f); } }

	public string IsWireframe 		{ get { return GetSetting("wireframe", false); } }

	public string IsFlat 			{ get { return GetSetting("flat", false); } }

	public string ZSortOffset      	{ get { return GetSetting("zSortOffset", 0); } }
	
	private string GetSetting(string property, System.Object defaultValue) {
		if (mMaterialSettings != null) {
			return mMaterialSettings.GetSettingAsString(property);
		}
		if (defaultValue.GetType() == typeof(bool)) {
			return defaultValue.ToString().Equals("True") ? "true" : "false";
		}
		else {
			return defaultValue.ToString();
		}
	}
	
	public GLexTexture[] Textures {
		get {
			return mTextures.ToArray();
		}
	}
	
	public GLexShader Shader {
		get {
			return mShader;
		}
	}
	
	// static
	
	public static GLexMaterial[] MaterialsAsArray {
		get {
			return mMaterialsAsArray;
		}
	}

	public static bool Exists(string name, Component meshRenderer) {
		foreach (GLexMaterial material in mMaterials) {
			if (NamesUtil.CleanMaterial(material.mMaterial.name) == name && HasSameMaterialSettings(material.mMaterialSettings, meshRenderer.GetComponent<GLexMaterialSettings>())) {
				return true;
			}
		}
		return false;
	}
	
	public static bool UniqueNameExists(string uniqueName) {
		foreach (GLexMaterial material in mMaterials) {
			if (material.mUniqueName == uniqueName) {
				return true;
			}
		}
		return false;
	}

	private static bool HasSameMaterialSettings(GLexMaterialSettings a, GLexMaterialSettings b) {
		bool aIsDefault = b == null ? true : b.IsDefault;
		bool bIsDefault = a == null ? true : a.IsDefault;
		
		if (aIsDefault && bIsDefault) {
			return true;
		}
		else if (b != null && a != null) {
			return b.Equals(a);
		}
		else {
			return false;
		}
	}
	
	public static GLexMaterial Get(Material material, Component meshRenderer) {
		string name = NamesUtil.CleanMaterial(material.name);
		
		if (Exists(name, meshRenderer)) {
			return Get(name, meshRenderer);
		}
		else {
			return new GLexMaterial(material, meshRenderer);
		}
	}
	
	public static GLexMaterial Get(string name, Component meshRenderer) {
		foreach (GLexMaterial material in mMaterials) {
			if (NamesUtil.CleanMaterial(material.mMaterial.name) == name && HasSameMaterialSettings(material.mMaterialSettings, meshRenderer.GetComponent<GLexMaterialSettings>())) {
				return material;
			}
		}
		
		Debug.LogError("GLexMaterial.Get: Trying to get " + name + " but it doesn't exists!");
		return null;
	}
}
