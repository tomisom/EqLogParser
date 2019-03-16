using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace EqLogParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileSystemWatcher m_Watcher;
        private FileStream m_FileStream;
        private StreamReader m_StreamReader;
        public string folderMonitorPath = Properties.Settings.Default.monitorFolder;
        public string currentFileName = Properties.Settings.Default.currentFileName;

        private long lastPosition;

        public MainWindow()
        {
            InitializeComponent();
            lastPosition = 0;

            string filePath = folderMonitorPath + currentFileName;
            m_FileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            m_StreamReader = new StreamReader(m_FileStream);

            this.Activated += MainWindow_Activated;
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => updateDisplay());
            fileWatch();
        }

        public void fileWatch()
        {
            if (folderMonitorPath != "")
            {
                m_Watcher = new FileSystemWatcher();
                m_Watcher.Filter = currentFileName;
                m_Watcher.Path = folderMonitorPath;
                m_Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess;
                m_Watcher.Changed += new FileSystemEventHandler(OnChanged);
            }
        }

        public void OnChanged(object sender, FileSystemEventArgs e)
        {
            Task.Factory.StartNew(() => updateDisplay());
        }

        public void displayNewLines()
        {
            this.txtLogOutput.AppendText(m_StreamReader.ReadToEnd());
        }

        public void updateDisplay()
        {
            DispatcherOperation op = Dispatcher.BeginInvoke((Action)(() =>
            {
                string line;
                while (!m_StreamReader.EndOfStream)
                {
                    line = m_StreamReader.ReadLine();
                    txtLogOutput.AppendText(line + "\r\n");
                }
                txtLogOutput.ScrollToEnd();
            }));
        }
    }
}
