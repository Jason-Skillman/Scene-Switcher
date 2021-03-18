using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcherWindow : EditorWindow {
	
	struct SceneObj {
		public string Path { get; set; }
		public string Name => AssetDatabase.GUIDToAssetPath(Path);
	}
	
	private ReorderableList reorderableList;

	private SceneObj[] sceneObjs;
	
	[MenuItem("Window/Scene Switcher")]
	static void OpenWindow() {
		//Get existing open window or if none, make a new one:
		SceneSwitcherWindow window = (SceneSwitcherWindow)GetWindow(typeof(SceneSwitcherWindow));
		window.Show();
	}

	private void OnEnable() {
		CacheScenes();
		
		CreateReorderableList();
	}

	private void OnGUI() {
		
		GUI.Label(new Rect(0, 0, 100, ), "Test");
		
		reorderableList.DoLayoutList();
		
	}

	private void CacheScenes() {
		string[] sceneGUIDs = AssetDatabase.FindAssets("t: " + nameof(Scene));
		sceneObjs = new SceneObj[sceneGUIDs.Length];
		
		for(int i = 0; i < sceneGUIDs.Length; i++) {
			SceneObj scene = new SceneObj();
			scene.Path = sceneGUIDs[i];
			sceneObjs[i] = scene;
		}
	}
	
	private void CreateReorderableList() {
		reorderableList = new ReorderableList(sceneObjs, typeof(SceneObj), true, true, false, false);
		reorderableList.drawElementCallback = DrawElementCallback;
		reorderableList.drawHeaderCallback = DrawHeaderCallback;
		//reorderableList.onReorderCallback = OnReorder;
		reorderableList.headerHeight = 0f;
		reorderableList.elementHeight = 24f;
	}

	private void DrawHeaderCallback(Rect rect) {
		EditorGUI.LabelField(rect, "Scene Switcher");
	}


	private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused) {
		
		GUI.Label(rect, sceneObjs[index].Name);
		
	}
	
	/*private void DrawElementBackgroundCallback(Rect rect, int index, bool isActive, bool isFocused) {
		ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, isFocused, true);
		using (Helper.ReplaceColor.With(kItemBorderColor)) {
			GUI.Box(rect, GUIContent.none, _styles.itemBorder);
		}
	}*/


}