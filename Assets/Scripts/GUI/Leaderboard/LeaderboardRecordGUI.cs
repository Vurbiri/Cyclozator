using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LeaderboardRecordGUI : MonoBehaviour
{
    [SerializeField] private Text _rankText;
    [SerializeField] private RawImage _avatarRawImage;
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _scoreText;
    [SerializeField] private Color _colorText;
    [Space]
    [SerializeField] private string _keyAnonymName = "Anonym";
    [SerializeField] private Texture _avatarAnonym;
    [Space]
    [SerializeField] private Image _fonImage;
    [SerializeField] private Image _maskAvatar;
    [SerializeField] private Color _fonNormal;
    [SerializeField] private Color _fonPlayer;
    [Space]
    [SerializeField] private TypeRecord _normal;
    [Space]
    [SerializeField] private TypeRecord[] _ranks;

    public void Setup(LeaderboardRecord record, bool isPlayer = false)
    {
        SetText(_rankText, record.Rank.ToString());
        SetText(_scoreText, record.Score.ToString());
        if(!string.IsNullOrEmpty(record.Name))
            SetText(_nameText, record.Name);
        else
            SetText(_nameText, Localization.Inst.GetText(_keyAnonymName));

        SetAvatar(record.AvatarURL).Forget();

        if(isPlayer)
            SetFonColor(_fonPlayer);
        else
            SetFonColor(_fonNormal);

        if (record.Rank <= _ranks.Length)
            SetRecord(_ranks[record.Rank - 1]);
        else
            SetRecord(_normal);

        // ================ local func =====================
        void SetText(Text text, string str)
        {
            text.text = str;
            text.color = _colorText;
        }

        async UniTaskVoid SetAvatar(string url)
        {
            var (result, texture) = await Storage.TryLoadTextureWeb(url);
            if (result)
                _avatarRawImage.texture = texture;
            else
                _avatarRawImage.texture = _avatarAnonym;

        }

        void SetFonColor(Color color)
        {
            _fonImage.color = color;
            _maskAvatar.color = color;

        }

        void SetRecord(TypeRecord type)
        {
            Image thisImage = GetComponent<Image>();
            Vector2 size = thisImage.rectTransform.sizeDelta;

            thisImage.color = type.Color;
            thisImage.rectTransform.sizeDelta = new(size.x + type.OffsetSize, size.y + type.OffsetSize);
        }
    }

    [System.Serializable]
    private class TypeRecord
    {
        [SerializeField] private Color _color;
        [SerializeField] private int _offsetSize;

        public Color Color => _color;
        public int OffsetSize => _offsetSize;
    }
}
