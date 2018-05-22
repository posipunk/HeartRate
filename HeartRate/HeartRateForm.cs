using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace HeartRate
{
    public partial class HeartRateForm : Form
    {
        private readonly HeartRateService _service;
        private readonly object _disposeSync = new object();
        private readonly object _updateSync = new object();

        public HeartRateForm()
        {
            _service = new HeartRateService();

            InitializeComponent();
        }

        private void HeartRateForm_Load(object sender, EventArgs e)
        {
            String error = "";
            bool success = false;
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    // InitiateDefault is blocking. A better UI would show some type
                    // of status during this time, but it's not super important.
                    _service.InitiateDefault();
                    success = true;
                    break;
                }
                catch (Exception ex)
                {
                    // Do nothing
                    error = ex.Message;
                }
            }
            if(!success)
            {
                MessageBox.Show(
                        $"Unable to initialize bluetooth service. Exiting.\n{error}",
                        "Fatal exception",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                Environment.Exit(-1);
            }

            _service.HeartRateUpdated += Service_HeartRateUpdated;
        }

        private void Service_HeartRateUpdated(
            ContactSensorStatus status,
            int bpm)
        {
            try
            {
                Service_HeartRateUpdatedCore(status, bpm);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in Service_HeartRateUpdated {ex}");

                Debugger.Break();
            }
        }

        private void Service_HeartRateUpdatedCore(
            ContactSensorStatus status,
            int bpm)
        {
            var isDisconnected = bpm == 0 ||
                status == ContactSensorStatus.NoContact;

            var iconText = bpm.ToString();
            // <= 0 implies disabled.
            var isWarn = bpm >= 40 && bpm < 180;

            lock (_updateSync)
            {
                var color = isWarn ? Color.Red : Color.Blue;
                
                Invoke(new Action(() => {
                    labelHR.Text = iconText;
                    labelHR.ForeColor = color;
                }));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                lock (_disposeSync)
                {
                    TryDispose(_service);
                }
            }

            base.Dispose(disposing);
        }

        private static void TryDispose(IDisposable disposable)
        {
            if (disposable == null)
            {
                return;
            }

            try
            {
                disposable.Dispose();
            }
            catch { }
        }

        private void HeartRateForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                Dispose(true);
            }
        }

    }
}
