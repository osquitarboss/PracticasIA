using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace GrupoJ{
    public class QTableStorage
    {
        private Dictionary<string, float[]> _table;
        private string[] _actionNames;
        private string _filePath;

        public QTableStorage(string[] actionNames, string fileName = "qTable.csv")
        {
            _actionNames = actionNames;
            _table = new Dictionary<string, float[]>();

            _filePath = Path.Combine(Application.persistentDataPath, fileName);

            LoadFromCsv();
        } 

        public bool ContainsState(string stateKey)
        {
            return _table.ContainsKey(stateKey);
        }

        public float[] GetRow(string stateKey)
        {
            return _table[stateKey];
        }

        public void SetRow(string stateKey, float[] values)
        {
            _table[stateKey] = values;
        }


        private void LoadFromCsv()
        {
            if (!File.Exists(_filePath))
            {
                Debug.Log("QTable CSV not found, starting empty table.");
                return;
            }

            var lines = File.ReadAllLines(_filePath);

            foreach (var line in lines)
            {
                var parts = line.Split(',');

                string stateKey = parts[0];
                float[] values = new float[_actionNames.Length];

                for (int i = 0; i < _actionNames.Length; i++)
                    values[i] = float.Parse(parts[i + 1]);

                _table[stateKey] = values;
            }

            Debug.Log("QTable loaded from CSV");
        }

        public void SaveToCsv()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var pair in _table)
            {
                sb.Append(pair.Key);
                for (int i = 0; i < pair.Value.Length; i++)
                {
                    sb.Append(",");
                    sb.Append(pair.Value[i].ToString("F4"));
                }
                sb.AppendLine();
            }

            File.WriteAllText(_filePath, sb.ToString());

            Debug.Log("QTable saved to CSV: " + _filePath);
        } 
    }
}
