using System.IO.MemoryMappedFiles;
using System.Threading;

namespace _2DOF
{
    /// <summary>
    /// Этот класс используется для отправки данных
    /// </summary>
    public sealed class SendingData
    {
        /// <summary>
        /// Время ожидания между отправками данных.
        /// В миллисекундах.
        /// </summary>
        public const int WAIT_TIME = 20;
        private const int DATA_LENGTH = 6;
        private const int MAP_SIZE_BYTES = DATA_LENGTH * sizeof(double);

        /// <summary>
        /// Данные телеметрии объекта.
        /// </summary>
        public readonly ObjectTelemetryData ObjectTelemetryData = new();

        private const string MAP_NAME = "2DOFMemoryDataGrabber";
        private Thread _thread;
        private volatile bool _running;

        /// <summary>
        /// Запуск отправки данных.
        /// </summary>
        public void SendingStart()
        {
            if (_thread != null && _thread.IsAlive)
                return;

            _running = true;
            _thread = new Thread(HandlerData);
            _thread.IsBackground = true;
            _thread.Start();
        }

        /// <summary>
        /// Остановка отправки данных.
        /// </summary>
        public void SendingStop()
        {
            _running = false;

            if (_thread != null && _thread.IsAlive)
            {
                _thread.Join(WAIT_TIME * 2);
            }

            _thread = null;
        }

        private void HandlerData()
        {
            using var memoryMappedFile = MemoryMappedFile.CreateOrOpen(MAP_NAME, MAP_SIZE_BYTES);
            using var accessor = memoryMappedFile.CreateViewAccessor(0, MAP_SIZE_BYTES, MemoryMappedFileAccess.Write);

            while (_running)
            {
                accessor.WriteArray(0, ObjectTelemetryData.DataArray, 0, DATA_LENGTH);
                Thread.Sleep(WAIT_TIME);
            }

            // При остановке записываем нулевой пакет, чтобы платформа вернулась в базу
            var zeros = new double[DATA_LENGTH];
            accessor.WriteArray(0, zeros, 0, DATA_LENGTH);
        }
    }
}
