using UnityEngine;
using System;
using Navigation.World;
using Navigation.Interfaces;

namespace GrupoJ
{
    public interface IQMindTrainer
    {
        int CurrentEpisode { get; set; }
        int CurrentStep { get; set; }
        CellInfo AgentPosition { get; set; }
        CellInfo OtherPosition { get; set; }
        float Return { get; set; }
        float ReturnAveraged { get; set; }

        event EventHandler OnEpisodeStarted;
        event EventHandler OnEpisodeFinished;

        void Initialize(QMindTrainerParams trainerParams, WorldInfo worldInfo, INavigationAlgorithm navigationAlgorithm);

        void DoStep(bool agentMove);
    }
}
