using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsStorage : Singleton<SettingsStorage>, ILoading
{
	[SerializeField] private string _keyDefaultName = "Guest";
#pragma warning disable 414
    [SerializeField] private string _keyAnonymName = "Anonym";
#pragma warning restore 414
    [SerializeField] private Texture _avatar;
	[SerializeField] private Texture _avatarAnonym;
	[Space]
	[SerializeField] private Profile _profileDesktop = new();
	[SerializeField] private Profile _profileMobile = new();
	[Space]
	[SerializeField] private AudioMixer _audioMixer;
	[SerializeField] private float _audioMinValue = 0.01f;
	[SerializeField] private float _audioMaxValue = 1.5845f;
	private Profile _profileCurrent = null;
	private Profile ProfileCurrent
	{
		get
		{
			if(_profileCurrent == null)
				SetupProfile();
			return _profileCurrent;
		}
		set { _profileCurrent = value; }
	}

	public float MinValue => _audioMinValue;
	public float MaxValue => _audioMaxValue;

	public bool IsFirstStart { get; set; } = true;
	public bool IsDesktop { get; private set; } = true;

	public ModeShowGUI ModeShow
	{
		get => _modeShow;
		set
		{
			if (value != _modeShow)
			{
				_modeShow = value; 
				EventModeShowChange?.Invoke();
				Save(false);
			}
		}
	}
	private ModeShowGUI _modeShow = ModeShowGUI.Default;
	public event Action EventModeShowChange;

	public void SetVolume(MixerGroup type, float volume) => _audioMixer.SetFloat(type.ToString(), ConvertToDB(volume));
	public float GetVolume(MixerGroup type) => ProfileCurrent.volumes[type.ToInt()];

	public void Save(bool isSaveHard = true, Action<bool> callback = null)
	{
		ProfileCurrent.idLang = Localization.Inst.CurrentIdLang;
		ProfileCurrent.quality = QualitySettings.GetQualityLevel();
		ProfileCurrent.modeShow = ModeShow;
		foreach (var mixer in Enum<MixerGroup>.GetValues())
		{
			_audioMixer.GetFloat(mixer.ToString(), out float volumeDB);
			ProfileCurrent.volumes[mixer.ToInt()] = MathF.Round(ConvertFromDB(volumeDB), 3);
		}

		Storage.Save(ProfileCurrent.key, ProfileCurrent, isSaveHard, callback);
	}

	public bool Load()
	{
		var (result, value) = Storage.Load<Profile>(ProfileCurrent.key);
		if (result)
		{
			ProfileCurrent.Copy(value);
			Apply();
		}

		return result;
	}

	public void Create()
	{
		SetupProfile();

#if YSDK
		if (YandexSDK.Inst.IsInitialize)
			if (Localization.Inst.TryIdFromCode(YandexSDK.Inst.Lang, out int id))
				ProfileCurrent.idLang = id;
#endif
 
		Apply();
	}

	private void SetupProfile()
	{

#if !UNITY_EDITOR
		if (YandexSDK.Inst.IsPlayer)
			IsDesktop = YandexSDK.Inst.IsDesktop;
		else
			IsDesktop = !UnityJS.IsMobile;
#endif

		_profileCurrent ??= new();
		if (IsDesktop)
		{
			_profileCurrent = _profileDesktop.Clone();
		}
		else
		{
			_profileCurrent = _profileMobile.Clone();
			Screen.orientation = ScreenOrientation.LandscapeLeft;
		}
	
	}

	public void Apply()
	{
		Localization.Inst.SwitchLanguage(ProfileCurrent.idLang);
		QualitySettings.SetQualityLevel(ProfileCurrent.quality, true);
		_modeShow = ProfileCurrent.modeShow;
		foreach (var mixer in Enum<MixerGroup>.GetValues())
			SetVolume(mixer, ProfileCurrent.volumes[mixer.ToInt()]);
	}

	public async UniTaskVoid Personalization(Text caption, RawImage avatar)
	{
#if YSDK
		if (YandexSDK.Inst.IsLogOn)
		{
			string name = YandexSDK.Inst.PlayerName;
			if (!string.IsNullOrEmpty(name))
				caption.text = name;
			else
				caption.text = Localization.Inst.GetText(_keyAnonymName);

			var (result, texture) = await YandexSDK.Inst.GetPlayerAvatar(AvatarSize.Medium);
			if (result)
				avatar.texture = texture;
			else
				avatar.texture = _avatarAnonym;

		}
		else
#endif
		{
			caption.text = Localization.Inst.GetText(_keyDefaultName);
			avatar.texture = _avatar;
		}
		await UniTask.CompletedTask;
	}

	public void Personalization(Text caption)
	{

#if YSDK
		if (YandexSDK.Inst.IsLogOn)
		{
			string name = YandexSDK.Inst.PlayerName;
			if (!string.IsNullOrEmpty(name))
				caption.text = name;
			else
				caption.text = Localization.Inst.GetText(_keyAnonymName);
		}
		else
#endif
		{
			caption.text = Localization.Inst.GetText(_keyDefaultName);
		}

	}

	private float ConvertToDB(float volume)
	{
		volume = Mathf.Log10(volume) * 40f;
		if (volume > 0) volume *= 2.5f;

		return volume;
	}

	private float ConvertFromDB(float dB)
	{
		if (dB > 0) dB /= 2.5f;
		dB = Mathf.Pow(10, dB / 40f);

		return dB;
	}


	[System.Serializable]
	private class Profile
	{
		[JsonIgnore]
		public string key = "stts";
		[JsonProperty("ilg")]
		public int idLang = 1;
		[JsonProperty("vls")]
		public float[] volumes = { 1f, 1f, 1f, 1f };
		[JsonProperty("qlt")]
		public int quality = 2;
		[JsonProperty("msw")]
		public ModeShowGUI modeShow = ModeShowGUI.Default;

		[JsonConstructor]
		public Profile(int idLang, float[] volumes, int quality, ModeShowGUI modeShow)
		{
			this.idLang = idLang;
			this.quality = quality;
			volumes.CopyTo(this.volumes, 0);
			this.modeShow = modeShow;
		}

		public Profile() { }

		public void Copy(Profile profile)
		{
			if (profile == null) return;

			idLang = profile.idLang;
			quality = profile.quality;
			volumes = profile.volumes;
			modeShow = profile.modeShow;
		}

		public Profile Clone()
		{
			Profile profile = new(idLang, volumes, quality, modeShow)
			{
				key = key
			};
			return profile;
		}

	}
}
