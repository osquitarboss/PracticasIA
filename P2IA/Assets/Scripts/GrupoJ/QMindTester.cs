using NavigationDJIA.World;
using QMind;
using QMind.Interfaces;

namespace GrupoJ
{
    public class QMindTester : IQMind
    {
        private WorldInfo _worldInfo;
        private QTableStorage _qStorage;
        private QTable _qTable;

        public void Initialize(WorldInfo worldInfo)
        {
            _worldInfo = worldInfo;

            _qStorage = new QTableStorage("TablaQ.csv");
            _qTable = new QTable(_qStorage);
        }

        public CellInfo GetNextStep(CellInfo currentPosition, CellInfo otherPosition)
        {
            string stateKey = BuildStateKey(currentPosition, otherPosition);

            QAction bestAction = _qTable.GetBestAction(stateKey);

            CellInfo nextPosition = ApplyAction(currentPosition, bestAction);

            return nextPosition;
        }

        private string BuildStateKey(CellInfo agent, CellInfo other)
    {
        
        var state = new QState(agent, other,_worldInfo);
        return state.ToKey();
    }

        private CellInfo ApplyAction(CellInfo agentCell, QAction action)
        {
            int nx = agentCell.x;
            int ny = agentCell.y;
            switch (action)
            {
                case QAction.Up: ny += 1; break;
                case QAction.Down: ny -= 1; break;
                case QAction.Right: nx += 1; break;
                case QAction.Left: nx -= 1; break;
                case QAction.Stay: return agentCell;
            }

            if (nx >= 0 && nx < _worldInfo.WorldSize.x && ny >= 0 && ny < _worldInfo.WorldSize.y)
            {
                CellInfo targetCell = _worldInfo[nx, ny];
                // Si es caminable se mueve si no devuelve la posición actual.
                if (targetCell.Walkable)
                    return targetCell;
            }

            return agentCell;
        }
    }
}