using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DotNetEnv;
using Discord.Commands;

class Program
{
    private static DiscordSocketClient? client;

    static async Task Main()
    {
        Env.Load();
        var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };
        client = new DiscordSocketClient(config);

        client.MessageReceived += HandleMessageAsync;

        client.Log += msg =>
        {
            Console.WriteLine(msg);
            return Task.CompletedTask;
        };

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        await Task.Delay(-1);
    }

    private static async Task HandleMessageAsync(SocketMessage message)
    {
        if (message.Author.IsBot) return;

        if (message is not SocketUserMessage userMessage) return;

        int argPos = 0;
        if (userMessage.HasStringPrefix("$", ref argPos))
        {
            var content = userMessage.Content.Substring(argPos).Trim();

            if (content.Equals("ping", StringComparison.OrdinalIgnoreCase))
            {
                await message.Channel.SendMessageAsync($"pong! {client?.Latency ?? 0}ms");
            }
        }
    }
}