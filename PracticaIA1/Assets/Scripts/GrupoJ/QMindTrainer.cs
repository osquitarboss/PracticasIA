using UnityEngine;
using Navigation.World;
using Navigation.Interfaces;
using System;

namespace GrupoJ{

    
    public class QMindTrainer : MonoBehaviour, IQMindTrainer
    {
        public int CurrentEpisode { get; set; }
        public int CurrentStep { get; set; }
        public CellInfo AgentPosition { get; set; }
        public CellInfo OtherPosition { get; set; }
        public float Return { get; set; }
        public float ReturnAveraged { get; set; }

        public event EventHandler OnEpisodeStarted;
        public event EventHandler OnEpisodeFinished;

        ///////////////////////////////////////////////////
        
        private WorldInfo world;
        private INavigationAlgorithm navAlgorithm;
        private INavigationAgent agent;
        private QTable qTable;
        private QMindTrainerParams trainerParams;

        private float alpha;
        private float gamma;
        private float epsilon;

        private QAction currentAction;
        private string currentStateKey; 

        
        public void Initialize(QMindTrainerParams trainerParams, WorldInfo worldInfo, INavigationAlgorithm navigationAlgorithm){
            world = worldInfo;
            navAlgorithm = navigationAlgorithm;
            qTable = new QTable(Enum.GetNames(typeof(QAction)));

            alpha = trainerParams.alpha;
            gamma = trainerParams.gamma;
            epsilon = trainerParams.epsilon;

            AgentPosition = world[0,0];
            OtherPosition = world.Enemies[0];
            currentStateKey = StateKey(AgentPosition, OtherPosition);

            CurrentEpisode = 0;
            CurrentStep = 0;
            Return = 0;

            OnEpisodeStarted?.Invoke(this, EventArgs.Empty);


        }

        public void DoStep(bool agentMove){
            if(agentMove){
                OtherPosition = world.Enemies[0];
                string state = StateKey(AgentPosition, OtherPosition);

                QAction action = ChooseAction(state);

                CellInfo targetCell = GetTargetCell(action);
                var path = navAlgorithm.GetPath(AgentPosition, targetCell);
                if (!targetCell.Walkable)
                {
                    ApplyQUpdate(state, action, -1f, state);
                    Return -= 1f;
                    return;
                }

                AgentPosition = path[0];
                string nextStateKey = StateKey(AgentPosition, OtherPosition);
                
                float reward = GetReward(AgentPosition);
                ApplyQUpdate(currentStateKey, action, reward, nextStateKey);

                currentStateKey = nextStateKey;
                CurrentStep++;
                Return += reward;

                if(AgentPosition == OtherPosition){
                    FinishEpisode();
                }
            }
        }

        private void ApplyQUpdate(string state, QAction action, float reward, string nextState)
        {
            float oldQ = qTable.GetQ(state, action);
            float maxNext = qTable.GetMaxQ(nextState);

            float newQ = oldQ + alpha * (reward + gamma * maxNext - oldQ); //Ecuacion del algoritmo Q-Learning
            qTable.SetQ(state, action, newQ);
        }

        private QAction ChooseAction(string state){
            if (UnityEngine.Random.value < epsilon)
            {
                int r = UnityEngine.Random.Range(0, Enum.GetValues(typeof(QAction)).Length);
                return (QAction)r;
            }
            return (QAction) Enum.GetValues(typeof(QAction)).GetValue(1); //Coge el primer valor en caso de que el aleatorio sea mayor que epsilon
        }

        CellInfo GetTargetCell(QAction action)
        {
            int x = AgentPosition.x;
            int y = AgentPosition.y;

            switch (action)
            {
                case QAction.North:    y += 1; break;
                case QAction.South:  y -= 1; break;
                case QAction.West:  x -= 1; break;
                case QAction.East: x += 1; break;
            }

            return world[x, y];
        }

        private string StateKey(CellInfo agent, CellInfo enemy){
            return $"{agent.x},{agent.y}|{enemy.x},{enemy.y}";
        }

        private float GetReward(CellInfo cell){
            // poner un switch o algo para las recompensas
            return 0;
        }

        private void FinishEpisode()
        {
            OnEpisodeFinished?.Invoke(this, EventArgs.Empty);

            CurrentEpisode++;
            CurrentStep = 0;
            Return = 0;

            AgentPosition = world[0, 0];
            currentStateKey = StateKey(AgentPosition, OtherPosition);

            OnEpisodeStarted?.Invoke(this, EventArgs.Empty);
        }
    }
}

