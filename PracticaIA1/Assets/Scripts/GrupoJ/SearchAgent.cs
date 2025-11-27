using Navigation.Interfaces;
using Navigation.World;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrupoJ
{
    public class SearchAgent : Navigation.Interfaces.INavigationAgent
    {
        private WorldInfo _worldInfo;
        private INavigationAlgorithm _navigationAlgorithm;

        private List<CellInfo> _objectives;
        private Queue<CellInfo> _path;

        public CellInfo CurrentObjective { get; private set; }

        public Vector3 CurrentDestination { get; private set; }

        public int NumberOfDestinations { get; private set; }

        public void Initialize(WorldInfo worldInfo, Navigation.Interfaces.INavigationAlgorithm navigationAlgorithm)
        {
            _worldInfo = worldInfo;
            _navigationAlgorithm = navigationAlgorithm;
            Debug.Log("numero de destinos: " + GetDestinations().Count);
        }

        public Vector3? GetNextDestination(Vector3 currentPosition)
        {
            Debug.Log("obteniendo siguiente destino desde: " + currentPosition);
            // Primera iteracion, obtener objetivos y sacar el primero
            if (_objectives == null)
            {
                _objectives = GetDestinations();
                _objectives = SortByDistance(_objectives, _worldInfo.FromVector3(currentPosition));
                CurrentObjective = _objectives[0];
                _objectives.RemoveAt(0);
                NumberOfDestinations = _objectives.Count;
            }

            // Si no hay ruta, calcularla hacia el objetivo actual
            if (_path == null || _path.Count == 0)
            {
                CellInfo position = _worldInfo.FromVector3(currentPosition);
                CellInfo[] path = _navigationAlgorithm.GetPath(position, CurrentObjective);
                _path = new Queue<CellInfo>(path);

            }

            // Si hay ruta, obtener el siguiente nodo
            if (_path.Count > 0)
            {
                CellInfo destination = _path.Dequeue();
                Debug.Log("nodos: " + _path.Count);
                CurrentDestination = _worldInfo.ToWorldPosition(destination);
            }

            //Si se ha llegado al objetivo actual obtener el siguiente objetivo
            if (_path.Count == 0 && _objectives.Count > 0)
            {
                _objectives = SortByDistance(_objectives, _worldInfo.FromVector3(currentPosition));
                CurrentObjective = _objectives[0];
                _objectives.RemoveAt(0);
                NumberOfDestinations = _objectives.Count;
            }

            // Si no hay mas objetivos manfar al agente a la cell exit
            if (_objectives.Count == 0 && _path.Count == 0)
            {
                _objectives.Add(_worldInfo.Exit);
            }

            Debug.Log("ruta hacia el objetivo: " + CurrentObjective);
            Debug.Log("Numero de destinos: " + _objectives.Count);
            return CurrentDestination;
        }

        //Meter  las cell objetivo
        private List<CellInfo> GetDestinations()
        {
            List<CellInfo> targets = new List<CellInfo>();
            foreach (var cell in _worldInfo.Targets)
            {
                targets.Add(cell);
            }

            return targets;
        }

        // Ordenar las cell objetivo por distancia al agente para que al llegar a un objetivo se dirija al mas cercano
        List<CellInfo> SortByDistance(List<CellInfo> targets, CellInfo currentPos)
        {
            targets.Sort((a, b) =>
            {
                float distA = currentPos.EuclideanDistance(a);
                float distB = currentPos.EuclideanDistance(b);
                return distA.CompareTo(distB);
            });

            return targets;
        }
    }
}