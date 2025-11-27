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
            //Hacemos nuestra openList con sortedlist para mantener ordenados por heurística (distancia línea recta)
            SortedList<float, CellInfo> openList = new SortedList<float, CellInfo>();

            // El coste desde el nodo inicial a cada nodo
            Dictionary<CellInfo, float> costs = new Dictionary<CellInfo, float>();

            //Mapa para reconstruir el camino una vez encontrado el target
            Dictionary<CellInfo, CellInfo> cameFrom = new Dictionary<CellInfo, CellInfo>();

            // Añadimos el nodo inicial con su heurística y su coste (al principio vale 0 porque no ha recorrido nada aún)
            float h = startNode.EuclideanDistance(targetNode);
            costs[startNode] = 0;
            openList.Add(h, startNode);

            while (openList.Count > 0)
            {
                // Sacar el de menor heurística
                float lowestF = openList.Keys[0];
                CellInfo current = openList[lowestF];
                openList.RemoveAt(0);

                ///Si encontramos el target, reconstruir el camino
                if (current == targetNode)
                    return ReconstructPath(cameFrom, current);

                // Expandir a los  vecinos
                foreach (CellInfo neighbor in GetNeighbours(current))
                {

                    if (!neighbor.Walkable)
                        continue;

                    // Coste acumulado al siguiente nodo
                    float nextNodeCost = costs[current] + 1f;

                    // Si es la primera vez o encontramos un mejor camino
                    if (!costs.ContainsKey(neighbor) || nextNodeCost < costs[neighbor])
                    {
                        costs[neighbor] = nextNodeCost; // Actualizar los coste
                        cameFrom[neighbor] = current;   //Guardar camino

                        //Calculamos f estrella : f* = g + h*
                        float f = nextNodeCost + neighbor.EuclideanDistance(targetNode);

                        // SortedList no deja meter claves repetidas, así que si existe la modificamos un poco
                        while (openList.ContainsKey(f))
                            f += 0.0001f;

                        openList.Add(f, neighbor);
                    }
                }
            }

            return null; // Delvolver null si no se encuentra camino
        }


        private CellInfo[] ReconstructPath(Dictionary<CellInfo, CellInfo> cameFrom, CellInfo current)
        {
            //Convertir el mapa de cameFrom en un array de celdas
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

