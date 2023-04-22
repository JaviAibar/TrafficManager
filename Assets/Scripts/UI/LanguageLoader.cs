using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageLoader : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;
        int selected = PlayerPrefs.GetInt("Language", -1);
        if (selected > -1)
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[selected];
    }
}
