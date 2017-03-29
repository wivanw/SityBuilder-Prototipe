using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class CountUpdater : MonoBehaviour
{
    private Text _text;
    public int Factor;
    public int Min;
    private Text Text
    {
        get { return _text ?? (_text = GetComponent<Text>()); }
    }

    public float Count
    {
        set { Text.text = (Factor * value + Min).ToString(CultureInfo.InvariantCulture); }
    }

}
