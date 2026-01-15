using NavigationDJIA.World;

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
        public int AgentX { get; }
        public int AgentY { get; }
        public int OtherX { get; }
        public int OtherY { get; }
        
        private bool _wallUp, _wallDown, _wallLeft, _wallRight;

        public QState(CellInfo agent, CellInfo other, NavigationDJIA.World.WorldInfo world)
        {
            AgentX = agent.x;
            AgentY = agent.y;
            OtherX = other.x;
            OtherY = other.y;

            // Detectamos muros o bordes en las 4 direcciones
            _wallUp = IsWall(agent.x, agent.y + 1, world);
            _wallDown = IsWall(agent.x, agent.y - 1, world);
            _wallLeft = IsWall(agent.x - 1, agent.y, world);
            _wallRight = IsWall(agent.x + 1, agent.y, world);
        }

        private bool IsWall(int x, int y, NavigationDJIA.World.WorldInfo world)
        {
            // Si la coordenada se sale de los índices, la IA lo percibe como un muro
            if (x < 0 || x >= world.WorldSize.x || y < 0 || y >= world.WorldSize.y)
                return true;

            // Si la celda existe pero no es caminable
            return !world[x, y].Walkable;
        }

        public string ToKey()
        {
            // El estado ahora incluye la configuración de muros alrededor
            string walls = $"{(_wallUp?1:0)}{(_wallDown?1:0)}{(_wallLeft?1:0)}{(_wallRight?1:0)}";
            return $"{AgentX},{AgentY}|{OtherX},{OtherY}|{walls}";
        }
    }
}