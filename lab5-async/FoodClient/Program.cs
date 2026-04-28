using System.Net.Sockets;
using System.Text;
using System.Text.Json;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("Выберите режим: 1 - Администратор, 2 - Голодный клиент");
var mode = Console.ReadLine();

using var tcpClient = new TcpClient();
await tcpClient.ConnectAsync("127.0.0.1", 5000);
await using var stream = tcpClient.GetStream();
using var reader = new StreamReader(stream, Encoding.UTF8);
await using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

if (mode == "1")
{
    await RunAdminAsync(reader, writer);
}
else
{
    await RunCustomerAsync(reader, writer);
}

static async Task RunAdminAsync(StreamReader reader, StreamWriter writer)
{
    Console.WriteLine("=== Режим администратора ===");

    Console.Write("ID блюда: ");
    var id = int.Parse(Console.ReadLine() ?? "0");

    Console.Write("Название блюда: ");
    var name = Console.ReadLine() ?? string.Empty;

    Console.Write("Цена блюда: ");
    var price = decimal.Parse(Console.ReadLine() ?? "0");

    var addPacket = new Packet
    {
        Command = "AddDish",
        Dish = new Dish { Id = id, Name = name, Price = price }
    };

    await SendAsync(writer, addPacket);
    var answer = await ReceiveAsync(reader);

    Console.WriteLine(answer?.Message ?? "Нет ответа от сервера");
}

static async Task RunCustomerAsync(StreamReader reader, StreamWriter writer)
{
    Console.WriteLine("=== Режим клиента ===");

    await SendAsync(writer, new Packet { Command = "WaitMenu" });
    var menuResponse = await ReceiveAsync(reader);

    if (menuResponse?.Menu is null || menuResponse.Menu.Count == 0)
    {
        Console.WriteLine("Меню пустое");
        return;
    }

    Console.WriteLine("Доступное меню:");
    foreach (var dish in menuResponse.Menu)
    {
        Console.WriteLine($"{dish.Id}. {dish.Name} - {dish.Price} руб.");
    }

    Console.WriteLine("Введите ID блюд через запятую (например: 1,2):");
    var input = Console.ReadLine() ?? string.Empty;
    var orderIds = input
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Select(x => int.TryParse(x.Trim(), out var id) ? id : 0)
        .Where(x => x > 0)
        .ToList();

    await SendAsync(writer, new Packet { Command = "Order", OrderIds = orderIds });
    var receiptResponse = await ReceiveAsync(reader);

    if (receiptResponse?.Receipt is null)
    {
        Console.WriteLine("Не удалось получить чек");
        return;
    }

    Console.WriteLine("\nЧек:");
    foreach (var item in receiptResponse.Receipt.Items)
    {
        Console.WriteLine($"- {item.Name}: {item.Price} руб.");
    }

    Console.WriteLine($"Количество: {receiptResponse.Receipt.Count}");
    Console.WriteLine($"Итого: {receiptResponse.Receipt.Total} руб.");

    Console.Write("Внесите оплату: ");
    var payment = decimal.Parse(Console.ReadLine() ?? "0");

    await SendAsync(writer, new Packet
    {
        Command = "Payment",
        PaymentAmount = payment,
        ExpectedTotal = receiptResponse.Receipt.Total
    });

    var paymentResult = await ReceiveAsync(reader);
    Console.WriteLine(paymentResult?.Message ?? "Нет ответа по оплате");
}

static async Task SendAsync(StreamWriter writer, Packet packet)
{
    var json = JsonSerializer.Serialize(packet);
    await writer.WriteLineAsync(json);
}

static async Task<Packet?> ReceiveAsync(StreamReader reader)
{
    var line = await reader.ReadLineAsync();
    if (line is null) return null;

    return JsonSerializer.Deserialize<Packet>(line);
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
