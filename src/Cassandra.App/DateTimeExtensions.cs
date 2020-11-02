// <copyright file="DateTimeExtensions.cs" company="VAR Unit">
// Copyright (c) VAR Unit. All rights reserved.
// </copyright>

using System;
using System.Globalization;

namespace Cassandra
{
    public static class DateTimeExtensions
    {
        public static DateTime Max(this DateTime a, DateTime b)
        {
            return new DateTime(Math.Max(a.Ticks, b.Ticks));
        }

        public static DateTime Min(this DateTime a, DateTime b)
        {
            return new DateTime(Math.Min(a.Ticks, b.Ticks));
        }

        public static string ToRowKey(this DateTime a)
        {
            return a.Ticks.ToString("d19");
        }

        public static string ToDisplayString(this DateTime a)
        {
            return a.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToDisplayString(this DateTime? a)
        {
            return a?.ToDisplayString();
        }

        public static DateTime TruncateMiliseconds(this DateTime dateTime)
        {
            return dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond));
        }

        public static int GetIso8601WeekOfYear(this DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);

            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}