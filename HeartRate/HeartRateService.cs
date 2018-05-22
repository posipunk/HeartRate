using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace HeartRate
{
    public enum ContactSensorStatus
    {
        NotSupported,
        NotSupported2,
        NoContact,
        Contact
    }

    public class HeartRateService : IDisposable
    {
        // https://www.bluetooth.com/specifications/gatt/viewer?attributeXmlFile=org.bluetooth.characteristic.heart_rate_measurement.xml
        private const int _heartRateMeasurementCharacteristicId = 0x2A37;

        private GattDeviceService _service;
        private readonly object _disposeSync = new object();
        private bool _isDisposed;

        public event HeartRateUpdateEventHandler HeartRateUpdated;
        public delegate void HeartRateUpdateEventHandler(ContactSensorStatus status, int bpm);

        public String filename;

        public void LogData(HeartRateServiceData data)
        {
            String line="";
            if(!File.Exists(filename))
            {
                line += HeartRateServiceData.GetCsVHeader() + Environment.NewLine;
            }
            line += data.ToString();
            using (var streamWriter = new StreamWriter(filename, true))
            {
                streamWriter.WriteLine(line);
            }

        }

        public void InitiateDefault()
        {
            filename = "polar" + DateTime.Now.ToString("yyyyMMddhhmmss") +".csv";
            var heartrateSelector = GattDeviceService
                .GetDeviceSelectorFromUuid(GattServiceUuids.HeartRate);

            var devices = AsyncResult(DeviceInformation
                .FindAllAsync(heartrateSelector));

            var device = devices.FirstOrDefault();

            if (device == null)
            {
                throw new ArgumentOutOfRangeException(
                    "Unable to locate heart rate device.");
            }

            GattDeviceService service;

            lock (_disposeSync)
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                Cleanup();

                service = AsyncResult(GattDeviceService.FromIdAsync(device.Id));

                _service = service;
            }

            var uuid = BluetoothUuidHelper.FromShortId(_heartRateMeasurementCharacteristicId);
            var heartrate = AsyncResult(service.GetCharacteristicsForUuidAsync(uuid)).Characteristics.FirstOrDefault();

            if (heartrate == null)
            {
                throw new ArgumentOutOfRangeException(
                    $"Unable to locate heart rate measurement on device {device.Name} ({device.Id}).");
            }

            var status = AsyncResult(
                heartrate.WriteClientCharacteristicConfigurationDescriptorAsync(
                    GattClientCharacteristicConfigurationDescriptorValue.Notify));

            heartrate.ValueChanged += HeartRate_ValueChanged;

            Debug.WriteLine($"Started {status}");
        }

        private void HeartRate_ValueChanged(
            GattCharacteristic sender,
            GattValueChangedEventArgs args)
        {
            var value = args.CharacteristicValue;

            if (value.Length == 0)
            {
                return;
            }

            using (var reader = DataReader.FromBuffer(value))
            {
                byte[] data = new byte[value.Length];
                reader.ReadBytes(data);
                var bpm = -1;
                var flags = data[0];
                var isshort = (flags & 1) == 1; // 1 == UINT16
                var contactSensor = (ContactSensorStatus)((flags >> 1) & 3);
                var minLength = isshort ? 3 : 2;

                if (value.Length < minLength)
                {
                    Debug.WriteLine($"Buffer was too small. Got {value.Length}, expected {minLength}.");
                    return;
                }

                HeartRateServiceData hrsd = new HeartRateServiceData(data);
                LogData(hrsd);

                bpm = hrsd.HeartRateMeasurement;

                Debug.WriteLine($"Read {flags.ToString("X")} {contactSensor} {bpm}");
                foreach(var rr in hrsd.RRIntervals)
                {
                    Debug.WriteLine($"RR Interval {rr}");
                }

                HeartRateUpdated?.Invoke(contactSensor, bpm);
            }
        }

        private void Cleanup()
        {
            var service = Interlocked.Exchange(ref _service, null);

            if (service == null)
            {
                return;
            }

            try
            {
                service.Dispose();
            }
            catch { }
        }

        private static T AsyncResult<T>(IAsyncOperation<T> async)
        {
            while (true)
            {
                switch (async.Status)
                {
                    case AsyncStatus.Started:
                        Thread.Sleep(10);
                        continue;
                    case AsyncStatus.Completed:
                        return async.GetResults();
                    case AsyncStatus.Error:
                        throw async.ErrorCode;
                    case AsyncStatus.Canceled:
                        throw new TaskCanceledException();
                }
            }
        }

        public void Dispose()
        {
            lock (_disposeSync)
            {
                _isDisposed = true;

                Cleanup();
            }
        }
    }
}
