using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GlobalHook.Core;
using Gma.System.MouseKeyHook;
using TagPic;

namespace tagpic
{
    public partial class Form1 : Form
    {
        private const int BUTTON_HEIGHT = 30;
        private const int PANEL_WIDTH = 600;
        private const int PANEL_MARGIN = 10;
        private const int MARGIN = 10;
        private const int FORM_WIDTH = 500;


        private List<ImageWithTags> images;
        private List<string> selectedTags;
        private System.Windows.Forms.Panel panel;
        private bool isAddingImage = false;

        private IKeyboardMouseEvents m_GlobalHook;

        private FlowLayoutPanel tagsFlowLayout; // Added new control for displaying tags
        private TextBox tagsTextBox; // Added new control for entering tags
        private Label label;

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.KeyPreview = true; // Set the KeyPreview property to true to enable detecting key presses on the form

            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.KeyDown += GlobalHookKeyDown;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_GlobalHook.Dispose();
        }

        private void GlobalHookKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert && Clipboard.ContainsImage())
            {
                // Handle the key press
                ImageConfirmationForm form = new ImageConfirmationForm(Clipboard.GetImage());
                form.TopMost = true; // Set TopMost property to true
                form.Activate();
                DialogResult result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // Get the file path from the confirmation form
                    string fileName = form.fileName;

                    // Load the image from the file and add it to the list
                    Image savedImage = form.image;
                    this.images.Add(new ImageWithTags(savedImage, fileName));
                    // Display the images
                    display_images();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.selectedTags = new List<string>();

            // Add the scrollable control
            ScrollableControl scrollableControl = new ScrollableControl();
            scrollableControl.Dock = DockStyle.Fill;
            scrollableControl.AutoScroll = true;
            this.Controls.Add(scrollableControl);
            Debug.WriteLine("constructor fired");

            // Add the panel to the scrollable control
            this.panel = new Panel();
            this.panel.Width = this.Width;
            this.panel.Margin = new Padding(PANEL_MARGIN);
            scrollableControl.Controls.Add(this.panel);

            // Set the location of the panel
            this.panel.Location = new Point((scrollableControl.ClientSize.Width - this.panel.Width) / 2, PANEL_MARGIN);

            // Load any previously saved images
            string imageDir = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            DirectoryInfo di = new DirectoryInfo(imageDir);
            FileInfo[] imageFiles = di.GetFiles("*.png");

            images = new List<ImageWithTags>();
            foreach (FileInfo file in imageFiles)
            {
                Image image = Image.FromFile(file.FullName);
                string name = file.Name;
                images.Add(new ImageWithTags(image, name));
            }
            display_images();

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

            if (e.KeyCode == Keys.Escape)
            {
                //noButton_Click(sender, e);
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

        private void AddNewTag()
        {
            if (tagsTextBox.Text.Length == 0)
                return;

            selectedTags.Add(tagsTextBox.Text);

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

            display_images();
        }

        private void display_images()
        {
            // Clear the panel
            this.panel.Controls.Clear();

            // Add the images that have at least one of the given tags to the panel
            int y = 0;
            foreach (ImageWithTags imageWithTags in images)
            {
                if (selectedTags.Count == 0 || imageWithTags.Tags.All(tag => selectedTags.Contains(tag)))
                {
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Image = imageWithTags.Image;
                    pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
                    pictureBox.Location = new Point(0, y);
                    this.panel.Controls.Add(pictureBox);
                    y += pictureBox.Height + PANEL_MARGIN;
                }
            }

            // Resize the panel to fit the images
            this.panel.Height = y;
        }
    }
}
