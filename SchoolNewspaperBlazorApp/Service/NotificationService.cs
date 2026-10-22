using Microsoft.Data.SqlClient;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Service;

namespace SchoolNewspaperBlazorApp.Service
{
    public class NotificationService : INotificationService
    {
        private readonly Queue<NotificationMessage> _queue = new();
        public event Action? OnChange;
        public void Show(string message, EnumNotificationType type)
        {
            _queue.Enqueue(new NotificationMessage { Message = message, Type = type });
            OnChange?.Invoke();
        }
        public bool TryDequeue(out NotificationMessage? message)
        {
            if (_queue.Count > 0)
            {
                message = _queue.Dequeue();
                return true;
            }
            message = null;
            return false;
        }
    }
}
