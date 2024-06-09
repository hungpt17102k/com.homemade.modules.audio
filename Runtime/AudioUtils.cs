using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace com.homemade.modules.audio
{
    public static class AudioUtils
    {
        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetString(key, value.ToString());
        }

        public static bool GetBool(string key)
        {
            bool value = Convert.ToBoolean(PlayerPrefs.GetString(key));

            return value;
        }

        public static T RandomEnumValue<T>()
        {
            System.Random _R = new System.Random();
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(_R.Next(v.Length));
        }

        public static IEnumerator FadeVolum(this AudioSource source, float resultValue, float time, Action callBack)
        {
            float startValue = source.volume;
            float timeElapsed = startValue;
            float timer = time;

            while (source.volume > 0)
            {
                source.volume = Mathf.Lerp(startValue, resultValue, timeElapsed / time);
                timeElapsed += Time.deltaTime;
                timer -= Time.deltaTime;
                yield return null;
            }

            callBack?.Invoke();
        }

        public static IEnumerator OnComplete(this AudioCase audioCase, Action callback)
        {
            audioCase.OnAudioEnded += callback;

            yield return new WaitForSeconds(audioCase.source.clip.length);

            audioCase.Complete();
            audioCase.OnAudioEnded -= callback;
        }

        public static List<string> GetAllNameFile(string path)
        {
            List<string> names = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] info = dir.GetFiles("*.*");

            foreach (FileInfo f in info)
            {
                names.Add(f.Name);
            }

            return names;
        }
    }
}