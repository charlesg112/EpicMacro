using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EpicMacro.Models
{
    public static class UserEventValueConverter
    {

        public static Tuple<int, int> GetClickValue()
        {
            throw new NotImplementedException();
        }

        public static string GetString(UserEventType userEventType)
        {
            string output = "";
            switch (userEventType)
            {
                case UserEventType.Click:
                    output = "Click"; break;
                case UserEventType.KeyPress:
                    output = "Key Press"; break;
                case UserEventType.Delay:
                    output = "Delay"; break;
                case UserEventType.LongClick:
                    output = "Long Click"; break;
                case UserEventType.Loop:
                    output = "Loop"; break;
                case UserEventType.EndLoop:
                    output = "End Loop"; break;
            }

            return output;
        }

        public static UserEventType GetType(string userEventTypeString)
        {
            string input = userEventTypeString.Trim().ToLower();
            UserEventType output = UserEventType.Unknown;
            switch (input)
            {
                case "click":
                    output = UserEventType.Click; break;
                case "keypress":
                    output = UserEventType.KeyPress; break;
                case "delay":
                    output = UserEventType.Delay; break;
                case "long click":
                    output = UserEventType.LongClick; break;
                case "loop":
                    output = UserEventType.Loop; break;
                case "end loop":
                    output = UserEventType.EndLoop; break;
            }

            return output;

        }

        public static string GetDefaultFieldValue(UserEventType userEventType)
        {
            string output = "?";
            switch (userEventType)
            {
                case UserEventType.Click:
                    output = "(200, 200)"; break;
                case UserEventType.KeyPress:
                    output = "q"; break;
                case UserEventType.Delay:
                    output = "1.0"; break;
            }
            return output;
        }

        public static int GetComboBoxID(UserEventType userEventType)
        {
            int output = 0;
            switch (userEventType)
            {
                case UserEventType.Click:
                    output = 0; break;
                case UserEventType.KeyPress:
                    output = 1; break;
                case UserEventType.Delay:
                    output = 2; break;
                case UserEventType.LongClick:
                    output = 3; break;
                case UserEventType.Loop:
                    output = 4; break;
                case UserEventType.EndLoop:
                    output = 4; break;
            }
            return output;
        }

    }
}
