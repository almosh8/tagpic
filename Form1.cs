using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GlobalHook.Core;
using Gma.System.MouseKeyHook;

namespace tagpic
{
    public partial class Form1 : Form
    {
        private const int BUTTON_HEIGHT = 30;
        private const int PANEL_WIDTH = 600;
        private const int PANEL_MARGIN = 10;

        private List<Image> images = new List<Image>();
        private System.Windows.Forms.Panel panel;
        private bool isAddingImage = false;

        private IKeyboardMouseEvents m_GlobalHook;

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
                    string filePath = form.Tag as string;

                    // Load the image from the file and add it to the list
                    Image savedImage = form.image;
                    this.images.Insert(0, savedImage);
                    // Display the images
                    display_images();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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
            foreach (FileInfo file in imageFiles)
            {
                Image image = Image.FromFile(file.FullName);
                images.Add(image);
            }
            display_images();
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
