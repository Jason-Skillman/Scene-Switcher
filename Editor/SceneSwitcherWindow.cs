using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcherWindow : EditorWindow {
	
	struct SceneObj {
		public string GUID { get; set; }
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

				int extentionLength = 6;	//.unity

				return Path.Substring(index + 1, Path.Length - index - 1 - extentionLength);
			}
		}
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
		reorderableList.DoLayoutList();
	}

	private void CacheScenes() {
		string[] sceneGUIDs = AssetDatabase.FindAssets("t: " + nameof(Scene));
		sceneObjs = new SceneObj[sceneGUIDs.Length];
		
		for(int i = 0; i < sceneGUIDs.Length; i++) {
			SceneObj scene = new SceneObj();
			scene.GUID = sceneGUIDs[i];
			sceneObjs[i] = scene;
		}
	}
	
	private void CreateReorderableList() {
		reorderableList = new ReorderableList(sceneObjs, typeof(SceneObj), false, true, false, false);
		reorderableList.drawElementCallback = DrawElementCallback;
		reorderableList.headerHeight = 0.0f;
		reorderableList.elementHeight = 24.0f;
	}
	
	private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused) {
		Rect currentRect = rect;

		//Draw single load button
		{
			currentRect.width = 20.0f;
			currentRect.height = 18.0f;
			currentRect.y += (rect.height - currentRect.height) / 2;
			if(GUI.Button(currentRect, "S")) {
				LoadScene(index);
			}
		}
		
		//Draw additive load button
		{
			currentRect.x += 25;
			currentRect.width = 20.0f;
			currentRect.height = 18.0f;
			if(GUI.Button(currentRect, "A")) {
				LoadScene(index, OpenSceneMode.Additive);
			}
		}
		
		//Draw label
		{
			currentRect.x += 50.0f;
			//currentRect.y = rect.y;
			currentRect.width = 200.0f;
			GUI.Label(currentRect, sceneObjs[index].Name);
		}
	}

	private void LoadScene(int index, OpenSceneMode mode = OpenSceneMode.Single) {
		EditorSceneManager.OpenScene(sceneObjs[index].Path, mode);
	}

}