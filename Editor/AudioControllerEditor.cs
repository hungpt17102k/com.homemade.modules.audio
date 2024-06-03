using System.Collections.Generic;
using UnityEngine;
using System.IO;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AudioController), true)]
public class AudioControllerEditor : Editor
{
    private static string PATH_RESOURCES_SOUND = "Resources/Audio/Sound";
    private static string PATH_RESOURCES_MUSIC = "Resources/Audio/Music";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("");
        EditorGUILayout.HelpBox("After assigning Audio clips, Hit this button to get an enum of audioclip names created/updated. Makes method calls easy.\n ** NOTE : Make sure the name is a valid enum name **\n" +
                                " Example - AudioController.Instance.PlaySound(SoundClips.Demo);", MessageType.Info);

        if (GUILayout.Button("Generate Sound Clip Names Enum"))
        {
            GenerateClipsEnum(PATH_RESOURCES_SOUND);
        }

        if (GUILayout.Button("Generate Music Clip Names Enum"))
        {
            GenerateClipsEnum(PATH_RESOURCES_MUSIC);
        }
    }

    /// <summary>
    /// Creates the soundClips enum at the soundManager's location
    /// </summary>
    public void GenerateClipsEnum(string path)
    {
        //Get the list of audios
        AudioController soundManager = (AudioController)target;
        List<string> listSound = GetListAudio(path);

        //Get the script's path
        MonoScript thisScript = MonoScript.FromMonoBehaviour(soundManager);
        string ScriptFilePath = AssetDatabase.GetAssetPath(thisScript);

        //Create a path for the enum file
        string enumName = "SoundClips";
        string filePathAndName = ScriptFilePath.Replace(thisScript.name + ".cs", "") + "/" + enumName + ".cs";

        //Wrire the enum at above path
        using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
        {
            streamWriter.WriteLine("public enum " + enumName);
            streamWriter.WriteLine("{");
            for (int i = 0; i < listSound.Count; i++)
            {
                streamWriter.WriteLine("\t" + listSound[i] + ",");
            }
            streamWriter.WriteLine("}");
        }
        AssetDatabase.Refresh();

        Debug.Log($"SoundClips  enum created/updated at {filePathAndName}");
    }

    private List<string> GetListAudio(string path)
    {
        List<string> listAudio = new List<string>();
        DirectoryInfo directoryPath = new DirectoryInfo(Path.Combine(Application.dataPath, path));
        FileInfo[] fileInfo = directoryPath.GetFiles("*.*", SearchOption.AllDirectories);

        foreach (FileInfo file in fileInfo)
        {
            if(file.Extension != ".meta")
            {
                string[] text = file.Name.Split('.');
                listAudio.Add(text[0]);
            }
        }

        return listAudio;
    }
}

#endif