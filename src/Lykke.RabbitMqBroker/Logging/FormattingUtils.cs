using System;

namespace Lykke.RabbitMqBroker.Logging
{
    internal static class FormattingUtils
    {
        internal static string FormatMilliseconds(double milliseconds)
        {
            var time = TimeSpan.FromMilliseconds(milliseconds);
            string formattedTime;
            if (time.TotalSeconds < 1)
            {
                formattedTime = $"{time.TotalMilliseconds:0.##} ms";
            }
            else if (time.TotalMinutes < 1)
            {
                formattedTime = $"{time.TotalSeconds:0.##} sec";
            }
            else if (time.TotalHours < 1)
            {
                formattedTime = $"{time.TotalMinutes:0.##} min";
            }
            else
            {
                formattedTime = $"{time.TotalHours:0.##} hours";
            }

            return formattedTime;
        }
    }
}
