using Job.Domain.Enums;

namespace Job.Domain.Entities.Rental;

public sealed class RentalEntity : BaseEntity
{
    private RentalEntity()
    {
    }

    public RentalEntity(
        string identifier,
        Guid idMotoboy,
        Guid idMoto,
        DateOnly dateStart,
        DateOnly dateEnd,
        DateOnly datePreview,
        EPlan plan)
    {
        Identifier = identifier;
        IdMotoboy = idMotoboy;
        IdMoto = idMoto;
        DateStart = dateStart;
        DateEnd = dateEnd;
        DatePreview = datePreview;
        Plan = plan;
        DailyValue = ResolveDailyValue(plan);
        Value = DailyValue * (int)plan;
    }

    public string Identifier { get; private set; } = string.Empty;
    public DateOnly DateStart { get; private set; }
    public DateOnly DateEnd { get; private set; }
    public DateOnly DatePreview { get; private set; }
    public DateOnly? DateReturn { get; private set; }
    public Guid IdMoto { get; private set; }
    public Guid IdMotoboy { get; private set; }
    public EPlan Plan { get; private set; }
    public decimal Value { get; private set; }
    public decimal DailyValue { get; private set; }
    public decimal? Fine { get; private set; }

    public decimal RegisterReturn(DateOnly dateReturn)
    {
        Update();
        DateReturn = dateReturn;
        Fine = CalculateFine(dateReturn);
        Value = ComputeFinalValue(dateReturn);
        return Value;
    }

    private decimal ComputeFinalValue(DateOnly dateReturn)
    {
        if (dateReturn < DateEnd)
        {
            var daysUsed = dateReturn.DayNumber - DateStart.DayNumber;
            if (daysUsed < 0) daysUsed = 0;
            return DailyValue * daysUsed + (Fine ?? 0);
        }

        if (dateReturn > DateEnd)
        {
            return DailyValue * (int)Plan + (Fine ?? 0);
        }

        return DailyValue * (int)Plan;
    }

    private decimal CalculateFine(DateOnly dateReturn)
    {
        if (dateReturn < DateEnd)
        {
            var unusedDays = DateEnd.DayNumber - dateReturn.DayNumber;
            var unusedAmount = DailyValue * unusedDays;
            var penaltyRate = Plan == EPlan.Sete ? 0.20m : 0.40m;
            return Math.Round(unusedAmount * penaltyRate, 2);
        }

        if (dateReturn > DateEnd)
        {
            var lateDays = dateReturn.DayNumber - DateEnd.DayNumber;
            return 50m * lateDays;
        }

        return 0m;
    }

    private static decimal ResolveDailyValue(EPlan plan) => plan switch
    {
        EPlan.Sete => 30m,
        EPlan.Quinze => 28m,
        EPlan.Trinta => 22m,
        EPlan.QuarentaCinco => 20m,
        EPlan.Cinquenta => 18m,
        _ => 0m,
    };
}
