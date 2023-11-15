using Fall2020_CSC403_Project.code;
using Fall2020_CSC403_Project.Properties;
using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project
{
    public partial class FrmBattle : Form
    {
        public static FrmBattle instance = null;
        private Enemy enemy;
        private Player player;
        //Adding the Button for Flee Ability.
        private Button fleeButton;
        // Adding a button for the Heal Ability.
        private Button healButton;
        public static string charname;
        // Constructor for the FrmBattle class. Initializes the form components, the Flee and Heal buttons, and sets the player instance.  
        private FrmBattle()
        {
            InitializeComponent();
            InitializeButtonForFlee();
            InitializeButtonForHealing();

            player = Game.player;
        }

        // Creates and configures the Flee button, positioning it next to the Attack button.

        private void InitializeButtonForFlee()
        {
            this.fleeButton = new System.Windows.Forms.Button();

            //btnAttack is the attack button, position btnFlee beside it
            int spacing = 10;
            this.fleeButton.Location = new System.Drawing.Point(
                btnAttack.Location.X + btnAttack.Size.Width + spacing,
                btnAttack.Location.Y
            );
            this.fleeButton.Size = new System.Drawing.Size(75, btnAttack.Size.Height); // Make height same as attack button
            this.fleeButton.Text = "Flee";
            this.fleeButton.UseVisualStyleBackColor = true;
            this.fleeButton.Click += new System.EventHandler(this.clickFleeButton);

            this.Controls.Add(this.fleeButton);
        }

        // Event handler for the Flee button click. Attempts to flee from battle.	
        private void clickFleeButton(object sender, EventArgs e)
        {
            tryingToFlee();
        }

        // Logic to determine if the player can flee from the enemy based on health points.	
        public bool tryingToFlee()
        {
            if (enemy.Health > player.Health)
            {
                MessageBox.Show("You are now fleeing from the Opponent.");
                instance = null;
                Close();
                return true;
            }
            else
            {
                MessageBox.Show("You cant flee. You have more health points than the opponent.");
                return false;
            }
        }

        // Creates and configures the Heal button, positioning it next to the Flee button.
        private void InitializeButtonForHealing()
        {
            this.healButton = new System.Windows.Forms.Button();
            int spacing = 10;
            this.healButton.Location = new System.Drawing.Point(fleeButton.Location.X + fleeButton.Size.Width + spacing, fleeButton.Location.Y);
            this.healButton.Size = new System.Drawing.Size(75, fleeButton.Size.Height);
            this.healButton.UseVisualStyleBackColor = true;
            this.healButton.Text = "Heal";
            this.healButton.Click += new System.EventHandler(this.clickHealButton);
            this.Controls.Add(this.healButton);
        }
        // Event handler for the Heal button click. Initiates the player healing process.
        private void clickHealButton(object sender, EventArgs e)
        {
            PlayerHealing();
            UpdateHealthBars();
        }
        // Handles the player's healing logic, ensuring health does not exceed the maximum.
        private void PlayerHealing()
        {
            int totalHealAmount = 20;
            int healthDeficit = player.MaxHealth - player.Health; // Calculate how much health is missing to reach max

            if (player.Health + totalHealAmount >= player.MaxHealth)
            {
                player.AlterHealth(healthDeficit); // Heal up to the maximum health

                if (healthDeficit == 0)
                {
                    MessageBox.Show("You already have full Health. You cannot increase it more.", "Health Full", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("You are healed now. You have full Health. All the best for the battle ahead of you!", "Healed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                player.AlterHealth(totalHealAmount); // Heal the player by the total heal amount
                MessageBox.Show($"You are healed by {totalHealAmount} points.", "Healing", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Sets up the battle scene with enemy specifics. Configures event handlers for attack events.
        public void Setup()
        {
            // update for this enemy
            picEnemy.BackgroundImage = enemy.Img;
            picEnemy.Refresh();
            BackColor = enemy.Color;
            picBossBattle.Visible = false;

            // Observer pattern
            enemy.AttackEvent += PlayerDamage;
            player.AttackEvent += EnemyDamage;

            // show health
            UpdateHealthBars();
        }

        // Configures the form for a boss battle, including playing a sound and enabling a timer.
        public void SetupForBossBattle()
        {
            picBossBattle.Location = Point.Empty;
            picBossBattle.Size = ClientSize;
            picBossBattle.Visible = true;

            SoundPlayer simpleSound = new SoundPlayer(Resources.final_battle);
            simpleSound.Play();

            tmrFinalBattle.Enabled = true;
        }
        // Static method to create a singleton instance of FrmBattle. Sets up the battle environment.
        public static FrmBattle GetInstance(Enemy enemy, Player player, string name)
        {

            if (instance == null)
            {
                charname = name;
                instance = new FrmBattle();
                instance.enemy = enemy;

                instance.Setup();

            }
            return instance;
        }
        // Updates the health bars for both player and enemy based on their current health.
        private void UpdateHealthBars()
        {
            float playerHealthPer = player.Health / (float)player.MaxHealth;
            float enemyHealthPer = enemy.Health / (float)enemy.MaxHealth;

            const int MAX_HEALTHBAR_WIDTH = 226;
            lblPlayerHealthFull.Width = (int)(MAX_HEALTHBAR_WIDTH * playerHealthPer);
            lblEnemyHealthFull.Width = (int)(MAX_HEALTHBAR_WIDTH * enemyHealthPer);

            lblPlayerHealthFull.Text = player.Health.ToString();
            lblEnemyHealthFull.Text = enemy.Health.ToString();


        }
        // Event handler for the Attack button. Handles the attack logic for both player and enemy.
        private void btnAttack_Click(object sender, EventArgs e)
        {

            player.OnAttack(-4);
            if (enemy.Health > 0)
            {
                enemy.OnAttack(-2);
            }

            UpdateHealthBars();
            if (player.Health <= 0)
            {
                // instance = null;
                groupBox2.Visible = true;
                groupBox2.Location = new Point(140, 184);
                // Close();
            }

            if (enemy.Health <= 0)
            {
                if (charname == "bosskoolaid")
                {
                    PictureBox t = Application.OpenForms["FrmLevel"].Controls.Find("picBossKoolAid", true)[0] as PictureBox;
                    Button b = Application.OpenForms["FrmLevel"].Controls.Find("button6", true)[0] as Button;
                    b.Visible = false;
                    t.Visible = false;


                }
                else if (charname == "enemycheeto")
                {
                    PictureBox t = Application.OpenForms["FrmLevel"].Controls.Find("picEnemyCheeto", true)[0] as PictureBox;
                    Button b = Application.OpenForms["FrmLevel"].Controls.Find("button14", true)[0] as Button;
                    b.Visible = false;
                    t.Visible = false;

                }
                else if (charname == "enemyPoisonPacket")
                {
                    PictureBox t = Application.OpenForms["FrmLevel"].Controls.Find("picEnemyPoisonPacket", true)[0] as PictureBox;
                    Button b = Application.OpenForms["FrmLevel"].Controls.Find("btn1", true)[0] as Button;
                    b.Visible = false;
                    t.Visible = false;
                }
                instance = null;
                Close();
            }





        }
        // Alters the enemy's health based on the damage amount.
        private void EnemyDamage(int amount)
        {
            enemy.AlterHealth(amount);
        }
        // Alters the player's health based on the damage amount.
        private void PlayerDamage(int amount)
        {
            player.AlterHealth(amount);
        }

        private void tmrFinalBattle_Tick(object sender, EventArgs e)
        {
            picBossBattle.Visible = false;
            tmrFinalBattle.Enabled = false;
        }
        // Timer event for the final battle. Hides the boss battle picture and disables the timer.
        private void FrmBattle_Load(object sender, EventArgs e)
        {
            // Timer to Close advert 
            System.Windows.Forms.Timer MyTimer = new System.Windows.Forms.Timer();
            MyTimer.Interval = (5000); // 5 seconds
            MyTimer.Tick += new EventHandler(timer1_Tick);
            MyTimer.Start();

            groupBox1.Visible = true;
            groupBox1.Location = new Point(160, 60);
            //picEnemy.BackgroundImage = enemy.Img;
        }
        // Event handler for entering the group box. Hides the advertisement group box.
        private void groupBox1_Enter(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
        }
        private int[] myTimer = new[] { 3, 1, 3 };

        // Timer event handler for displaying countdown in the advertisement.
        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = myTimer[0].ToString() + " " + myTimer[1].ToString() + " " + myTimer[2].ToString();
            if (myTimer[0] > 0)
            {
                myTimer[0] -= 1;
            }
            else if (myTimer[1] > 0)
            {
                myTimer[1] -= 1;
                groupBox1.Visible = false;
            }


        }
        // Event handler for a button click. Hides the advertisement group box.
        private void button1_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
        }
        // Event handler for a button click. Restarts the application and exits the environment.
        private void button2_Click(object sender, EventArgs e)
        {
            Application.Restart();
            Environment.Exit(0);
        }
        // Event handler for a click on the player's health label.
        private void lblPlayerHealthFull_Click(object sender, EventArgs e)
        {

        }
        // Event handler for a click on the enemy picture.
        private void picEnemy_Click(object sender, EventArgs e)
        {

        }
    }


}
