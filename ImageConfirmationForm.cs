namespace tagpic
{
    public partial class ImageConfirmationForm : Form
    {
        private const int FORM_WIDTH = 500;
        private const int FORM_HEIGHT = 300;
        private const int BUTTON_WIDTH = 75;
        private const int BUTTON_HEIGHT = 30;
        private const int MARGIN = 10;

        public Image image;
        private string fileName;

        private PictureBox pictureBox;
        private Label label;
        private Button yesButton;
        private Button noButton;

        public ImageConfirmationForm(Image image)
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;

            this.image = image;
            this.fileName = "image_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";


            //this.ClientSize = new Size(FORM_WIDTH, FORM_HEIGHT);

            // Add the picture box to display the image
            this.pictureBox = new PictureBox();
            this.pictureBox.Image = this.image;
            this.pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox.Dock = DockStyle.Fill;
            this.Controls.Add(this.pictureBox);

            // Add the label asking the user to confirm
            this.label = new Label();
            this.label.Text = "Do you want to add this image?";
            this.label.Font = new Font(this.label.Font.FontFamily, 14, FontStyle.Bold);
            this.label.AutoSize = true;
            this.label.Dock = DockStyle.Top;
            this.label.Padding = new Padding(MARGIN);
            this.Controls.Add(this.label);

            // Add the "Yes" button
            this.yesButton = new Button();
            this.yesButton.Text = "Yes";
            this.yesButton.Width = BUTTON_WIDTH;
            this.yesButton.Height = BUTTON_HEIGHT;
            this.yesButton.Dock = DockStyle.Right;
            this.yesButton.Margin = new Padding(MARGIN);
            this.yesButton.Click += new EventHandler(yesButton_Click);
            this.Controls.Add(this.yesButton);

            // Add the "No" button
            this.noButton = new Button();
            this.noButton.Text = "No";
            this.noButton.Width = BUTTON_WIDTH;
            this.noButton.Height = BUTTON_HEIGHT;
            this.noButton.Dock = DockStyle.Right;
            this.noButton.Margin = new Padding(MARGIN);
            this.noButton.Click += new EventHandler(noButton_Click);
            this.Controls.Add(this.noButton);

            this.AcceptButton = this.yesButton;
        }

        private void yesButton_Click(object sender, EventArgs e)
        {
            // Set the default file name and location
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", this.fileName);

            // Create the directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            // Save the image to the file
            this.image.Save(filePath);

            // Close the form with the result set to OK
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void noButton_Click(object sender, EventArgs e)
        {
            // Close the form with the result set to Cancel
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ImageConfirmationForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                yesButton_Click(sender, e);
            }
        }

    }


}
