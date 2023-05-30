using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    [SerializeField] private Material _themeMaterial;
    [SerializeField] private Light _lightSource;
    [SerializeField] private Theme[] _themes;

    // apply selected theme
    private void Start()
    {
        int index = DataManager.Instance.ActiveTheme;
        _themeMaterial.mainTexture = _themes[index].Texture;
        _lightSource.color = _themes[index].LightColor;
    }
}
