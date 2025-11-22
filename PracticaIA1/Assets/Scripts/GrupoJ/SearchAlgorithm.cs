using System;
using System.Collections.Generic;
using Navigation.Interfaces;
using Navigation.World;

namespace GrupoJ
{
    public class SearchAlgorithm : Navigation.Interfaces.INavigationAlgorithm
    {
        private WorldInfo _world;
        public enum Directions
        {
            None,
            Up,
            Right,
            Down,
            Left
        }

        public void Initialize(WorldInfo worldInfo)
        {
            _world = worldInfo;
        }

        // Para esta heurística, se puede usar la distancia euclidiana entre dos celdas



        /// MAlLLLL!!!! AQUÍ HAY Q CAMBIAR LA LÓGICA PARA QUE NO SOLO COJA LOS VECINOS, SINO QUE BUSQUE UN CAMINO COMPLETO HASTA EL TARGETNODE

        public CellInfo[] GetPath(CellInfo startNode, CellInfo targetNode)
        {
            Dictionary<float, CellInfo> hStarDictionary = new Dictionary<float, CellInfo>();
            List<CellInfo> neighbours = GetNeighbours(startNode);

            foreach (CellInfo cell in neighbours)
            {
                if (cell.Walkable)
                {
                    float hStar = cell.EuclideanDistance(targetNode);
                    hStarDictionary[hStar] = cell;
                }
            }

            return DictionaryToArray(hStarDictionary);
        }

        // Sacar los vecinos de una celda
        public List<CellInfo> GetNeighbours(CellInfo current)
        {
            List<CellInfo> neighbours = new List<CellInfo>();

            neighbours.Add(_world[current.x, current.y - 1]);
            neighbours.Add(_world[current.x + 1, current.y]);
            neighbours.Add(_world[current.x, current.y + 1]);
            neighbours.Add(_world[current.x - 1, current.y]);

            return neighbours;
        }
        // Transformar el diccionario en un array ordenado por clave
        CellInfo[] DictionaryToArray(Dictionary<float, CellInfo> hStarDictionary)
        {
            var sortedKeys = new List<float>(hStarDictionary.Keys);
            sortedKeys.Sort();

            CellInfo[] path = new CellInfo[hStarDictionary.Count];

            for (int i = 0; i < sortedKeys.Count; i++)
            {
                path[i] = hStarDictionary[sortedKeys[i]];
            }

            return path;
        }
    }
}