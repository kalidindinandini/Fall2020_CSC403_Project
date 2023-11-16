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
        //game story button
        private Button gameStoryButton;

        string name;
        // Constructor for FrmLevel, initializes game components and sets up the main menu.
        public FrmLevel()
        {
            InitializeComponent();
            //InitializeTheQuitButton();
            //InitializeRestartButton();
            this.KeyPreview = true;
            // Initialize the game in 'menu state'
            SetupMainMenu();
            InitializeGameComponents(false);
        }

        // Initializes game components and sets their visibility.


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
        // Sets up the main menu including buttons and their event handlers.
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

            // Story panel button
            startY += buttonHeight + spacing;
            gameStoryButton = new Button()
            {
                Text = "Game Story",
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(10, startY),
                ForeColor = Color.White,
                BackColor = Color.Black
            };
            gameStoryButton.Click += gameStoryButton_Click;
            mainMenuPanel.Controls.Add(gameStoryButton);
        }

        // Event handler for the Game Story button click, makes the story group box visible.
        private void gameStoryButton_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = true;
            groupBox1.Location = new Point(341, 118);
        }
        // Event handler for starting a new game, initializes game components and starts the game.
        private void NewGameButton_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }

        // Starts a new game by hiding the main menu and showing game components.
        private void StartNewGame()
        {
            mainMenuPanel.Visible = false;
            InitializeGameComponents(true); // Show the game components
            InitializeRestartButton();
            InitializeButtonForPlayPause();
            //restartButton.Visible = true;

        }

        // Initializes the Restart button and its event handler.
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

        // Event handler for the Play/Pause button, toggles game pause state.
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

        // Shows the FAQ form with options for different FAQ categories.
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
        // Displays FAQ details based on the selected category.
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

        // Form Load event handler, initializes player, enemies, walls, and game timer.
        private void FrmLevel_Load(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
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


        // Creates a Vector2 position from a PictureBox control.
        private Vector2 CreatePosition(PictureBox pic)
        {
            return new Vector2(pic.Location.X, pic.Location.Y);
        }

        // Creates a collider for a PictureBox control with specified padding.
        private Collider CreateCollider(PictureBox pic, int padding)
        {
            Rectangle rect = new Rectangle(pic.Location, new Size(pic.Size.Width - padding, pic.Size.Height - padding));
            return new Collider(rect);
        }

        // Timer event handler for updating in-game time display.
        private void tmrUpdateInGameTime_Tick(object sender, EventArgs e)
        {
            TimeSpan span = DateTime.Now - timeBegin;
            string time = span.ToString(@"hh\:mm\:ss");
            lblInGameTime.Text = "Time: " + time.ToString();
        }

        // Timer event handler for moving the player and handling collisions.
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
                if (picEnemyPoisonPacket.Visible == true)
                {
                    enemyPoisonPacket.Img = picEnemyPoisonPacket.BackgroundImage;
                    Fight(enemyPoisonPacket);
                }

            }
            else if (HitAChar(player, enemyCheeto))
            {
                if (picEnemyCheeto.Visible == true)
                {
                    enemyCheeto.Img = picEnemyCheeto.BackgroundImage;
                    Fight(enemyCheeto);
                }

            }
            if (HitAChar(player, bossKoolaid))
            {
                if (picBossKoolAid.Visible == true)
                {
                    bossKoolaid.Img = picBossKoolAid.BackgroundImage;
                    Fight(bossKoolaid);
                }

            }

            // update player's picture box
            picPlayer.Location = new Point((int)player.Position.x, (int)player.Position.y);
        }


        // Checks if the character has hit a wall.
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

        // Checks if two characters have collided.
        private bool HitAChar(Character you, Character other)
        {
            return you.Collider.Intersects(other.Collider);
        }

        // Handles the logic for initiating a fight with an enemy.
        private void Fight(Enemy enemy)
        {


            if (enemy == bossKoolaid)
            {
                name = "bosskoolaid";
                enemy.Img = picBossKoolAid.BackgroundImage;
                frmBattle.SetupForBossBattle();

            }
            else if (enemy == enemyCheeto)
            {
                name = "enemycheeto";
                enemy.Img = picEnemyCheeto.BackgroundImage;

            }
            else if (enemy == enemyPoisonPacket)
            {
                name = "enemyPoisonPacket";
                enemy.Img = picEnemyPoisonPacket.BackgroundImage;
            }

            player.ResetMoveSpeed();
            player.MoveBack();
            frmBattle = FrmBattle.GetInstance(enemy, player, name);
            frmBattle.Show();




            //updating player health
            int health = player.Health - 8;

            float playerHealthPer = health / (float)player.MaxHealth;
            const int MAX_HEALTHBAR_WIDTH = 226;
            lblPlayerHealth.Width = (int)(MAX_HEALTHBAR_WIDTH * playerHealthPer);
            lblPlayerHealth.Text = health.ToString();

            //RemoveEnemy(enemy);

        }

        // Removes a defeated enemy from the game.		
        private void RemoveEnemy(Enemy enemy)
        {
            if (enemy == enemyPoisonPacket)
            {
                picEnemyPoisonPacket.Visible = false;
                btn1.Visible = false;

            }
            else if (enemy == bossKoolaid)
            {
                picBossKoolAid.Visible = false;
                button6.Visible = false;

            }
            else if (enemy == enemyCheeto)
            {
                picEnemyCheeto.Visible = false;
                button14.Visible = false;
            }

        }


        // Overrides the OnLoad event to set the active control to null.
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.ActiveControl = null;
        }

        // Overrides ProcessCmdKey to handle player movement controls.
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

        // KeyDown event handler for player movement.
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
        // KeyUp event handler to reset player movement speed.
        private void FrmLevel_KeyUp(object sender, KeyEventArgs e)
        {
            player.ResetMoveSpeed();
        }

        private void lblInGameTime_Click(object sender, EventArgs e)
        {

        }

        private void txtContent_TextChanged(object sender, EventArgs e)
        {

        }

        // Event handler for entering groupBox1. Hides the groupBox.
        private void groupBox1_Enter(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
        }

        // Event handler for button1 click. Moves and hides groupBox1.
        private void button1_Click(object sender, EventArgs e)
        {
            groupBox1.Location = new Point(632, 716);
            groupBox1.Visible = false;

        }

        // Event handler for button3 click. Changes the background image of all wall PictureBoxes to 'wall3'.
        private void button3_Click(object sender, EventArgs e)
        {
            picWall0.BackgroundImage = Resources.wall3;
            picWall1.BackgroundImage = Resources.wall3;
            picWall2.BackgroundImage = Resources.wall3;
            picWall3.BackgroundImage = Resources.wall3;
            picWall4.BackgroundImage = Resources.wall3;
            picWall5.BackgroundImage = Resources.wall3;
            picWall6.BackgroundImage = Resources.wall3;
            picWall7.BackgroundImage = Resources.wall3;
            picWall8.BackgroundImage = Resources.wall3;
            picWall9.BackgroundImage = Resources.wall3;
            picWall10.BackgroundImage = Resources.wall3;
            picWall11.BackgroundImage = Resources.wall3;
            picWall12.BackgroundImage = Resources.wall3;


        }

        // Event handler for button5 click. Changes the background image of all wall PictureBoxes to the default 'wall'.
        private void button5_Click(object sender, EventArgs e)
        {
            picWall0.BackgroundImage = Resources.wall;
            picWall1.BackgroundImage = Resources.wall;
            picWall2.BackgroundImage = Resources.wall;
            picWall3.BackgroundImage = Resources.wall;
            picWall4.BackgroundImage = Resources.wall;
            picWall5.BackgroundImage = Resources.wall;
            picWall6.BackgroundImage = Resources.wall;
            picWall7.BackgroundImage = Resources.wall;
            picWall8.BackgroundImage = Resources.wall;
            picWall9.BackgroundImage = Resources.wall;
            picWall10.BackgroundImage = Resources.wall;
            picWall11.BackgroundImage = Resources.wall;
            picWall12.BackgroundImage = Resources.wall;
        }

        // Event handler for button2 click. Changes the background image of all wall PictureBoxes to 'wall2'.
        private void button2_Click(object sender, EventArgs e)
        {
            picWall0.BackgroundImage = Resources.wall2;
            picWall1.BackgroundImage = Resources.wall2;
            picWall2.BackgroundImage = Resources.wall2;
            picWall3.BackgroundImage = Resources.wall2;
            picWall4.BackgroundImage = Resources.wall2;
            picWall5.BackgroundImage = Resources.wall2;
            picWall6.BackgroundImage = Resources.wall2;
            picWall7.BackgroundImage = Resources.wall2;
            picWall8.BackgroundImage = Resources.wall2;
            picWall9.BackgroundImage = Resources.wall2;
            picWall10.BackgroundImage = Resources.wall2;
            picWall11.BackgroundImage = Resources.wall2;
            picWall12.BackgroundImage = Resources.wall2;
        }

        // Event handler for button4 click. Changes the background image of all wall PictureBoxes to 'wall4'.        
        private void button4_Click(object sender, EventArgs e)
        {
            picWall0.BackgroundImage = Resources.wall4;
            picWall1.BackgroundImage = Resources.wall4;
            picWall2.BackgroundImage = Resources.wall4;
            picWall3.BackgroundImage = Resources.wall4;
            picWall4.BackgroundImage = Resources.wall4;
            picWall5.BackgroundImage = Resources.wall4;
            picWall6.BackgroundImage = Resources.wall4;
            picWall7.BackgroundImage = Resources.wall4;
            picWall8.BackgroundImage = Resources.wall4;
            picWall9.BackgroundImage = Resources.wall4;
            picWall10.BackgroundImage = Resources.wall4;
            picWall11.BackgroundImage = Resources.wall4;
            picWall12.BackgroundImage = Resources.wall4;
        }

        // Event handler for the second button2 click (duplicated button). Restarts the application.
        private void button2_Click_1(object sender, EventArgs e)
        {
            Application.Restart();
            Environment.Exit(0);

        }

        string skinChange;

        // Event handler for button15 click. Moves groupBox3 to a specific location on the screen.
        private void button15_Click(object sender, EventArgs e)
        {
            groupBox3.Location = new Point(1257, 298);

        }

        // Event handler for button6 click. Sets the skin change target to 'boss' and moves groupBox3.
        private void button6_Click(object sender, EventArgs e)
        {
            skinChange = "boss";
            groupBox3.Location = new Point(887, 98);

        }

        // Event handler for button13 click. Sets the skin change target to 'poison' and moves groupBox3.
        private void button13_Click(object sender, EventArgs e)
        {
            skinChange = "poison";
            groupBox3.Location = new Point(194, 74);

        }

        // Event handler for button14 click. Sets the skin change target to 'cheeto' and moves groupBox3.
        private void button14_Click(object sender, EventArgs e)
        {
            skinChange = "cheeto";
            groupBox3.Location = new Point(887, 243);

        }

        private void picBossKoolAid_Click(object sender, EventArgs e)
        {

        }

        private void picEnemyCheeto_Click(object sender, EventArgs e)
        {

        }

        // Event handler for button7 click. Changes the skin of the selected enemy to 'enemy_poisonpacket_fw'.
        private void button7_Click(object sender, EventArgs e)
        {
            if (skinChange == "boss")
            {
                picBossKoolAid.BackgroundImage = Resources.enemy_poisonpacket_fw;
            }
            else if (skinChange == "cheeto")
            {
                picEnemyCheeto.BackgroundImage = Resources.enemy_poisonpacket_fw;
            }
            else if (skinChange == "poison")
            {
                picEnemyPoisonPacket.BackgroundImage = Resources.enemy_poisonpacket_fw;
            }
        }

        // Event handler for button8 click. Changes the skin of the selected enemy to 'enemy_koolaid'.
        private void button8_Click(object sender, EventArgs e)
        {
            if (skinChange == "boss")
            {
                picBossKoolAid.BackgroundImage = Resources.enemy_koolaid;
            }
            else if (skinChange == "cheeto")
            {
                picEnemyCheeto.BackgroundImage = Resources.enemy_koolaid;
            }
            else if (skinChange == "poison")
            {
                picEnemyPoisonPacket.BackgroundImage = Resources.enemy_koolaid;
            }
        }

        // Event handler for button9 click. Changes the skin of the selected enemy to 'enemy_cheetos_fw'.
        private void button9_Click(object sender, EventArgs e)
        {
            if (skinChange == "boss")
            {
                picBossKoolAid.BackgroundImage = Resources.enemy_cheetos_fw;
            }
            else if (skinChange == "cheeto")
            {
                picEnemyCheeto.BackgroundImage = Resources.enemy_cheetos_fw;
            }
            else if (skinChange == "poison")
            {
                picEnemyPoisonPacket.BackgroundImage = Resources.enemy_cheetos_fw;
            }
        }

        // Event handler for button10 click. Changes the skin of the selected enemy to 'batvillain'.
        private void button10_Click(object sender, EventArgs e)
        {
            if (skinChange == "boss")
            {
                picBossKoolAid.BackgroundImage = Resources.batvillain;
            }
            else if (skinChange == "cheeto")
            {
                picEnemyCheeto.BackgroundImage = Resources.batvillain;
            }
            else if ((skinChange == "poison"))
            {
                picEnemyPoisonPacket.BackgroundImage = Resources.batvillain;
            }
        }

        // Event handler for button11 click. Changes the skin of the selected enemy to 'evilman'.
        private void button11_Click(object sender, EventArgs e)
        {
            if (skinChange == "boss")
            {
                picBossKoolAid.BackgroundImage = Resources.evilman;
            }
            else if (skinChange == "cheeto")
            {
                picEnemyCheeto.BackgroundImage = Resources.evilman;
            }
            else if (skinChange == "poison")
            {
                picEnemyPoisonPacket.BackgroundImage = Resources.evilman;
            }
        }

        // Event handler for button12 click. Changes the skin of the selected enemy to 'mojo'.
        private void button12_Click(object sender, EventArgs e)
        {
            if (skinChange == "boss")
            {
                picBossKoolAid.BackgroundImage = Resources.mojo;
            }
            else if (skinChange == "cheeto")
            {
                picEnemyCheeto.BackgroundImage = Resources.mojo;
            }
            else if ((skinChange == "poison"))
            {
                picEnemyPoisonPacket.BackgroundImage = Resources.mojo;
            }
        }

        private void lblPlayerHealthFull_Click(object sender, EventArgs e)
        {

        }

        // Event handler for button13 click. Displays a message about increased player abilities.
        private void button13_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Congratulations, you now have increased attack power, improved agility, and the ability to break through obstacles. \n\n Kidding! If you actually want to use the orb, please purchase the full version of the game from Amazon. The full version is only for 99$." );
        }

        // Event handler for button16 click. Displays a message about a new protective shield ability.
        private void button16_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Congratulations, you now have a protective shield against enemy attacks.  \n\n Kidding! If you actually want to use the shield, please purchase the full version of the game from Amazon. The full version is only for 99$.");
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
