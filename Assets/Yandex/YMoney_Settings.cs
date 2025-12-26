#if YSDK

using UnityEngine;
using UnityEngine.Audio;

public partial class YMoney
{
    [System.Serializable]
    public class Settings
	{
        [Header("Показ рекламы за вознаграждение")]
        [Range(0, 59)] public int minCDAdReward = 10;
        [Range(0, 59)] public int secCDAdReward = 1;
        [Space]
        [Header("Показ полноэкранной рекламы")]
        [Range(0, 59)] public int minCDAdFull = 5;
        [Range(0, 59)] public int secCDAdFull = 55;
        [Space]
        public int rewardAdFull = 1;
        public float chanceRewardAdFull = 25f;
        [Space]
        [Range(0, 59)]
        public int minOffFirstAdFull = 15;
        public Chance chanceReward;
        [Space]
        [Header("Регулировка громкости")]
        public AudioMixerController audioMixer;
    }

    [System.Serializable]
    public class AudioMixerController
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private string _mixerGroup = "SuperMaster";
        [SerializeField] private float _audioOffValue = -80f;
        [SerializeField] private float _audioOnValue = 0f;

        public void On() => _audioMixer.SetFloat(_mixerGroup, _audioOnValue);
        public void Off() => _audioMixer.SetFloat(_mixerGroup, _audioOffValue);
    }
}
#endif