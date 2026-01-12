using UnityEngine;
using Navigation.World;
using Navigation.Interfaces;

namespace GrupoJ{
    public class QMindTester : IQMind
    {
        public void Initialize(WorldInfo worldInfo){

        }

        public CellInfo GetNextStep(CellInfo currentCell, CellInfo previousCell){
            return null;
        }
    }
}

