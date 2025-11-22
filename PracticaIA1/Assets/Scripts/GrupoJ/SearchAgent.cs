using Navigation.Interfaces;
using Navigation.World;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrupoJ
{
    public class SearchAgent : Navigation.Interfaces.INavigationAgent
    {
        private WorldInfo _worldInfo;
        private INavigationAlgorithm _navigationAlgorithm;

        private CellInfo[] _objectives;
        private Queue<CellInfo> _path;

        public CellInfo CurrentObjective { get; private set; }

        public Vector3 CurrentDestination { get; private set; }

        public int NumberOfDestinations { get; private set; }

        public void Initialize(WorldInfo worldInfo, Navigation.Interfaces.INavigationAlgorithm navigationAlgorithm)
        {
            _worldInfo = worldInfo;
            _navigationAlgorithm = navigationAlgorithm;
        }

        public Vector3? GetNextDestination(Vector3 currentPosition)
        {
            if (_objectives == null)
            {
                _objectives = GetDestinations();
                CurrentObjective = _objectives[_objectives.Length - 1];
                NumberOfDestinations = _objectives.Length;
            }

            if (_path == null || _path.Count == 0)
            {
                CellInfo position = _worldInfo.FromVector3(currentPosition);
                CellInfo[] path = _navigationAlgorithm.GetPath(position, CurrentObjective);
                _path = new Queue<CellInfo>(path);
            }

            if (_path.Count > 0)
            {
                CellInfo destination = _path.Dequeue();
                CurrentDestination = _worldInfo.ToWorldPosition(destination);
            }

            return CurrentDestination;
        }

        private CellInfo[] GetDestinations()
        {
            List<CellInfo> targets = new List<CellInfo>();
            targets.Add(_worldInfo.Exit);
            return targets.ToArray();
        }
    }
}