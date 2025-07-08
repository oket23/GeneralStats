using BrawlStarsService.Models;
using BrawlStarsService.Services;
using GeneralStats.Models;
using Serilog;
using Serilog.Core;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace GeneralStats.Services;

public class TgService
{
    private readonly TelegramBotClient _client;
    private readonly ILogger _logger;
    private readonly User _bot;
    private Dictionary<long, UserSession> _userSessions;
    private StringBuilder _sb;
    private readonly BSService _bsService;

    public TgService(Logger logger, string token, BSService bSService)
    {
        _bsService = bSService;
        _userSessions = new Dictionary<long, UserSession>();
        _sb = new StringBuilder();
        _logger = logger;

        _client = new TelegramBotClient(token);
        _bot = _client.GetMe().GetAwaiter().GetResult();
    }

    public void StartTelegramBotAsync()
    {
        Console.WriteLine($"Hello, World! I am user {_bot.Id} and my name is {_bot.FirstName}.");

         _client.OnMessage += TelegramClient_OnMessage;
         _client.OnError += TelegramClient_OnError;
         _client.OnUpdate += TelegramClient_OnUpdate;
    }

    private async Task TelegramClient_OnUpdate(Update update)
    {
        var chatId = update.CallbackQuery.Message.Chat.Id;
        try
        {
            if (!_userSessions.ContainsKey(chatId))
            {
                _userSessions[chatId] = new UserSession();
            }

            var session = _userSessions[chatId];

            if (update.CallbackQuery is { } query)
            {
                await _client.AnswerCallbackQuery(query.Id);

                switch (query.Data)
                {
                    case "show_bs_menu":
                        ResetSession(session);
                        await ShowBSMenu(chatId);
                        break;
                    case "show_cr_menu":
                        ResetSession(session);
                        await _client.SendMessage(chatId, "Comming soon...");
                        await ShowMainMenu(chatId);
                        break;
                    case "show_coc_menu":
                        ResetSession(session);
                        await _client.SendMessage(chatId, "Comming soon...");
                        await ShowMainMenu(chatId);
                        break;
                    case "back_to_main":
                        ResetSession(session);
                        await ShowMainMenu(chatId);
                        break;
                    case "back_to_bs":
                        ResetSession(session);
                        await ShowBSMenu(chatId);
                        break;
                    case "prev":                       
                    case "next":
                        await HandlePagination(chatId, query.Data, session);
                        break;
                }
                if (query.Data.StartsWith("bs_"))
                {
                    await BsMenuHandler(chatId, query, session);
                }
                else if (query.Data.StartsWith("cr_"))
                {
                    // Handle Clash Royale related queries here
                }
                else if (query.Data.StartsWith("coc_"))
                {
                    // Handle Clash of Clans related queries here
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"Update handling error: {ex.Message}");
        }
    }
    private async Task TelegramClient_OnMessage(Message msg, UpdateType type)
    {
        var chadId = msg.Chat.Id;
        try
        {
            if (!_userSessions.ContainsKey(msg.Chat.Id))
            {
                _userSessions[msg.Chat.Id] = new UserSession();
            }

            var session = _userSessions[msg.Chat.Id];

            if (msg.Chat.Type == ChatType.Group || msg.Chat.Type == ChatType.Supergroup)
            {
                return;
            }

            if (msg.Chat.Type == ChatType.Private && msg.Text != null && Regex.IsMatch(msg.Text, @"[\u1F600-\u1F64F\u2702\u2705\u2615\u2764\u1F4A9]+"))
            {
                switch (msg.Text)
                {
                    case "/start":
                        await ShowMainMenu(msg.Chat.Id);
                        break;
                }

                switch (session.UserStatus)
                {
                    case "bs_player_stats":
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(msg.Text))
                            {
                                var playerStats = await _bsService.GetPlayerStatsAsync(msg.Text);
                                session.LastPlayerTag = msg.Text;

                                await _client.SendMessage(msg.Chat.Id, playerStats.ToString());
                                await ShowBlawlers(msg.Chat.Id, session);

                                _logger.Information($"Tg show player stats");
                            }
                        }
                        catch (Exception ex)
                        {
                            await _client.SendMessage(chadId, ex.Message);
                            _logger.Error($"{ex.Message}");
                        }
                        break;

                    case "bs_club_stats":
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(msg.Text))
                            {
                                var clubStatus = await _bsService.GetClubStatsAsync(msg.Text);

                                await _client.SendMessage(msg.Chat.Id, clubStatus.ToString(), replyMarkup: new[] { InlineKeyboardButton.WithCallbackData("Back to menu", "back_to_bs") });

                                _logger.Information($"Tg show club stats");
                            }
                        }
                        catch (Exception ex)
                        {
                            await _client.SendMessage(chadId, ex.Message);
                            _logger.Error($"{ex.Message}");
                        }
                        break;

                }
            }
            else if (msg.Animation != null)
            {
                string animation = msg.Animation.FileId;
                await _client.SendAnimation(msg.Chat.Id, animation);
            }
            else if (msg.Sticker != null)
            {
                string sticker = msg.Sticker.FileId;
                await _client.SendSticker(msg.Chat.Id, sticker);
            }
            else
            {
                await _client.SendMessage(msg.Chat.Id, "I only accept text messages!");
            }
        }
        catch (Exception ex)
        {
            _logger.Error($"Update handling error: {ex.Message}");
        }
    }
    private async Task TelegramClient_OnError(Exception exception, HandleErrorSource source)
    {
        _logger.Error($"telegram bot has error: {exception.Message}");
    }


    private async Task ShowMainMenu(long chatId)
    {
        await _client.SendMessage(chatId, "Hello, choose game:", replyMarkup: new InlineKeyboardMarkup(new[]
        {
            new[] {InlineKeyboardButton.WithCallbackData("Brawl Stars","show_bs_menu") },
            new[] {InlineKeyboardButton.WithCallbackData("Clash Royale", "show_cr_menu") },
            new[] {InlineKeyboardButton.WithCallbackData("Clash Of Clans", "show_coc_menu") }
        }));  
    }
    private async Task ShowBSMenu(long chatId)
    {
        await _client.SendMessage(chatId, "Select option:", replyMarkup: new InlineKeyboardMarkup(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("Player Stats", "bs_player_stats") },
            new[] { InlineKeyboardButton.WithCallbackData("Club Stats", "bs_club_stats") },
            new[] { InlineKeyboardButton.WithCallbackData("Events", "bs_events") },
            new[] { InlineKeyboardButton.WithCallbackData("Back to Main Menu", "back_to_main") }
        }));
    }


    private async Task BsMenuHandler(long chatId, CallbackQuery query, UserSession session)
    {
        switch (query.Data)
        {
            case "bs_player_stats":
                session.UserStatus = "bs_player_stats";
                await _client.SendMessage(chatId, "Send player tag (without #):");
                break;

            case "bs_club_stats":
                session.UserStatus = "bs_club_stats";
                await _client.SendMessage(chatId, "Send club tag (without #):");
                break;

            case "bs_events":
                await ShowEvents(chatId);
                break;
        }
    }


    private async Task ShowEvents(long chatId)
    {
        _sb.Clear();
        _sb.AppendLine("Events:\n");

        var events = await _bsService.GetEventsAsync();

        foreach(var item in events)
        {
            _sb.AppendLine($"Map: {item.EventDesc.Map}");
            _sb.AppendLine($"Game mode: {item.EventDesc.Mode}");
            _sb.AppendLine($"Start Time: {item.StartTime}");
            _sb.AppendLine($"End Time: {item.EndTime}");
            _sb.AppendLine();
        }

        await _client.SendMessage(chatId, _sb.ToString(), replyMarkup: new[] { InlineKeyboardButton.WithCallbackData("Back to menu", "back_to_bs") });

        _logger.Information($"UI show pro teams");
    }
    private async Task ShowBlawlers(long chatId, UserSession session)
    {
        if (string.IsNullOrWhiteSpace(session.LastPlayerTag))
        {
            await _client.SendMessage(chatId, "No player tag found for refresh.");
            return;
        }

        try
        {
            var limit = 5;
            var playerStats = await _bsService.GetPlayerStatsAsync(session.LastPlayerTag);
            var paginated = playerStats.Brawlers.Skip((session.Page - 1) * limit).Take(limit).ToList();

            if (paginated.Count == 0)
            {
                await _client.SendMessage(chatId, "No more brawlers...", replyMarkup: new InlineKeyboardMarkup(new[]
                {
                        new[] { InlineKeyboardButton.WithCallbackData("Prev", "prev")},
                        new[] { InlineKeyboardButton.WithCallbackData("Back to menu", "back_to_bs") }
                }));
                return;
            }

            _sb.Clear();
            _sb.AppendLine($"Brawlers: ");
            _sb.AppendLine($"Page: {session.Page}\n");
            foreach (var brawler in paginated)
            {
                _sb.AppendLine(brawler.ToString());
            }

            if (session.LastMessageId.HasValue)
            {
                try
                {
                    await _client.DeleteMessage(chatId, session.LastMessageId.Value);
                }
                catch (Exception ex)
                {
                    _logger.Warning($"Could not delete message: {ex.Message}");
                }
            }

            var sentMessage = await _client.SendMessage(chatId, _sb.ToString(), replyMarkup: new InlineKeyboardMarkup(new[]
                {
                        new[] { InlineKeyboardButton.WithCallbackData("Prev", "prev"), InlineKeyboardButton.WithCallbackData("Next", "next") },
                        new[] { InlineKeyboardButton.WithCallbackData("Back to menu", "back_to_bs") }
                }));

            session.LastMessageId = sentMessage.MessageId;

            _logger.Information($"UI show brawlers - Page {session.Page}");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to show brawlers: {ex.Message}");
            await _client.SendMessage(chatId, "Error while loading brawlers.");
        }
    }

    private async Task HandlePagination(long chatId, string data, UserSession session)
    {
        if (data == "prev")
            session.Page = Math.Max(1, session.Page - 1);
        else if (data == "next")
            session.Page++;

        switch (session.UserStatus)
        {
            case "bs_player_stats":
                await ShowBlawlers(chatId, session);
                break;

                // якщо в тебе буде більше меню — додай інші тут
        }
    }

    private void ResetSession(UserSession session)
    {
        session.UserStatus = "null";
        session.Page = 1;
        session.LastPlayerTag = null;
        session.LastMessageId = null;
    }
}
