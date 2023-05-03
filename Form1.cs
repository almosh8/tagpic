using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace tagpic
{
    public partial class Form1 : Form
    {
        private const int BUTTON_HEIGHT = 30;
        private const int PANEL_WIDTH = 600;
        private const int PANEL_MARGIN = 10;

        private List<Image> images = new List<Image>();
        private System.Windows.Forms.Panel panel;

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Add the "Fetch Image" button
            Button button1 = new Button();
            button1.Text = "Fetch Image";
            button1.Dock = DockStyle.Top;
            button1.Height = BUTTON_HEIGHT;
            button1.Click += new EventHandler(button1_Click);
            this.Controls.Add(button1);

            // Add the scrollable control
            ScrollableControl scrollableControl = new ScrollableControl();
            scrollableControl.Dock = DockStyle.Bottom;
            scrollableControl.AutoScroll = true;
            scrollableControl.Height = this.ClientSize.Height - button1.Height;
            this.Controls.Add(scrollableControl);

            // Add the panel to the scrollable control
            this.panel = new Panel();
            this.panel.Width = this.Width;
            this.panel.Margin = new Padding(PANEL_MARGIN);
            scrollableControl.Controls.Add(this.panel);

            // Set the location of the panel and button
            this.panel.Location = new Point((this.ClientSize.Width - this.panel.Width) / 2, button1.Bottom + PANEL_MARGIN);
            button1.Top = this.panel.Bottom + PANEL_MARGIN;

            // Load any previously saved images
            string imageDir = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            DirectoryInfo di = new DirectoryInfo(imageDir);
            FileInfo[] imageFiles = di.GetFiles("*.png");
            foreach (FileInfo file in imageFiles)
            {
                Image image = Image.FromFile(file.FullName);
                images.Add(image);
            }
            display_images();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the image from the clipboard
                Image image = Clipboard.GetImage();

                // Set the default file name and location
                string fileName = "image_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", fileName);

                // Create the directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                // Save the image to the file
                image.Save(filePath);

                // Add the image to the list
                images.Insert(0, image);

                // Display the images
                display_images();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void display_images()
        {
            // Clear the panel
            this.panel.Controls.Clear();

            // Add the images to the panel
            int y = 0;
            foreach (Image image in images)
            {
                PictureBox pictureBox = new PictureBox();
                pictureBox.Image = image;
                pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
                pictureBox.Location = new Point(0, y);
                this.panel.Controls.Add(pictureBox);
                y += pictureBox.Height + PANEL_MARGIN;
            }

            // Resize the panel to fit the images
            this.panel.Height = y;
        }
    }
}
