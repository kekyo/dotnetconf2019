using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfAppRaceConditionWithAsync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            button.Click += this.Button_Click_NonMarshaling;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var fs = new FileStream("data.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 65536, true))
            {
                var tr = new StreamReader(fs, Encoding.UTF8);
                var value = int.TryParse(await tr.ReadToEndAsync(), out var v) ? (v + 1) : 0;
                fs.Position = 0;
                var tw = new StreamWriter(fs, Encoding.UTF8);
                await tw.WriteLineAsync(value.ToString());
                await tw.FlushAsync();

                this.textBox.AppendText($"Value={value}\r\n");
            }
        }

        private async void Button_Click_EachTransfer(object sender, RoutedEventArgs e)
        {
            using (var fs = new FileStream("data.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 65536, true))
            {
                var tr = new StreamReader(fs, Encoding.UTF8);
                var value = int.TryParse(
                    await Task.Run(() => tr.ReadToEnd()), out var v) ? (v + 1) : 0;
                fs.Position = 0;
                var tw = new StreamWriter(fs, Encoding.UTF8);
                await Task.Run(() => tw.WriteLine(value.ToString()));
                await Task.Run(() => tw.Flush());

                this.textBox.AppendText($"Value={value}\r\n");
            }
        }

        private async void Button_Click_NonMarshaling(object sender, RoutedEventArgs e)
        {
            using (var fs = new FileStream("data.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 65536, true))
            {
                var tr = new StreamReader(fs, Encoding.UTF8);
                var value = int.TryParse(
                    await tr.ReadToEndAsync().ConfigureAwait(false),
                    out var v) ? (v + 1) : 0;
                fs.Position = 0;
                var tw = new StreamWriter(fs, Encoding.UTF8);
                await tw.WriteLineAsync(value.ToString());
                await tw.FlushAsync();

                Dispatcher.Invoke(() => this.textBox.AppendText($"Value={value}\r\n"));
            }
        }
    }
}
