using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfAppWithAsync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            button.Click += this.Button_Click;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var fs = new FileStream(
                "sample.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.None,
                65536, true))    // Asynchronous I/O
            {
                var tw = new StreamWriter(fs, Encoding.UTF8);

                var total = 0;
                for (var index = 0; index < 100; index++)
                {
                    total += index;
                    await tw.WriteLineAsync($"Index={index}, Total={total}");
                }

                await tw.FlushAsync();
            }
        }
    }
}
