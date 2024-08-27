using Ray.Notes.RE.Entity;

namespace Ray.Notes.RE.Repository;

public class ConditionRepository
{
    public ConditionEntity Get(int conditionId)
    {
        return new ConditionEntity
        {
            Type = ConditionType.ActivedVehicleCount,
            Code = "MoreThanZero",
            Description = "拥有激活车",
            ExpressionStr = "count > 0"
        };
    }
}
