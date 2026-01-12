using UnityEngine;

namespace GrupoJ
{
    public class QTable
    {
        private QTableStorage _storage;
        private string[] _actionNames;

        public QTable(string[] actionNames)
        {
            _actionNames = actionNames;
            _storage = new QTableStorage(actionNames);
        }

        private void EnsureState(string stateKey){
            if (_storage.ContainsState(stateKey)) return;
            _storage.SetRow(stateKey, new float[_actionNames.Length]);
        }

        public float GetQ(string stateKey, QAction action){
            EnsureState(stateKey);
            return _storage.GetRow(stateKey)[(int)action];
        }

        public void SetQ(string stateKey, QAction action, float value){
            EnsureState(stateKey);
            _storage.GetRow(stateKey)[(int)action] = value;
        }

        public float GetMaxQ(string stateKey){
            EnsureState(stateKey);
            float max = float.MinValue;
            var row = _storage.GetRow(stateKey);

            foreach (float v in row)
                if (v > max) max = v;

            return max;
        }

        public QAction GetMaxQAction(string stateKey){
            EnsureState(stateKey);

            var row = _storage.GetRow(stateKey);
            float max = float.MinValue;
            int bestIndex = 0;

            for (int i = 0; i < row.Length; i++)
            {
                if (row[i] > max)
                {
                    max = row[i];
                    bestIndex = i;
                }
            }

            return (QAction)bestIndex;
        }

        public void SaveToCsv(){
            _storage.SaveToCsv();
        }
    }
}
