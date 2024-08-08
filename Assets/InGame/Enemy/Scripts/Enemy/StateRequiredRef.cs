using System.Collections.Generic;
using Enemy.FSM;

namespace Enemy
{
    public class StateRequiredRef
    {
        public StateRequiredRef(Dictionary<StateKey, State> states, EnemyParams enemyParams, BlackBoard blackBoard,
            Body body, BodyAnimation bodyAnimation, Effector effector, AgentScript agentScript)
        {
            States = states;
            EnemyParams = enemyParams;
            BlackBoard = blackBoard;
            Body = body;
            BodyAnimation = bodyAnimation;
            Effector = effector;
            AgentScript = agentScript;
        }

        public Dictionary<StateKey, State> States { get; private set; }
        public EnemyParams EnemyParams { get; private set; }
        public BlackBoard BlackBoard { get; private set; }
        public Body Body { get; private set; }
        public BodyAnimation BodyAnimation { get; private set; }
        public Effector Effector { get; private set; }
        public AgentScript AgentScript { get; private set; }
    }
}
