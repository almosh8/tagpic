using System.Diagnostics;
using System.Windows.Forms.VisualStyles;
using TagPic;

namespace tagpic
{
    
    public partial class ImageConfirmationForm : Form
    {
        private const int FORM_WIDTH = 500;
        private const int FORM_HEIGHT = 350; // Increased form height to accommodate the new control
        private const int BUTTON_WIDTH = 75;
        private const int BUTTON_HEIGHT = 30;
        private const int MARGIN = 10;

        public Image image;
        public string fileName;
        private string tags; // Added new field to store tags entered by the user

        private PictureBox pictureBox;
        private Label label;
        private Button yesButton;
        private Button noButton;
        private FlowLayoutPanel tagsFlowLayout; // Added new control for displaying tags
        private TextBox tagsTextBox; // Added new control for entering tags


        public ImageConfirmationForm(Image image)
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.Select();
            //this.KeyPress += new KeyPressEventHandler(ImageConfirmationForm_KeyPress);

            this.image = image;
            this.fileName = "image_" + DateTime.Now.ToString("yyyyMMddHHmmss"); // Removed ".png" extension from file name

            //this.ClientSize = new Size(FORM_WIDTH, FORM_HEIGHT);

            // Add the picture box to display the image
            this.pictureBox = new PictureBox();
            this.pictureBox.Image = this.image;
            this.pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox.Dock = DockStyle.Fill;
            this.Controls.Add(this.pictureBox);

            // Add the label asking the user to confirm
            this.label = new Label();
            this.label.Text = "Enter tags for this image." +
                "Press Tab or Space buttons to add new tag." +
                "Press Enter to save picture";
            this.label.Font = new Font(this.label.Font.FontFamily, 14, FontStyle.Bold);
            this.label.AutoSize = true;
            this.label.Dock = DockStyle.Top;
            this.label.Padding = new Padding(MARGIN);
            this.Controls.Add(this.label);

            this.tagsFlowLayout = new FlowLayoutPanel();
            this.tagsFlowLayout.Dock = DockStyle.Top;
            this.tagsFlowLayout.Padding = new Padding(MARGIN);
            this.tagsFlowLayout.FlowDirection = FlowDirection.LeftToRight;
            this.Controls.Add(tagsFlowLayout);

            // Add the textbox for entering tags
            this.tagsTextBox = new TextBox();
            this.tagsTextBox.Width = FORM_WIDTH - (2 * MARGIN);
            this.tagsTextBox.Height = BUTTON_HEIGHT;
            this.tagsTextBox.Dock = DockStyle.Top;
            this.tagsTextBox.Margin = new Padding(MARGIN);
            this.tagsTextBox.KeyDown += new KeyEventHandler(tagsTextBox_KeyDown);
            this.tagsTextBox.PreviewKeyDown += new PreviewKeyDownEventHandler(tagsTextBox_PreviewKeyDown);
            this.tagsTextBox.Select();

            this.tagsTextBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.tagsTextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;


            AutoCompleteStringCollection autoCompleteOptions = new AutoCompleteStringCollection();
            // Retrieve the list of tags from the TagStorage
            List<string> tagsList = TagsStorage.GetAllTags();

            // Add the tags to the autoCompleteOptions collection
            autoCompleteOptions.AddRange(tagsList.ToArray());

            // Set the AutoCompleteCustomSource property of the TextBox to the autoCompleteOptions collection
            this.tagsTextBox.AutoCompleteCustomSource = autoCompleteOptions;
            this.tagsFlowLayout.Controls.Add(this.tagsTextBox);

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
            // Set the default file name and location

            AddNewTag();

            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(), 
                "Images", this.fileName + ".png");

            // Create the directory if it doesn't exist
            Directory.CreateDirectory(
                Path.GetDirectoryName(filePath));

            // Save the image to the file
            this.image.Save(filePath);
            TagsStorage.AddTags(filePath);

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

        private void AddNewTag()
        {
            if (tagsTextBox.Text.Length == 0)
                return;

            TagsStorage.AddTag(tagsTextBox.Text);
            fileName += '_' + tagsTextBox.Text;

            // Create a new tagTextBox for entering the next tag
            var newTagTextBox = new TextBox();
            newTagTextBox.Width = FORM_WIDTH - (2 * MARGIN);
            newTagTextBox.Height = BUTTON_HEIGHT;
            newTagTextBox.Margin = new Padding(MARGIN, 0, MARGIN, MARGIN);
            newTagTextBox.KeyDown += new KeyEventHandler(tagsTextBox_KeyDown);
            newTagTextBox.PreviewKeyDown += new PreviewKeyDownEventHandler(tagsTextBox_PreviewKeyDown);
            newTagTextBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            newTagTextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;

            List<string> tagsList = TagsStorage.GetAllTags();
            // Add the tags to the autoCompleteOptions collection
            var newAutoCompleteOptions = new AutoCompleteStringCollection();
            newAutoCompleteOptions.AddRange(tagsList.ToArray());

            // Set the AutoCompleteCustomSource property of the TextBox to the autoCompleteOptions collection
            newTagTextBox.AutoCompleteCustomSource = newAutoCompleteOptions;

            tagsFlowLayout.Controls.Add(newTagTextBox);
            newTagTextBox.Select();
            this.tagsTextBox.Enabled = false;
            this.tagsTextBox = newTagTextBox;
        }

        private void tagsTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Debug.WriteLine(tagsTextBox.Text);

            List<string> tagsList = TagsStorage.GetAllTags();

            Debug.WriteLine(e.KeyCode);

            if (e.KeyCode == Keys.Space)
            {
                AddNewTag();
            }

            if (e.KeyCode == Keys.Right &&
            tagsList.Contains(tagsTextBox.Text))
            {
                AddNewTag();
            }

            if (e.KeyCode == Keys.Enter)
            {
                yesButton_Click(sender, e);
            }

            if (e.KeyCode == Keys.Escape)
            {
                noButton_Click(sender, e);
            }


        }

        private void tagsTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            List<string> tagsList = TagsStorage.GetAllTags();

            if (e.KeyCode == Keys.Tab &&
            tagsList.Contains(tagsTextBox.Text))
            {
                AddNewTag();
                e.IsInputKey = true;
            }
        }

    }
}
