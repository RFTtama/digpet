using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digpet.Modules
{
    public static class TaskMessager
    {
        public static void OutputMessage(string message, string name)
        {
            OutputMessage(message, name, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        public static void OutputMessage(string message, string name, MessageBoxButtons button, MessageBoxIcon icon)
        {
            Task.Run(() =>
            {
                MessageBox.Show(message, name, button, icon);
            });
        }
    }
}
