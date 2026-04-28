using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

var ip = IPAddress.Loopback;
const int port = 5000;
var listener = new TcpListener(ip, port);
listener.Start();

Console.WriteLine($"Сервер запущен: {ip}:{port}");

var menuPath = Path.Combine(AppContext.BaseDirectory, "menu.json");
if (!File.Exists(menuPath))
{
    var demo = new List<Dish>
    {
        new() { Id = 1, Name = "Бургер", Price = 220 },
        new() { Id = 2, Name = "Картофель фри", Price = 120 },
        new() { Id = 3, Name = "Кола", Price = 90 }
    };

    await SaveMenuAsync(menuPath, demo);
}

var menuLock = new SemaphoreSlim(1, 1);

while (true)
{
    var tcpClient = await listener.AcceptTcpClientAsync();
    _ = HandleClientAsync(tcpClient, menuPath, menuLock);
}

static async Task HandleClientAsync(TcpClient tcpClient, string menuPath, SemaphoreSlim menuLock)
{
    Console.WriteLine("Клиент подключен.");
    using var client = tcpClient;
    await using var stream = client.GetStream();
    using var reader = new StreamReader(stream, Encoding.UTF8);
    await using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

    string? line;
    while ((line = await reader.ReadLineAsync()) is not null)
    {
        Packet? packet;
        try
        {
            packet = JsonSerializer.Deserialize<Packet>(line);
        }
        catch
        {
            await SendAsync(writer, new Packet { Command = "Error", Message = "Неверный формат JSON" });
            continue;
        }

        if (packet is null || string.IsNullOrWhiteSpace(packet.Command))
        {
            await SendAsync(writer, new Packet { Command = "Error", Message = "Пустая команда" });
            continue;
        }

        switch (packet.Command)
        {
            case "WaitMenu":
            {
                var menu = await LoadMenuSafeAsync(menuPath, menuLock);
                await SendAsync(writer, new Packet { Command = "Menu", Menu = menu });
                break;
            }

            case "AddDish":
            {
                if (packet.Dish is null || packet.Dish.Id <= 0 || string.IsNullOrWhiteSpace(packet.Dish.Name) || packet.Dish.Price <= 0)
                {
                    await SendAsync(writer, new Packet { Command = "Error", Message = "Неверные данные блюда" });
                    break;
                }

                var menu = await LoadMenuSafeAsync(menuPath, menuLock);
                if (menu.Any(d => d.Id == packet.Dish.Id))
                {
                    await SendAsync(writer, new Packet { Command = "Error", Message = "Блюдо с таким ID уже есть" });
                    break;
                }

                menu.Add(packet.Dish);
                await SaveMenuSafeAsync(menuPath, menu, menuLock);

                await SendAsync(writer, new Packet { Command = "Ok", Message = "Блюдо добавлено" });
                Console.WriteLine($"Админ добавил блюдо: {packet.Dish.Id} {packet.Dish.Name}");
                break;
            }

            case "Order":
            {
                var menu = await LoadMenuSafeAsync(menuPath, menuLock);
                var selected = menu.Where(x => packet.OrderIds?.Contains(x.Id) == true).ToList();

                var totalCount = selected.Count;
                var totalPrice = selected.Sum(x => x.Price);

                await SendAsync(writer, new Packet
                {
                    Command = "Receipt",
                    Receipt = new Receipt
                    {
                        Items = selected,
                        Count = totalCount,
                        Total = totalPrice
                    }
                });

                break;
            }

            case "Payment":
            {
                var total = packet.ExpectedTotal ?? 0;
                var payment = packet.PaymentAmount ?? 0;
                if (payment < total)
                {
                    await SendAsync(writer, new Packet
                    {
                        Command = "PaymentResult",
                        Message = "Пойдешь мыть посуду!!!",
                        IsAccepted = false
                    });
                }
                else
                {
                    await SendAsync(writer, new Packet
                    {
                        Command = "PaymentResult",
                        Message = "Оплата принята. Приятного аппетита!",
                        IsAccepted = true
                    });
                }

                break;
            }

            default:
                await SendAsync(writer, new Packet { Command = "Error", Message = "Неизвестная команда" });
                break;
        }
    }

    Console.WriteLine("Клиент отключен.");
}

static async Task<List<Dish>> LoadMenuSafeAsync(string menuPath, SemaphoreSlim menuLock)
{
    await menuLock.WaitAsync();
    try
    {
        return await LoadMenuAsync(menuPath);
    }
    finally
    {
        menuLock.Release();
    }
}

static async Task SaveMenuSafeAsync(string menuPath, List<Dish> menu, SemaphoreSlim menuLock)
{
    await menuLock.WaitAsync();
    try
    {
        await SaveMenuAsync(menuPath, menu);
    }
    finally
    {
        menuLock.Release();
    }
}

static async Task<List<Dish>> LoadMenuAsync(string menuPath)
{
    var json = await File.ReadAllTextAsync(menuPath);
    return JsonSerializer.Deserialize<List<Dish>>(json) ?? new List<Dish>();
}

static async Task SaveMenuAsync(string menuPath, List<Dish> menu)
{
    var json = JsonSerializer.Serialize(menu, new JsonSerializerOptions { WriteIndented = true });
    await File.WriteAllTextAsync(menuPath, json);
}

static async Task SendAsync(StreamWriter writer, Packet packet)
{
    var json = JsonSerializer.Serialize(packet);
    await writer.WriteLineAsync(json);
}

sealed class Dish
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

sealed class Receipt
{
    public List<Dish> Items { get; set; } = new();
    public int Count { get; set; }
    public decimal Total { get; set; }
}

sealed class Packet
{
    public string? Command { get; set; }
    public string? Message { get; set; }

    public Dish? Dish { get; set; }
    public List<Dish>? Menu { get; set; }

    public List<int>? OrderIds { get; set; }
    public Receipt? Receipt { get; set; }

    public decimal? PaymentAmount { get; set; }
    public decimal? ExpectedTotal { get; set; }
    public bool? IsAccepted { get; set; }
}
