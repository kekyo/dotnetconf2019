using System.IO;
using System.Text;
using System.Windows;

namespace WpfAppModelDesignWithAsync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.button.Click += Button_ClickUseDataAccessor;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var fs = new FileStream("data.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 65536, true))
            {
                var tr = new StreamReader(fs, Encoding.UTF8);
                var value = int.TryParse(await tr.ReadToEndAsync(), out var v) ? v : 0;

                this.textBox1.AppendText($"Value0={value}\r\n");

                fs.Position = 0;
                var tw = new StreamWriter(fs, Encoding.UTF8);
                await tw.WriteLineAsync((++value).ToString());
                await tw.FlushAsync();

                this.textBox2.AppendText($"Value1={value}\r\n");
            }
        }

        private async void Button_ClickNonMarshaling(object sender, RoutedEventArgs e)
        {
            using (var fs = new FileStream("data.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 65536, true))
            {
                var tr = new StreamReader(fs, Encoding.UTF8);
                var value = int.TryParse(
                    await tr.ReadToEndAsync().ConfigureAwait(false), out var v) ? v : 0;

                Dispatcher.Invoke(() => this.textBox1.AppendText($"Value0={value}\r\n"));

                fs.Position = 0;
                var tw = new StreamWriter(fs, Encoding.UTF8);
                await tw.WriteLineAsync((++value).ToString());
                await tw.FlushAsync();

                Dispatcher.Invoke(() => this.textBox2.AppendText($"Value1={value}\r\n"));
            }
        }

        private async void Button_ClickUseDataAccessor(object sender, RoutedEventArgs e)
        {
            var (value0, value1) = await DataAccessor.IncrementAsync("data.txt");

            this.textBox1.AppendText($"Value0={value0}\r\n");
            this.textBox2.AppendText($"Value1={value1}\r\n");
        }
    }
}
