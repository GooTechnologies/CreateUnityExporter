using UnityEngine;
using System.Collections;
using Antlr.StringTemplate;
using System;
using System.Text;

public static class TemplateRenderers {
	public class BoolRenderer : IAttributeRenderer {
		#region IAttributeRenderer implementation
		public string ToString(object o) {
			bool b = (bool)o;
			return b ? "true" : "false";
		}
	
		public string ToString(object o, string formatName) {
			return ToString(o);
		}
		#endregion
		
		public static readonly BoolRenderer Instance = new BoolRenderer();
	}
	
	public class ColorRenderer : IAttributeRenderer {
		#region IAttributeRenderer implementation
		public string ToString(object o) {
			var c = (Color)o;
			return string.Format("[{0}, {1}, {2}, {3}]", c.r, c.g, c.b, c.a);
		}
	
		public string ToString(object o, string formatName) {
			return ToString(o);
		}
		#endregion
		
		public static readonly ColorRenderer Instance = new ColorRenderer();
	}
	
	public class DateTimeRenderer : IAttributeRenderer {
		#region IAttributeRenderer implementation
		public string ToString(object o) {
			var t = (DateTime)o;
			return t.ToString(GLexConfig.TimestampFormat);
		}
	
		public string ToString(object o, string formatName) {
			return ToString(o);
		}
		#endregion
		
		public static readonly DateTimeRenderer Instance = new DateTimeRenderer();
	}
	
	public class VectorRenderer : IAttributeRenderer {
		#region IAttributeRenderer implementation
		public string ToString(object o) {
			if (o is Vector3) {
				var v = (Vector3)o;
				return string.Format("[{0}, {1}, {2}]",
					v.x.ToString(GLexConfig.HighPrecision), 
					v.y.ToString(GLexConfig.HighPrecision), 
					v.z.ToString(GLexConfig.HighPrecision));
			}
			else if (o is Vector2) {
				var v = (Vector2)o;
				return string.Format("[{0}, {1}]",
					v.x.ToString(GLexConfig.HighPrecision), 
					v.y.ToString(GLexConfig.HighPrecision));
			}
			else {
				return string.Empty;
			}
		}
	
		public string ToString(object o, string formatName) {
			return ToString(o);
		}
		#endregion
		
		public static readonly VectorRenderer Instance = new VectorRenderer();
	}
	
	public class MatrixRenderer : IAttributeRenderer {
		#region IAttributeRenderer implementation
		public string ToString(object o) {
			if (o is Matrix4x4) {
				var m = (Matrix4x4)o;
				
				var sb = new StringBuilder();
				sb.Append("[").Append(m[0].ToString(GLexConfig.HighPrecision));
				for (int i = 1; i < 16; ++i) {
					sb.Append(", ").Append(m[i].ToString(GLexConfig.HighPrecision));
				}
				sb.Append("]");
				return sb.ToString();
			}
			else {
				return string.Empty;
			}
		}
	
		public string ToString(object o, string formatName) {
			return ToString(o);
		}
		#endregion
		
		public static readonly MatrixRenderer Instance = new MatrixRenderer();
	}
}
