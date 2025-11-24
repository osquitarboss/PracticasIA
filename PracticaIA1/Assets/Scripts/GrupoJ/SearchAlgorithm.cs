using System;
using System.Collections.Generic;
using Navigation.Interfaces;
using Navigation.World;

namespace GrupoJ
{
    public class SearchAlgorithm : INavigationAlgorithm
    {
        private WorldInfo _world;

        public void Initialize(WorldInfo worldInfo)
        {
            _world = worldInfo;
        }

        public CellInfo[] GetPath(CellInfo startNode, CellInfo targetNode)
        {
            //Hacemos nuestra openList con sortedlist para mantener ordenados por heurística
            SortedList<float, CellInfo> openSet = new SortedList<float, CellInfo>();
            // Conjunto con los nodos que hemos visitado para no tener ciclos simples
            HashSet<CellInfo> visited = new HashSet<CellInfo>();
            //Mapa para reconstruir el camino una vez encontrado el target
            Dictionary<CellInfo, CellInfo> cameFrom = new Dictionary<CellInfo, CellInfo>();

            // Añadimos el nodo inicial con su heurística
            float h = startNode.EuclideanDistance(targetNode);
            openSet.Add(h, startNode);
            visited.Add(startNode);

            while (openSet.Count > 0)
            {
                // Sacar el de menor heurística (EuclideanDistance de la clase CellInfo)
                float lowestH = openSet.Keys[0];
                CellInfo current = openSet[lowestH];
                openSet.RemoveAt(0);

                /// Si encontramos el target, reconstruir el camino
                if (current == targetNode)
                    return ReconstructPath(cameFrom, current);


                // Expandir vecinos
                foreach (CellInfo neighbor in GetNeighbours(current))
                {
                    if (!neighbor.Walkable || visited.Contains(neighbor)) // Comprobar si se puede caminar o si yalo habiamos visitado
                        continue;

                    visited.Add(neighbor); // En caso contrario, marcar como visitado y añadir al mapa de cameFrom
                    cameFrom[neighbor] = current;

                    //Calculamos las heurísticas y añadimos a la openSet
                    float hn = neighbor.EuclideanDistance(targetNode);

                    if (!openSet.ContainsKey(hn)) // No se pueden añadir claves duplicadas en SortedList
                        openSet.Add(hn, neighbor);
                }
            }
            return null;
        }

        private CellInfo[] ReconstructPath(Dictionary<CellInfo, CellInfo> cameFrom, CellInfo current)
        {
            List<CellInfo> path = new List<CellInfo>();
            path.Add(current);

            while (cameFrom.ContainsKey(current)) // Para reconstruir el camino desde el target al start, vamos retrocediendo en el mapa de cameFrom y añadiendo al path
            {
                current = cameFrom[current];
                path.Add(current);
            }

            path.Reverse();
            return path.ToArray();
        }


        //Sacar los vecinos de una celda
        public List<CellInfo> GetNeighbours(CellInfo current)
        {
            List<CellInfo> neighbours = new List<CellInfo>();

            neighbours.Add(_world[current.x, current.y - 1]);
            neighbours.Add(_world[current.x + 1, current.y]);
            neighbours.Add(_world[current.x, current.y + 1]);
            neighbours.Add(_world[current.x - 1, current.y]);

            return neighbours;
        }
    }
}

