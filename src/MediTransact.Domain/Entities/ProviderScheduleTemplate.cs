namespace MediTransact.Domain.Entities;

public class ProviderScheduleTemplate
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; }
    public Guid ProviderId { get; private set; }
    public Guid PracticeLocationId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public DateOnly EffectiveStartDate { get; private set; }
    public DateOnly? EffectiveEndDate { get; private set; }

    private ProviderScheduleTemplate() { }

    public ProviderScheduleTemplate(
        Guid providerId,
        Guid practiceLocationId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        DateOnly effectiveStartDate,
        DateOnly? effectiveEndDate,
        bool isActive)
    {
        ProviderId = providerId;
        PracticeLocationId = practiceLocationId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        EffectiveStartDate = effectiveStartDate;
        EffectiveEndDate = effectiveEndDate;
        IsActive = isActive;
        IsDeleted = false;
    }

    public void Update(
        Guid practiceLocationId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        DateOnly effectiveStartDate,
        DateOnly? effectiveEndDate,
        bool isActive)
    {
        PracticeLocationId = practiceLocationId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        EffectiveStartDate = effectiveStartDate;
        EffectiveEndDate = effectiveEndDate;
        IsActive = isActive;
        IsDeleted = false;
    }

    public bool Covers(DateTimeOffset start, DateTimeOffset end)
    {
        var date = DateOnly.FromDateTime(start.UtcDateTime);
        if (date < EffectiveStartDate) return false;
        if (EffectiveEndDate.HasValue && date > EffectiveEndDate.Value) return false;
        if (start.DayOfWeek != DayOfWeek || end.DayOfWeek != DayOfWeek) return false;

        var startTime = TimeOnly.FromDateTime(start.UtcDateTime);
        var endTime = TimeOnly.FromDateTime(end.UtcDateTime);
        return startTime >= StartTime && endTime <= EndTime && end > start;
    }
}
