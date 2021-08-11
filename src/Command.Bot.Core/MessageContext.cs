﻿using System.Threading.Tasks;
using Command.Bot.Core.Responders;
using Command.Bot.Core.Runner;
using SlackConnector;
using SlackConnector.Models;

namespace Command.Bot.Core
{
    public class MessageContext : IMessageContext
    {
        private readonly ISlackConnection _connection;

        public MessageContext(SlackMessage message, ISlackConnection connection)
        {
            _connection = connection;
            Message = message;
        }

        public SlackMessage Message { get; }

        public Task Say(string text)
        {
            return GetValue(new BotMessage() { Text = text});
        }

        public string Text => this.CleanMessage();

        public Task SayOutput(string text)
        {
            if (string.IsNullOrEmpty(text)) return Task.FromResult(true);
            return GetValue(new BotMessage() { Text = $"```{text}```"});
        }

        public Task SayError(string text)
        {
            if (string.IsNullOrEmpty(text)) return Task.FromResult(true);
            return GetValue(new BotMessage() { Text = $">>>`{text}`"});
        }
        public Task GetValue(BotMessage botMessage)
        {
            if (botMessage.ChatHub == null) botMessage.ChatHub = Message.ChatHub;
            if (string.IsNullOrEmpty(botMessage.Text)) return Task.FromResult(true);
            return _connection.Say(botMessage);
        }
    }
}