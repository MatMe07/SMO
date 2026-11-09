using Vichislenia;

Demo("1. Одноканальная СМО с отказами", SmoType.SingleRefusals,
    new SmoInput
    {
        Lambda = 1,
        LambdaUnit = TimeUnit.Minute,
        TObs = 0.5,
        TObsUnit = TimeUnit.Minute
    });

Demo("2. Одноканальная СМО с ограниченной очередью", SmoType.SingleLimitedQueue,
    new SmoInput
    {
        Lambda = 1,
        LambdaUnit = TimeUnit.Minute,
        Mu = 1.5,
        MuUnit = TimeUnit.Minute,
        QueueLength = 3
    });

Demo("3. Одноканальная СМО с неограниченной очередью", SmoType.SingleUnlimitedQueue,
    new SmoInput
    {
        Lambda = 1,
        LambdaUnit = TimeUnit.Minute,
        Mu = 1.5,
        MuUnit = TimeUnit.Minute
    });

Demo("4. Многоканальная СМО с отказами", SmoType.MultiRefusals,
    new SmoInput
    {
        Lambda = 10,
        LambdaUnit = TimeUnit.Hour,
        Mu = 4,
        MuUnit = TimeUnit.Hour,
        Channels = 3
    });

Demo("5. Многоканальная СМО с ограниченной очередью", SmoType.MultiLimitedQueue,
    new SmoInput
    {
        Lambda = 10,
        LambdaUnit = TimeUnit.Hour,
        Mu = 4,
        MuUnit = TimeUnit.Hour,
        Channels = 3,
        QueueLength = 2
    });

Demo("6. Многоканальная СМО с неограниченной очередью (домашнее задание)", SmoType.MultiUnlimitedQueue,
    new SmoInput
    {
        Lambda = 10,
        LambdaUnit = TimeUnit.Hour,
        Mu = 15,
        MuUnit = TimeUnit.Hour,
        Channels = 2
    });

Demo("7. Проверка единиц: λ в час, t_обс в минутах", SmoType.SingleUnlimitedQueue,
    new SmoInput
    {
        Lambda = 10,
        LambdaUnit = TimeUnit.Hour,
        TObs = 5,
        TObsUnit = TimeUnit.Minute
    });


Demo("8. Одноканальная СМО с неограниченной очередью", SmoType.SingleUnlimitedQueue,
    new SmoInput
    {
        Lambda = 10,
        LambdaUnit = TimeUnit.Hour,
        TObs = 5,
        TObsUnit = TimeUnit.Minute
    });


static void Demo(string title, SmoType type, SmoInput input)
{
    Console.WriteLine(new string('=', 78));
    Console.WriteLine(title);

    string service = input.Mu.HasValue
        ? $"μ = {input.Mu} /{input.MuUnit.Ru()}"
        : $"t_обс = {input.TObs} {input.TObsUnit.Ru()}";

    Console.WriteLine($"λ = {input.Lambda} заявок/{input.LambdaUnit.Ru()}; {service}; n = {input.Channels}; m = {input.QueueLength}");
    Console.WriteLine();

    SmoResult result = SmoCalculator.Calculate(type, input);

    if (!result.Success)
    {
        Console.WriteLine($"Ошибка: {result.Error}");
        return;
    }

    Console.WriteLine("Шаги расчёта:");
    foreach (CalculationStep step in result.Steps)
        Console.WriteLine("  " + step);

    Console.WriteLine();
    Console.WriteLine("Характеристики:");
    Console.WriteLine($"  ρ      = {ObshieVichislia.Format(result.Rho)}");
    Console.WriteLine($"  P0     = {ObshieVichislia.Format(result.P0)}");
    Console.WriteLine($"  P_отк  = {ObshieVichislia.Format(result.P_otk)}");
    Console.WriteLine($"  Q      = {ObshieVichislia.Format(result.Q)}");
    Console.WriteLine($"  A      = {ObshieVichislia.Format(result.A)}");
    if (result.P_wait.HasValue)
        Console.WriteLine($"  P_ожид = {ObshieVichislia.Format(result.P_wait.Value)}");
    Console.WriteLine($"  L_оч   = {ObshieVichislia.Format(result.L_och)}");
    Console.WriteLine($"  T_оч   = {ObshieVichislia.Format(result.T_och)}");
    Console.WriteLine($"  L_сист = {ObshieVichislia.Format(result.L_sist)}");
    Console.WriteLine($"  T_сист = {ObshieVichislia.Format(result.T_sist)}");
    if (result.K_zan.HasValue)
        Console.WriteLine($"  K_зан  = {ObshieVichislia.Format(result.K_zan.Value)}");
    Console.WriteLine();
}
