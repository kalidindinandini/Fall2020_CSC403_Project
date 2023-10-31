using Fall2020_CSC403_Project.code;
using Fall2020_CSC403_Project.Properties;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Media;

namespace Fall2020_CSC403_Project
{
    public partial class FrmLevel : Form
    {
        private Player player;

        private Enemy enemyPoisonPacket;
        private Enemy bossKoolaid;
        private Enemy enemyCheeto;
        private Character[] walls;

        private DateTime timeBegin;
        private FrmBattle frmBattle;
        // Button for Quitting the Game.
        private Button quitButton;
        // Button for Restarting the Game.
        private Button restartButton;
        // Button for the Main Menu.
        private Panel mainMenuPanel;
        // Buuton for New Game option in the main menu to start the game.
        private Button newGameButton;
        // Buttons for the Play/Pause of the game.
        private Button buttonForPlayPause;
        private bool gameIsPaused = false;
        // Button for the UserFAQ's in the Main Menu.
        private Button buttonForUserFAQ;


        // Start of the game code.
        public FrmLevel()
        {
            InitializeComponent();
            //InitializeTheQuitButton();
            //InitializeRestartButton();
            this.KeyPreview = true;
            // Initialize the game in 'menu state'
            SetupMainMenu();
            InitializeGameComponents(false); // Add this method
        }

        // Code to Initialize the Restart Button in the FrmLevel Code.


        private void InitializeGameComponents(bool visible)
        {
            foreach (Control control in this.Controls)
            {
                if (control != mainMenuPanel)
                {
                    control.Visible = visible;
                }
            }
        }

        private void SetupMainMenu()
        {
            // Spacing and sizing
            int buttonWidth = 180;
            int buttonHeight = 30;
            int spacing = 10;
            int headingHeight = 20; // Height for the heading label
            int extraPadding = 50; // Extra space for aesthetics or other controls
            int numButtons = 4; // Total number of buttons

            // Dynamic height calculation
            int panelHeight = (buttonHeight + spacing) * numButtons + headingHeight + spacing + extraPadding;

            mainMenuPanel = new Panel()
            {
                Size = new Size(200, panelHeight),
                Location = new Point((this.ClientSize.Width - 200) / 2, (this.ClientSize.Height - panelHeight) / 2),
                BackColor = Color.Black
            };
            this.Controls.Add(mainMenuPanel);

            // Label for Main Menu
            Label lblMainMenu = new Label()
            {
                Text = "Main Menu",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Size = new Size(180, headingHeight),
                Location = new Point(10, 10), // Position at the top
                TextAlign = ContentAlignment.MiddleCenter
            };
            mainMenuPanel.Controls.Add(lblMainMenu);

            // Calculate the starting Y position for the buttons
            int startY = lblMainMenu.Bottom + spacing;

            // New Game Button
            newGameButton = new Button()
            {
                Text = "New Game",
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(10, startY),
                ForeColor = Color.White,
                BackColor = Color.Black
            };
            newGameButton.Click += NewGameButton_Click;
            mainMenuPanel.Controls.Add(newGameButton);

            // Quit Button
            startY += buttonHeight + spacing; // Update startY for next button
            quitButton = new Button()
            {
                Text = "Quit",
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(10, startY),
                ForeColor = Color.White,
                BackColor = Color.Black
            };
            quitButton.Click += (sender, e) => Application.Exit();
            mainMenuPanel.Controls.Add(quitButton);

            // User FAQ's Button
            startY += buttonHeight + spacing; // Update startY for next button
            buttonForUserFAQ = new Button()
            {
                Text = "User FAQ's",
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(10, startY),
                ForeColor = Color.White,
                BackColor = Color.Black
            };
            buttonForUserFAQ.Click += (sender, e) => showTheFAQForm();
            mainMenuPanel.Controls.Add(buttonForUserFAQ);
        }


        private void NewGameButton_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private void StartNewGame()
        {
            mainMenuPanel.Visible = false;
            InitializeGameComponents(true); // Show the game components
            InitializeRestartButton();
            InitializeButtonForPlayPause();
            //restartButton.Visible = true;
        }

        private void InitializeRestartButton()
        {
            restartButton = new Button();
            restartButton.Text = "Restart";
            restartButton.Click += RestartButtonClick;
            restartButton.TabStop = false;
            restartButton.Size = new Size(60, 30);

            // Initially, position the button at the top right corner.
            restartButton.Location = new Point(this.ClientSize.Width - restartButton.Width - 10, 10);

            // Add the control to the form.
            this.Controls.Add(restartButton);

            // Initially, the button is not visible.
            restartButton.Visible = true;
            restartButton.BringToFront();
        }

        // The function for performing the restart of the game.
        private void RestartButtonClick(object sender, EventArgs e)
        {
            Application.Restart();
            Environment.Exit(0);
        }

        // The Intialization function for adding the play/pause button for the game.
        private void InitializeButtonForPlayPause()
        {
            buttonForPlayPause = new Button();
            buttonForPlayPause.Size = new Size(60, 30);
            buttonForPlayPause.Text = "Pause";
            buttonForPlayPause.Location = new Point(restartButton.Location.X - buttonForPlayPause.Width - 5, restartButton.Location.Y);
            buttonForPlayPause.Click += clickButtonForPlayPause;
            this.Controls.Add(buttonForPlayPause);
            buttonForPlayPause.BringToFront();
        }
        private void clickButtonForPlayPause(object sender, EventArgs e)
        {
            gameIsPaused = !gameIsPaused;
            buttonForPlayPause.Text = gameIsPaused ? "Play" : "Pause";
            if (gameIsPaused)
            {
                tmrPlayerMove.Stop();
                tmrUpdateInGameTime.Stop();
            }
            else
            {
                tmrPlayerMove.Start();
                tmrUpdateInGameTime.Start();
            }
        }

        private void showTheFAQForm()
        {
            Form faqForm = new Form()
            {
                Size = new Size(300, 200),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                Text = "FAQ"
            };

            // Button for Characters FAQ
            Button btnCharacters = new Button()
            {
                Text = "Characters",
                Size = new Size(100, 30),
                Location = new Point(10, 10)
            };
            btnCharacters.Click += (sender, e) => ShowFaqDetails("Characters");
            faqForm.Controls.Add(btnCharacters);

            // Button for Instructions FAQ
            Button btnInstructions = new Button()
            {
                Text = "Instructions",
                Size = new Size(100, 30),
                Location = new Point(10, 50)
            };
            btnInstructions.Click += (sender, e) => ShowFaqDetails("Instructions");
            faqForm.Controls.Add(btnInstructions);

            // Button for Abilities FAQ
            Button btnAbilities = new Button()
            {
                Text = "Abilities",
                Size = new Size(100, 30),
                Location = new Point(10, 90)
            };
            btnAbilities.Click += (sender, e) => ShowFaqDetails("Abilities");
            faqForm.Controls.Add(btnAbilities);

            faqForm.ShowDialog();
        }

        private void ShowFaqDetails(string faqType)
        {
            string contentForFAQ = "";

            switch (faqType)
            {
                case "Characters":
                    contentForFAQ = "Q: Are there any other characters that are available so I can select it to play in the game?\n" +
                                 "A: Sorry. Right now we only have one character. We will be getting more characters in future.\n\n" +
                                 "Q: Is KoolAid Man the opponent I need to fight last?\n" +
                                 "A: Yes, He is the boss so you need to fight him at last.";
                    break;

                case "Instructions":
                    contentForFAQ = "Q: WHat should i do to move the player?\n" +
                                 "A: You can just press the arrow keys in the keyboard for the movement of the player..\n\n" +
                                 "Q: Can the Enemies move?\n" +
                                 "A: No the enemies cannot move.";
                    break;

                case "Abilities":
                    contentForFAQ = "Q: What is the Flee Ability?\n" +
                                 "A: The flee ability allows you to flee the battle with the enemy when you have less health points.\n\n" +
                                 "Q: What is the Heal Ability?\n" +
                                 "A: When you have less health points than the enemy, yopu can click on the Heal button to make your health points as full..";
                    break;

                default:
                    contentForFAQ = "No FAQ available for this category.";
                    break;
            }

            MessageBox.Show(contentForFAQ, $"{faqType} FAQs");
        }


        private void FrmLevel_Load(object sender, EventArgs e)
        {
            const int PADDING = 7;
            const int NUM_WALLS = 13;

            player = new Player(CreatePosition(picPlayer), CreateCollider(picPlayer, PADDING));
            bossKoolaid = new Enemy(CreatePosition(picBossKoolAid), CreateCollider(picBossKoolAid, PADDING));
            enemyPoisonPacket = new Enemy(CreatePosition(picEnemyPoisonPacket), CreateCollider(picEnemyPoisonPacket, PADDING));
            enemyCheeto = new Enemy(CreatePosition(picEnemyCheeto), CreateCollider(picEnemyCheeto, PADDING));

            bossKoolaid.Img = picBossKoolAid.BackgroundImage;
            enemyPoisonPacket.Img = picEnemyPoisonPacket.BackgroundImage;
            enemyCheeto.Img = picEnemyCheeto.BackgroundImage;

            bossKoolaid.Color = Color.Red;
            enemyPoisonPacket.Color = Color.Green;
            enemyCheeto.Color = Color.FromArgb(255, 245, 161);

            walls = new Character[NUM_WALLS];
            for (int w = 0; w < NUM_WALLS; w++)
            {
                PictureBox pic = Controls.Find("picWall" + w.ToString(), true)[0] as PictureBox;
                walls[w] = new Character(CreatePosition(pic), CreateCollider(pic, PADDING));
            }

            Game.player = player;
            timeBegin = DateTime.Now;


        }



        private Vector2 CreatePosition(PictureBox pic)
        {
            return new Vector2(pic.Location.X, pic.Location.Y);
        }

        private Collider CreateCollider(PictureBox pic, int padding)
        {
            Rectangle rect = new Rectangle(pic.Location, new Size(pic.Size.Width - padding, pic.Size.Height - padding));
            return new Collider(rect);
        }

        private void tmrUpdateInGameTime_Tick(object sender, EventArgs e)
        {
            TimeSpan span = DateTime.Now - timeBegin;
            string time = span.ToString(@"hh\:mm\:ss");
            lblInGameTime.Text = "Time: " + time.ToString();
        }

        private void tmrPlayerMove_Tick(object sender, EventArgs e)
        {
            // move player
            player.Move();

            // check collision with walls
            if (HitAWall(player))
            {
                player.MoveBack();
            }

            // check collision with enemies
            if (HitAChar(player, enemyPoisonPacket))
            {
                Fight(enemyPoisonPacket);
            }
            else if (HitAChar(player, enemyCheeto))
            {
                Fight(enemyCheeto);
            }
            if (HitAChar(player, bossKoolaid))
            {
                Fight(bossKoolaid);
            }

            // update player's picture box
            picPlayer.Location = new Point((int)player.Position.x, (int)player.Position.y);
        }



        private bool HitAWall(Character c)
        {
            bool hitAWall = false;
            for (int w = 0; w < walls.Length; w++)
            {
                if (c.Collider.Intersects(walls[w].Collider))
                {
                    hitAWall = true;
                    break;
                }
            }
            return hitAWall;
        }

        private bool HitAChar(Character you, Character other)
        {
            return you.Collider.Intersects(other.Collider);
        }

        private void Fight(Enemy enemy)
        {
            player.ResetMoveSpeed();
            player.MoveBack();
            frmBattle = FrmBattle.GetInstance(enemy);
            frmBattle.Show();

            if (enemy == bossKoolaid)
            {
                frmBattle.SetupForBossBattle();

            }
            RemoveEnemy(enemy);
        }
        private void RemoveEnemy(Enemy enemy)
        {
            if (enemy == enemyPoisonPacket)
            {
                picEnemyPoisonPacket.Visible = false;
            }
            else if (enemy == bossKoolaid && enemy.Health == 0)
            {
                picBossKoolAid.Visible = false;
            }
            else if (enemy == enemyCheeto)
            {
                picEnemyCheeto.Visible = false;
            }

        }



        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.ActiveControl = null;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Handle player movement through ProcessCmdKey
            switch (keyData)
            {
                case Keys.Left:
                    player.GoLeft();
                    break;
                case Keys.Right:
                    player.GoRight();
                    break;
                case Keys.Up:
                    player.GoUp();
                    break;
                case Keys.Down:
                    player.GoDown();
                    break;
                default:
                    return base.ProcessCmdKey(ref msg, keyData); // Important to call the base implementation for other keys
            }

            return true; // Indicates that you've handled the key press
        }


        private void FrmLevel_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    player.GoLeft();
                    break;

                case Keys.Right:
                    player.GoRight();
                    break;

                case Keys.Up:
                    player.GoUp();
                    break;

                case Keys.Down:
                    player.GoDown();
                    break;

                default:
                    player.ResetMoveSpeed();
                    break;
            }
        }
        private void FrmLevel_KeyUp(object sender, KeyEventArgs e)
        {
            player.ResetMoveSpeed();
        }

        private void lblInGameTime_Click(object sender, EventArgs e)
        {

        }



    }
}
