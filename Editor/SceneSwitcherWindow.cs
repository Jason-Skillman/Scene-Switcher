using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

public class SceneSwitcherWindow : EditorWindow {
	
	private struct SceneAsset {
		public string GUID { get; private set; }
		public string Path => AssetDatabase.GUIDToAssetPath(GUID);
		public string Name {
			get {
				//Remove the file's path
				int index = 0;
				for(var i = 0; i < Path.Length; i++) {
					char c = Path[i];
					if(c.Equals('/'))
						index = i;
				}

				int extLength = 6;	//.unity

				return Path.Substring(index + 1, Path.Length - index - 1 - extLength);
			}
		}

		public SceneAsset(string guid) {
			GUID = guid;
		}
	}

	private ReorderableList reorderableList;
	private SceneAsset[] sceneObjs;
	private string filter;
	
	[MenuItem("Window/Scene Switcher")]
	private static void OpenWindow() {
		SceneSwitcherWindow window = (SceneSwitcherWindow)GetWindow(typeof(SceneSwitcherWindow), false, "Scene Switcher");
		window.Show();
	}

	private void OnGUI() {
		//Draw search bar
		filter = EditorGUILayout.TextField("Search", filter);

		UpdateList();

		reorderableList.DoList(new Rect(0.0f, 30.0f, position.width, position.height));
	}

	private void CacheScenes() {
		string[] sceneGUIDs = AssetDatabase.FindAssets("t: " + nameof(UnityEngine.SceneManagement.Scene) + " " + filter);
		sceneObjs = new SceneAsset[sceneGUIDs.Length];
		
		for(int i = 0; i < sceneGUIDs.Length; i++) {
			SceneAsset sceneAsset = new SceneAsset(sceneGUIDs[i]);
			sceneObjs[i] = sceneAsset;
		}
	}
	
	private void CreateReorderableList() {
		reorderableList = new ReorderableList(sceneObjs, typeof(SceneAsset), false, false, false, false);
		reorderableList.drawElementCallback = DrawElementCallback;
		reorderableList.elementHeight = 24.0f;
	}

	private void UpdateList() {
		CacheScenes();
		CreateReorderableList();
	}
	
	private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused) {
		Rect currentRect = rect;

		//Draw single load button
		{
			currentRect.width = 20.0f;
			currentRect.height = 18.0f;
			currentRect.y += (rect.height - currentRect.height) / 2;
			if(GUI.Button(currentRect, new GUIContent("S", "Opens the scene"))) {
				LoadScene(index);
			}
		}
		
		//Draw additive load button
		{
			currentRect.x += 25;
			currentRect.width = 20.0f;
			currentRect.height = 18.0f;
			if(GUI.Button(currentRect, new GUIContent("A", "Opens the scene additively"))) {
				LoadScene(index, OpenSceneMode.Additive);
			}
		}
		
		//Draw label
		{
			currentRect.x += 40.0f;
			currentRect.width = 200.0f;
			GUI.Label(currentRect, new GUIContent(sceneObjs[index].Name, sceneObjs[index].Path));
		}
	}

	private void LoadScene(int index, OpenSceneMode mode = OpenSceneMode.Single) {
		EditorSceneManager.OpenScene(sceneObjs[index].Path, mode);
	}

}