using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Version : MonoBehaviour
{
    void Start() => GetComponent<Text>().text = Application.version;
}
