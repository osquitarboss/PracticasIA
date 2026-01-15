using System;
using QMind;

namespace GrupoJ
{
    public class QTable
    {
        private readonly QTableStorage _storage;
        private readonly string[] _actionNames;

        public QTable(QTableStorage storage)
        {
            _storage = storage;
            _actionNames = Enum.GetNames(typeof(QAction));
        }

        private void EnsureState(string stateKey)
        {
            if (!_storage.Data.ContainsKey(stateKey))
            {
                _storage.Data[stateKey] = new float[_actionNames.Length];
            }
        }

        public float GetQ(string stateKey, QAction action)
        {
            EnsureState(stateKey);
            int index = (int)action;
            return _storage.Data[stateKey][index];
        }

        public void SetQ(string stateKey, QAction action, float value)
        {
            EnsureState(stateKey);
            int index = (int)action;
            _storage.Data[stateKey][index] = value;
        }

        public float GetMaxQ(string stateKey)
        {
            EnsureState(stateKey);
            var qValues = _storage.Data[stateKey];

            float max = qValues[0];
            for (int i = 1; i < qValues.Length; i++)
            {
                if (qValues[i] > max)
                    max = qValues[i];
            }

            return max;
        }

        public QAction GetBestAction(string stateKey)
        {
            EnsureState(stateKey);
            var qValues = _storage.Data[stateKey];

            int bestIndex = 0;
            float bestValue = qValues[0];

            for (int i = 1; i < qValues.Length; i++)
            {
                if (qValues[i] > bestValue)
                {
                    bestValue = qValues[i];
                    bestIndex = i;
                }
            }

            return (QAction)bestIndex;
        }

        public void SaveToCsv()
        {
            _storage.Save();
        }
    }
}
