using System;

public struct QteResultData
{
    public QteResultData(QTEResultType qteResultType, Guid enemyId) 
    {
        EnemyId = enemyId;
        ResultType = qteResultType;
    }

    public QTEResultType ResultType;
    public Guid EnemyId;
}
