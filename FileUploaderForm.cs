using System;
using System.Windows.Forms;
using System.IO;
using Werehouse.Services;
using Werehouse.Models;

namespace Werehouse
{
    partial class FileUploaderForm : Form
    {
        ExcelOrderProcessorService _processorService;
        OrdersAdaptorService _ordersAdaptorService;
        public FileUploaderForm(ExcelOrderProcessorService processorService, OrdersAdaptorService ordersAdaptorService)
        {
            InitializeComponent();
            _processorService = processorService;
            _ordersAdaptorService = ordersAdaptorService;
            btnUpload.Click += BtnUpload_Click;
        }

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    lblStatus.Text = $"Selected: {Path.GetFileName(openFileDialog.FileName)}";
                    progressBar.Visible = true;

                    List<Order> orders =  _processorService.ReadOrdersFromExcel(openFileDialog.FileName);

                    //_processorService.
                }
            }
        }
    }
}
