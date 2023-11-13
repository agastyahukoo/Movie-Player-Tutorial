using AxWMPLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace WindowsFormsApp15
{
    public partial class MainForm : Form
    {
        private List<string> movieNames = new List<string>();
        private PictureBox selectedPictureBox;
        private bool isDragging = false;
        private Point lastCursor;
        private Point lastForm;
        public MainForm()
        {
            InitializeComponent();
            InitializeFormAppearance();
            LoadMovieNames();
            DisplayThumbnails();
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            this.MouseDown += MainForm_MouseDown;
            this.MouseUp += MainForm_MouseUp;
            this.MouseMove += MainForm_MouseMove;
        }
        private void InitializeFormAppearance()
        {
            this.Size = new Size(800, 600);
            this.Text = "My Movie Player";
            this.BackColor = Color.FromArgb(20, 20, 20);

            Label infoLabel = new Label();
            infoLabel.Text = "Spider-Man: Across the Spider-Verse";
            infoLabel.ForeColor = Color.White;
            infoLabel.Location = new Point(10, 20);
            infoLabel.Font = new Font("Arial", 10, FontStyle.Bold);
            infoLabel.AutoSize = true;
            infoLabel.TextAlign = ContentAlignment.TopLeft;
            this.Controls.Add(infoLabel);
        }

        private void Player_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == (int)WMPLib.WMPPlayState.wmppsMediaEnded)
            {
                MoviePlayer.Ctlcontrols.stop();
                MoviePlayer.Visible = false;
                selectedPictureBox.Visible = true;
            }
        }

        private void PlayMovie(string moviePath, PictureBox clickedPictureBox)
        {
            try
            {
                selectedPictureBox = clickedPictureBox;
                MoviePlayer.URL = moviePath;
                MoviePlayer.uiMode = "none";

                MoviePlayer.Visible = true;
                selectedPictureBox.Visible = false;

                MoviePlayer.Ctlcontrols.play();
                MoviePlayer.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(Player_PlayStateChange);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error playing movie: " + ex.Message);
            }
        }

        private void LoadMovieNames()
        {
            try
            {
                using (StreamReader sr = new StreamReader("movieList.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        movieNames.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading movie names: " + ex.Message);
            }
        }

        private void DisplayThumbnails()
        {
            const int pictureBoxWidth = 150;
            const int pictureBoxHeight = 225;
            int xPos = 10;
            foreach (string movieName in movieNames)
            {
                PictureBox pictureBox = new PictureBox();
                pictureBox.Location = new Point(xPos, 50);
                pictureBox.Size = new Size(pictureBoxWidth, pictureBoxHeight);
                pictureBox.Image = Image.FromFile($".\\images\\{movieName}.png");
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

                pictureBox.MouseEnter += (sender, e) =>
                {
                    pictureBox.Size = new Size((int)(pictureBoxWidth * 1.1), (int)(pictureBoxHeight * 1.1));
                };
                pictureBox.MouseLeave += (sender, e) =>
                {
                    pictureBox.Size = new Size(pictureBoxWidth, pictureBoxHeight);
                };

                pictureBox.Click += (sender, e) =>
                {
                    string clickedMovieName = ((PictureBox)sender).Name;
                    string moviePath = $".\\movies\\{clickedMovieName}.mp4";
                    PlayMovie(moviePath, (PictureBox)sender);
                };
                pictureBox.Name = movieName;

                this.Controls.Add(pictureBox);
                xPos += pictureBoxWidth + 10;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (MoviePlayer.Visible)
            {
                if (e.KeyCode == Keys.Space)
                {
                    if (MoviePlayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
                    {
                        MoviePlayer.Ctlcontrols.pause();
                    }
                    else if (MoviePlayer.playState == WMPLib.WMPPlayState.wmppsPaused)
                    {
                        MoviePlayer.Ctlcontrols.play();
                    }
                }
                else if (e.KeyCode == Keys.Left)
                {
                    MoviePlayer.Ctlcontrols.currentPosition -= 5;
                }
                else if (e.KeyCode == Keys.Right)
                {
                    MoviePlayer.Ctlcontrols.currentPosition += 5;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    MoviePlayer.Ctlcontrols.pause();
                    MoviePlayer.Visible = false;

                    foreach (Control control in Controls)
                    {
                        if (control is PictureBox)
                        {
                            control.Visible = true;
                        }
                    }
                }
            }
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            lastCursor = Cursor.Position;
            lastForm = this.Location;
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentCursor = Cursor.Position;
                this.Location = new Point(
                    lastForm.X + (currentCursor.X - lastCursor.X),
                    lastForm.Y + (currentCursor.Y - lastCursor.Y));
            }
        }
    }
}