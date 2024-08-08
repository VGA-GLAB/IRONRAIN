using System.Collections.Generic;
using Enemy.Boss.FSM;

namespace Enemy.Boss
{
    public class StateRequiredRef
    {
        public StateRequiredRef(Dictionary<StateKey, State> states, BossParams bossParams, BlackBoard blackBoard,
            Body body, BodyAnimation bodyAnimation, Effector effector, IReadOnlyList<FunnelController> funnels,
            AgentScript agentScript)
        {
            States = states;
            BossParams = bossParams;
            BlackBoard = blackBoard;
            Body = body;
            BodyAnimation = bodyAnimation;
            Effector = effector;
            Funnels = funnels;
            AgentScript = agentScript;
        }

        public Dictionary<StateKey, State> States { get; private set; }
        public BossParams BossParams { get; private set; }
        public BlackBoard BlackBoard { get; private set; }
        public Body Body { get; private set; }
        public BodyAnimation BodyAnimation { get; private set; }
        public Effector Effector { get; private set; }
        public IReadOnlyList<FunnelController> Funnels { get; private set; }
        public AgentScript AgentScript { get; private set; }
    }
}
