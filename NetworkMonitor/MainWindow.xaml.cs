using System;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;

namespace NetworkMonitor
{
    public partial class MainWindow : Window
    {
        // Коллекции для хранения данных о сетевых подключениях и трафике
        private ObservableCollection<NetworkConnection> networkConnections = new ObservableCollection<NetworkConnection>();

        // Данные для графиков
        public ChartValues<double> SentData { get; set; } = new ChartValues<double>();
        public ChartValues<double> ReceivedData { get; set; } = new ChartValues<double>();

        // Интервал обновления
        private int updateInterval = 1; // интервал обновления в секундах

        public MainWindow()
        {
            InitializeComponent();
            ConnectionsDataGrid.ItemsSource = networkConnections;
            TrafficChart.DataContext = this; // Привязка данных к графику
            StartNetworkMonitor(); // Запуск мониторинга сетевых подключений и трафика
        }

        // Асинхронный метод для обновления данных о сетевых подключениях и трафике
        private async void StartNetworkMonitor()
        {
            while (true)
            {
                await Task.Delay(updateInterval * 1000); // Задержка на основе интервала обновления

                // Обновление сетевых подключений
                UpdateNetworkConnections();

                // Обновление данных о трафике
                UpdateTrafficData();
            }
        }

        // Метод для обновления сетевых подключений
        private void UpdateNetworkConnections()
        {
            // Очистка текущего списка подключений
            networkConnections.Clear();

            // Получаем все активные TCP-подключения
            var connections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();

            foreach (var connection in connections)
            {
                networkConnections.Add(new NetworkConnection
                {
                    LocalAddress = connection.LocalEndPoint.ToString(),
                    RemoteAddress = connection.RemoteEndPoint.ToString(),
                    Protocol = "TCP", // Можно добавить поддержку других протоколов
                    Status = connection.State.ToString(),
                    SentBytes = 0,  // Реальные данные о трафике можно собирать с использованием других методов
                    ReceivedBytes = 0
                });
            }

            // Для UDP подключений можно сделать аналогичный запрос
            // IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners();
        }

        // Метод для обновления данных о трафике
        private void UpdateTrafficData()
        {
            // Имитация данных о трафике для примера
            Random rand = new Random();
            SentData.Add(rand.Next(0, 500));  // Случайный объем отправленных данных (в байтах)
            ReceivedData.Add(rand.Next(0, 500));  // Случайный объем полученных данных (в байтах)

            // Очищаем старые данные, если их слишком много
            if (SentData.Count > 100) SentData.RemoveAt(0);
            if (ReceivedData.Count > 100) ReceivedData.RemoveAt(0);
        }

        // Обработчик для обновления интервала
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(UpdateIntervalTextBox.Text, out int interval))
            {
                updateInterval = interval;  // Обновляем интервал, если введено правильное число
            }
        }
    }

    // Класс для хранения информации о сетевом подключении
    public class NetworkConnection
    {
        public string LocalAddress { get; set; }
        public string RemoteAddress { get; set; }
        public string Protocol { get; set; }
        public string Status { get; set; }
        public long SentBytes { get; set; }
        public long ReceivedBytes { get; set; }
    }
}
