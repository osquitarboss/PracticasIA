using UnityEngine;
using Navigation.World;
using Navigation.Interfaces;
using System;

namespace GrupoJ{
    public class QMindTrainer : IQMindTrainer
    {
        public int CurrentEpisode { get; set; }
        public int CurrentStep { get; set; }
        public CellInfo AgentPosition { get; set; }
        public CellInfo OtherPosition { get; set; }
        public float Return { get; set; }
        public float ReturnAveraged { get; set; }

        public event EventHandler OnEpisodeStarted;
        public event EventHandler OnEpisodeFinished;

        public void Initialize(QMindTrainerParams trainerParams, WorldInfo worldInfo, INavigationAlgorithm navigationAlgorithm){
            
        }

        public void DoStep(bool agentMove){

        }
    }
}

