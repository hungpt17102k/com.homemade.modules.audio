using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace com.homemade.modules.audio
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(AudioController), true)]
    public class AudioControllerEditor : Editor
    {
        private static string PACKAGE_NAME = "com.homemade.modules.audio";

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
                string nameClip = "SoundClips";
                GenerateClips(nameClip, PATH_RESOURCES_SOUND);
            }

            if (GUILayout.Button("Generate Music Clip Names Enum"))
            {
                string nameClip = "MusicClips";
                GenerateClips(nameClip, PATH_RESOURCES_MUSIC);
            }
        }

        /// <summary>
        /// Creates the soundClips enum at the soundManager's location
        /// </summary>
        public void GenerateClips(string enumName, string path)
        {
            //Get the list of audios
            List<string> listSound = GetListAudio(path);

            //Get the script's path
            string folderPath = Path.Combine(Application.dataPath, "Scripts/Generated");
            string scriptPath = Path.Combine(folderPath, $"{enumName}.cs");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            //Wrire the enum at above path
            using (StreamWriter streamWriter = new StreamWriter(scriptPath))
            {
                streamWriter.WriteLine($"namespace {PACKAGE_NAME}" + "\n" + "{");
                streamWriter.WriteLine("\tpublic class " + enumName);
                streamWriter.WriteLine("\t{");
                for (int i = 0; i < listSound.Count; i++)
                {
                    streamWriter.WriteLine($"\t\tpublic const string {listSound[i]} = \"{listSound[i]}\";");
                }
                streamWriter.WriteLine("\t}");
                streamWriter.WriteLine("}");
            }
            AssetDatabase.Refresh();

            Debug.Log($"SoundClips  enum created/updated at {scriptPath}");
        }

        private List<string> GetListAudio(string path)
        {
            List<string> listAudio = new List<string>();
            DirectoryInfo directoryPath = new DirectoryInfo(Path.Combine(Application.dataPath, path));
            FileInfo[] fileInfo = directoryPath.GetFiles("*.*", SearchOption.AllDirectories);

            foreach (FileInfo file in fileInfo)
            {
                if (file.Extension != ".meta")
                {
                    string[] text = file.Name.Split('.');
                    listAudio.Add(text[0]);
                }
            }

            return listAudio;
        }
    }

#endif
}