using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Werehouse
{
    public partial class FileUploaderForm : Form
    {
        private Button btnUpload = new Button();
        private Label lblStatus = new Label();
        private ProgressBar progressBar = new ProgressBar();

        private void InitializeComponent()
        {
            // Form settings
            this.Text = "File Uploader";
            this.Size = new System.Drawing.Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Button settings
            btnUpload.Text = "Select File";
            btnUpload.Location = new System.Drawing.Point(20, 20);

            // Label settings
            lblStatus.Text = "No file selected";
            lblStatus.Location = new System.Drawing.Point(20, 60);
            lblStatus.AutoSize = true;

            // ProgressBar settings
            progressBar.Location = new System.Drawing.Point(20, 100);
            progressBar.Size = new System.Drawing.Size(440, 30);
            progressBar.Visible = false;

            // Add controls to form
            this.Controls.Add(btnUpload);
            this.Controls.Add(lblStatus);
            this.Controls.Add(progressBar);
        }
    }
}
