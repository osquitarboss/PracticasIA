using Components;
using NavigationDJIA.World;
using UnityEngine;

/// <summary>
/// TODO(alumno):
/// Define el "estado" que usará la Tabla Q para identificar cada situación del agente.
/// 
/// El estado debe contener toda la información necesaria para que el agente pueda
/// tomar decisiones informadas. Tú decides qué características incluir según lo
/// que consideres relevante para resolver el problema.
/// 
/// Ejemplos típicos de información que puede formar un estado:
///   - Posición del agente en la grid.
///   - Posición del otro personaje (enemigo).
///   - Distancia relativa entre agente y enemigo.
///   - Si hay muros en direcciones cercanas.
///   - Cualquier otro dato que consideres útil.
/// 
/// En este ejercicio te damos un ejemplo simple basado únicamente en las posiciones
/// del agente y del oponente. Puedes usarlo tal cual o ampliarlo.
/// 
/// IMPORTANTE: 
///  El estado debe poder convertirse a una clave única (string) mediante ToKey(),
///  ya que esa clave se usará como índice en la TablaQ y en el archivo CSV.
/// </summary>

namespace GrupoJ
{
    public sealed class QState
    {
        public int DistX { get; }
        public int DistY { get; }
        public bool WalkUp { get; }
        public bool WalkDown { get; }
        public bool WalkRight {  get; }
        public bool WalkLeft { get; }

        
        public QState(CellInfo agent, CellInfo other, WorldInfo world)
        {
            DistX = agent.x - other.x;   //cuanto por encima del zombie
            DistY = agent.y - other.y;   //cuanto a la derecha del zombie
            

            WalkUp= IsWalkable(agent.x,agent.y +1, world);
            WalkDown = IsWalkable(agent.x, agent.y - 1, world);
            WalkLeft = IsWalkable(agent.x - 1, agent.y, world);
            WalkRight= IsWalkable(agent.x+1, agent.y, world);
        }
        private bool IsWalkable(int x, int y, WorldInfo world)
        {
            if(x<0 || x>= world.WorldSize.x || y<0 || y>= world.WorldSize.y) return false;
            return world[x, y].Walkable;
        }

        public string ToKey()
        {
            // ejemplo 1,1 | 1111  >>>> 1 encima del zombie 1 a la derecha del zombie y todos los vecinos caminables
            return $"{DistX},{DistY}|{(WalkUp ? 1 : 0)}{(WalkDown ? 1 : 0)}{(WalkLeft ? 1 : 0)}{(WalkRight? 1 : 0)}";
        }
    }
}