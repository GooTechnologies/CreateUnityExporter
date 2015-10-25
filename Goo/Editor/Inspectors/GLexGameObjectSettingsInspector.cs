using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(GLexGameObjectSettings))]
public class GLexGameObjectSettingsInspector : Editor {
	private GLexGameObjectSettings	mSettings;
	
	public override void OnInspectorGUI() {
		mSettings =	target as GLexGameObjectSettings;

		EditorGUIUtility.LookLikeControls();
		base.OnInspectorGUI();

		GUILayout.BeginHorizontal();
		GUILayout.Label( "Exported as: ", GUILayout.Width( 145f ));
		GUILayout.Label( mSettings.UniqueName );
		GUILayout.EndHorizontal();
	}
}
