using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sentize
{
    public class ErrorMessageBox
    {
        private const int ErrorMessageDuration = 5000;

        private TextBlock textBlock;
        private MainWindow mainWindow;

        public ErrorMessageBox(TextBlock textBlock, MainWindow mainWindow)
        {
            this.textBlock = textBlock;
            this.mainWindow = mainWindow;

            textBlock.Foreground = Brushes.Red;
        }

        public async void ShowMessage(string msg)
        {
            await Task.Run(() =>
            {
                mainWindow.Dispatcher.Invoke(() =>
                {
                    textBlock.Text = msg;
                });

                Thread.Sleep(ErrorMessageDuration);

                mainWindow.Dispatcher.Invoke(() =>
                {
                    textBlock.Text = "";
                });
            });
        }
    }
}
