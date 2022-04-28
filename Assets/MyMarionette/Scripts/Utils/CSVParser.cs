using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVParser : MonoBehaviour
{
    [SerializeField] TextAsset sequenceFile;
    private static char ROW_DELIM = '\n';
    private static char ENTRY_DELIM = '|';

    private Dictionary<string, NarrationAudioData> narrationMap;

    private void Start() {
        // load the narration map
        narrationMap = NarrationManager.Instance.GetNarrationMap();

        // parse the csv file
        string[] dataset = sequenceFile.text.Split(ROW_DELIM);

        foreach(string row in dataset) {
            string[] entries = row.Split(ENTRY_DELIM);

            if (entries == null) { continue; }

            if (entries[0].Length <= 1) { continue; }

            // trim newline
            string sequenceID = entries[0];

            // trim first and last quotation mark
            string subtitle = entries[1];
            if (subtitle[0] == '"') {
                subtitle = subtitle.Substring(1, subtitle.Length - 1);
            }
            if (subtitle[subtitle.Length - 1] == '"') {
                subtitle = subtitle.Substring(0, subtitle.Length - 1);
            }

            // trim intermediate quotation marks
            int trimIndex = subtitle.IndexOf("\"\"");
            while (trimIndex != -1) {
                subtitle = subtitle.Substring(0, trimIndex) + subtitle.Substring(trimIndex + 1);
                trimIndex = subtitle.IndexOf("\"\"");
            }

            // Set the new subtitle
            if (narrationMap.ContainsKey(sequenceID)) {
                narrationMap[sequenceID].Subtitle = subtitle;
            }
        }

        // save the new map
        NarrationManager.Instance.SetNarrationMap(narrationMap);
    }
}
