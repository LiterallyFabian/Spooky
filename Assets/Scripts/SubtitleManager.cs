using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class SubtitleManager : MonoBehaviour
{
    [SerializeField] private TextAsset subtitlesTextFile;
    [SerializeField] private string voiceLinesFolderPath;
    [SerializeField] private Subtitle[] subtitles;
    [SerializeField] private int currentSubtitleIndex;

    private void OnValidate()
    {
        subtitles = subtitlesTextFile.text.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).Select((voiceLine, index) => 
        {
            return new Subtitle
            {
                line = index,
                text = voiceLine,
                duration = GetVoiceLineDuration(index)
            };
        }).ToArray();
    }

    public float GetVoiceLineDuration(int line)
    {
        string path = $"{voiceLinesFolderPath}/line {line}";
        var clip = Resources.Load<AudioClip>(path);
        if(clip == null)
        {
            return 0;
        }
        return clip.length;
    }

    public void StartSubtitle(int line)
    {
        GetComponent<TextMeshProUGUI>().text = subtitles[line].text;
        currentSubtitleIndex = line;
        StartCoroutine(Waiter());

    }

    private void StopSubtitle()
    {
        GetComponent<TextMeshProUGUI>().text = "";
    }

    private IEnumerator Waiter()
    {
          yield return new WaitForSeconds(subtitles[currentSubtitleIndex].duration);
          StopSubtitle();

    }

}
