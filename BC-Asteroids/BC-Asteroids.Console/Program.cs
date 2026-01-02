using BC_Asteroids.Console;
using BC_Asteroids.Shared;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

Console.WriteLine("This is the game interface for the web! Provide a game ID and it will set up the web socket and run your logic on every request.");
Console.WriteLine("If you wanted to play as a human, just call the API and specify the game ID as such: \"/{gameId}\".");

Console.WriteLine("Provide the Game ID.");
string input = Console.ReadLine()!;
int id;
try {
    id = Convert.ToInt32(input);
} catch {
    Console.WriteLine($"Could not convert input \"{input}\". Ending...");
    return;
}

Console.WriteLine($"Got game ID: {id}");

Console.WriteLine("Please provide the host URL to connect to. This should be like: \"http://localhost:5038\" or \"http://codyhowell.dev\". I will handle paths and protocols.");
string host = Console.ReadLine()!;
Uri url = new Uri(host);

HttpClient client = new() {
    BaseAddress = new Uri("http://" + url.Host + ":" + url.Port)
};

var response = await client.GetAsync($"/api/game/size/{id}");
GameSize? size = await response.Content.ReadFromJsonAsync<GameSize>();
if (size is null) {
    Console.WriteLine("Could not find game size.");
} else {
    Console.WriteLine($"Game size is {size.width} x {size.height}");
}

response = await client.PostAsync($"/api/game/register/{id}", null);
int playerId = (await response.Content.ReadFromJsonAsync<IdDTO>()).playerId;
Console.WriteLine($"Player registered as {playerId}");

Uri wsUrl = new("ws://" + url.Host + ":" + url.Port + $"/api/game/ws/{id}");
Console.WriteLine($"Sending socket request to {wsUrl}...");

var socket = new ClientWebSocket();
await socket.ConnectAsync(wsUrl, default);

Console.WriteLine("Connected!");

while (true) {
    var bytes = new byte[1024];
    var result = await socket.ReceiveAsync(bytes, default);
    string res = Encoding.UTF8.GetString(bytes, 0, result.Count);

    // Only keep this console in for debugging purposes. Otherwise, just use the provided objects.
    // Console.WriteLine(res);

    StringifiedGameObjects? stringObjects = JsonSerializer.Deserialize<StringifiedGameObjects>(res);
    if (stringObjects is null) {
        Console.WriteLine("Objects was not able to be parsed. Debug with the res."); 
        continue;
    }

    GameObjects gameObjects = new() {
        Players = [.. stringObjects.Players.Select(a => Player.FromTextFormat(a))],
        Asteroids = [.. stringObjects.Asteroids.Select(a => Asteroid.FromTextFormat(a))],
        Bullets = [.. stringObjects.Bullets.Select(a => Bullet.FromTextFormat(a))]   
    }; 

    Console.WriteLine(gameObjects);

    List<string> moves =
    [
        // Sample moves
        AvailableMoves.Accelerate(1),
        AvailableMoves.Left(),
    ];

    ///
    /// YOUR BOT GOES HERE
    ///
    /// Make sure you read the README document for details about 
    /// how the game works and, in particular, how this AvailableMoves 
    /// process works. 
    /// 
    


    await client.PostAsJsonAsync($"/api/game/move/{id}/{playerId}", moves);
}
// await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", default);