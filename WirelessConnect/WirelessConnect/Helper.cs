using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace WirelessConnect
{
    public static class Helper
    {
        public static class PreventTouchToMousePromotion
        {
            public static void Register(FrameworkElement root)
            {
                root.PreviewMouseDown += Evaluate;
                root.PreviewMouseMove += Evaluate;
                root.PreviewMouseUp += Evaluate;
            }

            private static void Evaluate(object sender, MouseEventArgs e)
            {
                if (e.StylusDevice != null)
                {
                    e.Handled = true;
                }
            }
        }

        // Environment.GetCommandLine was not working during execution on WinPE
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern System.IntPtr GetCommandLine();
        public static string GetRawCommandLine()
        {
            // Win32 API
            string s = Marshal.PtrToStringAuto(GetCommandLine());

            // or better, managed code as suggested by @mp3ferret
            // string s = Environment.CommandLine;
            return s.Substring(s.IndexOf('"', 1) + 1).Trim();
        }

        public static string[] GetRawArguments()
        {
            string cmdline = GetRawCommandLine();

            // Now let's split the arguments. 
            // Lets assume the fllowing possible escape sequence:
            // \" = "
            // \\ = \
            // \ with any other character will be treated as \
            //
            // You may choose other rules and implement them!

            var args = new ArrayList();
            bool inQuote = false;
            int pos = 0;
            StringBuilder currArg = new StringBuilder();
            while (pos < cmdline.Length)
            {
                char currChar = cmdline[pos];

                if (currChar == '"')
                {
                    currArg.Append(currChar);
                    inQuote = !inQuote;
                }
                else if (currChar == '\\')
                {
                    char nextChar = pos < cmdline.Length - 1 ? cmdline[pos + 1] : '\0';
                    if (nextChar == '\\' || nextChar == '"')
                    {
                        currArg.Append(nextChar);
                        pos += 2;
                        continue;
                    }
                    else
                    {
                        currArg.Append(currChar);
                    }
                }
                else if (inQuote || !char.IsWhiteSpace(currChar))
                {
                    currArg.Append(currChar);
                }
                if (!inQuote && char.IsWhiteSpace(currChar) && currArg.Length > 0)
                {
                    args.Add(currArg.ToString());
                    currArg.Clear();
                }
                pos++;
            }

            if (currArg.Length > 0)
            {
                args.Add(currArg.ToString());
                currArg.Clear();
            }
            return (string[])args.ToArray(typeof(string));
        }
    }
}
