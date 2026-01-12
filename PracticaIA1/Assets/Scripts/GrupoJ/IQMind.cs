using UnityEngine;
using Navigation.World;
using Navigation.Interfaces;

namespace GrupoJ
{
    public interface IQMind
    {
        void Initialize(WorldInfo worldInfo);

        CellInfo GetNextStep(CellInfo currentCell, CellInfo previousCell);
    }
}

