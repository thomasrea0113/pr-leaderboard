using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Leaderboard.Services
{
    public interface IMessageIEnumerable<T>
    {
        void PushMessage(T message);
        IEnumerable<T> GetAllMessages(bool peek = false);
    }

    public interface IMessageQueue : IMessageIEnumerable<string> { }

    public class TempDataMessageQueue : IMessageQueue
    {
        private readonly ITempDataDictionary _tempData;
        public const string MessageQueryKey = "QueuedMessages";
        private static InvalidOperationException InvalidType(object type)
            => new InvalidOperationException($"TempData key {MessageQueryKey} was of type {type.GetType()}, not {typeof(IEnumerable<string>)}");

        public TempDataMessageQueue(ITempDataDictionaryFactory tempFactory, IHttpContextAccessor accessor)
        {
            _tempData = tempFactory.GetTempData(accessor.HttpContext);
        }

        public IEnumerable<string> GetAllMessages(bool peek = false)
        {
            object messages;

            if (!peek)
                if (_tempData.ContainsKey(MessageQueryKey))
                    messages = _tempData[MessageQueryKey];
                else
                    return Enumerable.Empty<string>();
            else
                messages = _tempData.Peek(MessageQueryKey);

            if (messages == default)
                return Enumerable.Empty<string>();

            if (messages is IEnumerable<string> enumerable)
                return enumerable;

            throw InvalidType(messages);
        }

        public void PushMessage(string message)
        {
            var exists = _tempData.TryGetValue(MessageQueryKey, out var messagesObject);

            if (!exists)
            {
                _tempData[MessageQueryKey] = new string[] { message };
            }
            else if (messagesObject is IEnumerable<string> messages)
            {
                // Need to evaluate the appended enumerable so that it can be serialized
                _tempData[MessageQueryKey] = messages.Append(message).ToArray();
            }
            else
            {
                throw InvalidType(messagesObject);
            }
        }
    }
}