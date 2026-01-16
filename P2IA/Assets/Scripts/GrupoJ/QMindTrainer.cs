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
        private bool IsWalkable(int x, int y, WorldInfo world)
        {
            if (x < 0 || x >= world.WorldSize.x || y < 0 || y >= world.WorldSize.y) return false;
            return world[x, y].Walkable;
        }
        private float ComputeReward(CellInfo agent, CellInfo other)
        {
            float reward = 0f;
            // 1. Penalización máxima si lo atrapan
            if (agent.x == other.x && agent.y == other.y)
                reward = -100f;

            // 2. Cálculo de distancias
            int distActual = Math.Abs(agent.x-other.x) + Math.Abs(agent.y - other.y);
            int distPrevia = Math.Abs(_agentPosition.x - _otherPosition.x) + Math.Abs(_agentPosition.y - _otherPosition.y);


            
            if (agent.x == _agentPosition.x && agent.y == _agentPosition.y)
            {
                // Penalización por quedarse quieto 
                reward = -1.0f; 
            }
            else if (distActual >= distPrevia) 
            {
                // Recompensa por alejarse 
                reward = 2.0f; 
            }
            else 
            {
                // Penalización leve si se movió hacia el zombie
                reward = -1.5f; 
            }

            
            
            int count = 0;
            for (int i = -1; i <= 1; i += 2)
            {
                bool walkable1 = !(agent.x+i < 0 && agent.x+i >= _worldInfo.WorldSize.x && agent.y < 0 || agent.y >= _worldInfo.WorldSize.y);   //Calcula si los dos vecinos por la izquierda y derecha son caminables
                bool walkable2 = !(agent.x < 0 && agent.x >= _worldInfo.WorldSize.x && agent.y+i < 0 || _agentPosition.y+i >= _worldInfo.WorldSize.y);  //Calcula si los dos vecinos por encima y debajo son caminables
                if (walkable1) { count++; }
                if (walkable2) { count++; }
               
            }
            // Penalización si dos o más de sus vecinos no son caminables
            if (count < 2) { reward = -0.5f; }
            
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

            switch (action)
            {
                case QAction.Up:    ny += 1; break;
                case QAction.Down:  ny -= 1; break;
                case QAction.Right: nx += 1; break;
                case QAction.Left:  nx -= 1; break;
                case QAction.Stay:  return agentCell;
            }

            // Comprobamos límites del mundo
            if (nx >= 0 && nx < _worldInfo.WorldSize.x && ny >= 0 && ny < _worldInfo.WorldSize.y)
            {
                CellInfo targetCell = _worldInfo[nx, ny];
                // Si es caminable se mueve  si no devuelve la posición actual.
                if (targetCell.Walkable) 
                    return targetCell;
            }

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