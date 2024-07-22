using UnityEngine;

namespace com.homemade.modules.audio
{
    public class PlaySound : MonoBehaviour
    {
        [Header("Sound Name")]
        [SerializeField] private string soundName;

        public void OnPlaySound()
        {
            AudioController.Instance.PlaySound(soundName);
        }
    }
}
