using System.Diagnostics;
using TagPic;

namespace tagpic
{

    public partial class ImageDeletingForm : Form
    {
        private const int FORM_WIDTH = 500;
        private const int FORM_HEIGHT = 350; // Increased form height to accommodate the new control
        private const int BUTTON_WIDTH = 75;
        private const int BUTTON_HEIGHT = 30;
        private const int MARGIN = 10;

        public ImageWithTags imageWithTags;
        public string fileName;
        private string tags; // Added new field to store tags entered by the user

        private PictureBox pictureBox;
        private Label label;
        private Button yesButton;
        private Button noButton;
        private FlowLayoutPanel tagsFlowLayout; // Added new control for displaying tags
        private TextBox tagsTextBox; // Added new control for entering tags


        public ImageDeletingForm(ImageWithTags imageWithTags)
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            //this.KeyPress += new KeyPressEventHandler(ImageConfirmationForm_KeyPress);

            this.imageWithTags = imageWithTags;

            //this.ClientSize = new Size(FORM_WIDTH, FORM_HEIGHT);

            // Add the picture box to display the image
            this.pictureBox = new PictureBox();
            this.pictureBox.Image = this.imageWithTags.Image;
            this.pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox.Dock = DockStyle.Fill;
            this.Controls.Add(this.pictureBox);

            // Add the label asking the user to confirm
            this.label = new Label();
            this.label.Text = "Image copied to clipboard" +
                "Are you sure you want to take this image and delete permanently?" +
                "Press Enter to delete picture permanently, Escape to cancel";
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
            this.CancelButton = this.noButton;

            //Debug.WriteLine(this.noButton.Text);

        }

        private void yesButton_Click(object sender, EventArgs e)
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Removed_Images", this.imageWithTags.fileName);

            // Create the directory if it doesn't exist
            Directory.CreateDirectory(
                Path.GetDirectoryName(filePath));

            // Backup the image to the file
            this.imageWithTags.Image.Save(filePath);

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

    }
}
