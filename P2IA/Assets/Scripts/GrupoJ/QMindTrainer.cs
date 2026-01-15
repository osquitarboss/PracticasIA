using System;
using NavigationDJIA.Interfaces;
using NavigationDJIA.World;
using QMind;
using QMind.Interfaces;
using UnityEngine;

namespace GrupoJ
{
    public class QMindTrainer : IQMindTrainer
    {
        private QMindTrainerParams _params;
        private WorldInfo _worldInfo;
        INavigationAlgorithm _navigationAlgorithm;

        private QTableStorage _qStorage;
        private QTable _qTable;

        private CellInfo _agentPosition;
        private CellInfo _otherPosition;

        private float _return;
        private float _returnAveraged;
        private System.Random _random = new System.Random();

        #region IQMindTrainer implementation

        public CellInfo AgentPosition => _agentPosition;
        public CellInfo OtherPosition => _otherPosition;

        public int CurrentEpisode { get; private set; }
        public int CurrentStep { get; private set; }

        public float Return => _return;
        public float ReturnAveraged => _returnAveraged;

        public event EventHandler OnEpisodeStarted;
        public event EventHandler OnEpisodeFinished;

        #endregion

        public void Initialize(QMindTrainerParams qMindTrainerParams, WorldInfo worldInfo, INavigationAlgorithm navigationAlgorithm)
        {
            _params = qMindTrainerParams;
            _worldInfo = worldInfo;
            _navigationAlgorithm = navigationAlgorithm;
            _navigationAlgorithm.Initialize(worldInfo);

            _qStorage = new QTableStorage("TablaQ.csv");
            _qTable = new QTable(_qStorage);

            CurrentEpisode = 0;
            StartNewEpisode();
        }

        private void StartNewEpisode()
        {
            CurrentEpisode++;
            CurrentStep = 0;
            _return = 0f;

            _agentPosition = _worldInfo.RandomCell();
            _otherPosition = _worldInfo.RandomCell();

            OnEpisodeStarted?.Invoke(this, EventArgs.Empty);
        }

        private void EndEpisode()
        {
            _qTable.SaveToCsv();
            OnEpisodeFinished?.Invoke(this, EventArgs.Empty);

            if (_params.episodes > 0 && CurrentEpisode >= _params.episodes)
            {
                return;
            }

            StartNewEpisode();
        }

        public void DoStep(bool train)
        {
            Debug.Log($"Paso ejecutado. Agente en: {_agentPosition.x}, {_agentPosition.y}");
            string stateKey = BuildStateKey(_agentPosition, _otherPosition);
            QAction action = ChooseAction(stateKey, train);

            CellInfo newAgentPos = ApplyAction(_agentPosition, action);
            CellInfo newOtherPos = MoveOpponent(_otherPosition, newAgentPos);
            
            string nextStateKey = BuildStateKey(newAgentPos, newOtherPos);
            float reward = ComputeReward(newAgentPos, newOtherPos);

            if (train)
            {
                UpdateQ(stateKey, action, reward, nextStateKey);
            }

            _agentPosition = newAgentPos;
            _otherPosition = newOtherPos;

            CurrentStep++;
            _return += reward;
            _returnAveraged = (_returnAveraged * (CurrentStep - 1) + reward) / CurrentStep;

            if (IsTerminalState(_agentPosition, _otherPosition))
            {
                EndEpisode();
            }
        }

        #region Implementación Q-Learning

        private string BuildStateKey(CellInfo agent, CellInfo other)
    {
        return new QState(agent, other, _worldInfo).ToKey();
    }

        private QAction ChooseAction(string stateKey, bool train)
        {
            if (train && _random.NextDouble() < _params.epsilon)
            {
                Array values = Enum.GetValues(typeof(QAction));
                return (QAction)values.GetValue(_random.Next(values.Length));
            }
            return _qTable.GetBestAction(stateKey);
        }

        private void UpdateQ(string stateKey, QAction action, float reward, string nextStateKey)
        {
            float oldQ = _qTable.GetQ(stateKey, action);
            float maxQNext = _qTable.GetMaxQ(nextStateKey);

            float target = reward + _params.gamma * maxQNext;
            float newQ = (1 - _params.alpha) * oldQ + _params.alpha * target;

            _qTable.SetQ(stateKey, action, newQ);
        }

        private float ComputeReward(CellInfo agent, CellInfo other)
        {
            // 1. Penalización máxima si es alcanzado
            if (agent.x == other.x && agent.y == other.y)
                return -100f;

            // 2. Cálculo de mejora de distancia (como querías antes)
            int distActual = Math.Abs(agent.x - other.x) + Math.Abs(agent.y - other.y);
            int distPrevia = Math.Abs(_agentPosition.x - other.x) + Math.Abs(_agentPosition.y - other.y);

            float reward = 0f;

            if (distActual > distPrevia) reward = 5.0f; // Se aleja
            else if (distActual < distPrevia) reward = -2.0f; // Se acerca
            else reward = -0.5f; // Se queda igual (posible choque)

            // 3. PENALIZACIÓN POR BORDES (Tratarlos como muros)
            // Verificamos si la posición actual es el límite del mapa
            bool esBordeX = (agent.x == 0 || agent.x == _worldInfo.WorldSize.x - 1);
            bool esBordeY = (agent.y == 0 || agent.y == _worldInfo.WorldSize.y - 1);

            if (esBordeX || esBordeY)
            {
                reward -= 2.0f; // Penalización extra por estar pegado al borde
            }

            // 4. Penalización por esquinas (donde se juntan dos bordes)
            if (esBordeX && esBordeY)
            {
                reward -= 5.0f; // Las esquinas son trampas mortales
            }

            return reward;
        }

        private bool IsTerminalState(CellInfo agent, CellInfo other)
        {
            return agent == other;
        }

        private CellInfo ApplyAction(CellInfo agentCell, QAction action)
            {
                int nx = agentCell.x;
                int ny = agentCell.y;

                // 1. Calcular coordenadas teóricas
                switch (action)
                {
                    case QAction.Up:    ny += 1; break;
                    case QAction.Down:  ny -= 1; break;
                    case QAction.Right: nx += 1; break;
                    case QAction.Left:  nx -= 1; break;
                    case QAction.Stay:  return agentCell;
                }

                // 2. Validar límites usando WorldSize (x es width, y es height)
                if (nx >= 0 && nx < _worldInfo.WorldSize.x && ny >= 0 && ny < _worldInfo.WorldSize.y)
                {
                    CellInfo targetCell = _worldInfo[nx, ny];

                    // 3. Validar si la celda es caminable
                    if (targetCell.Walkable)
                    {
                        return targetCell;
                    }
                }

                // 4. Si es muro o fuera del mapa, se queda en la celda actual
                return agentCell;
            }

        private CellInfo MoveOpponent(CellInfo opponent, CellInfo target)
        {
            var path = _navigationAlgorithm.GetPath(opponent, target, 1);
            if (path != null && path.Length > 0)
                return path[0];

            return opponent;
        }
        #endregion
    }
}