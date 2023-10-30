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

        private FrmBattle()
        {
            InitializeComponent();
            InitializeFleeButton();
            player = Game.player;
        }

        // Adding the Flee Abitlity Button.

        private void InitializeFleeButton()
        {
            this.fleeButton = new System.Windows.Forms.Button();

            // Assuming btnAttack is your attack button, position btnFlee beside it
            int spacing = 10; // Space between buttons, adjust as needed
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
        private void clickFleeButton(object sender, EventArgs e)
        {
            tryingToFlee();
        }
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

        public void SetupForBossBattle()
        {
            picBossBattle.Location = Point.Empty;
            picBossBattle.Size = ClientSize;
            picBossBattle.Visible = true;

            SoundPlayer simpleSound = new SoundPlayer(Resources.final_battle);
            simpleSound.Play();

            tmrFinalBattle.Enabled = true;
        }

        public static FrmBattle GetInstance(Enemy enemy)
        {
            if (instance == null)
            {
                instance = new FrmBattle();
                instance.enemy = enemy;
                instance.Setup();
            }
            return instance;
        }

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

        private void btnAttack_Click(object sender, EventArgs e)
        {
            player.OnAttack(-4);
            if (enemy.Health > 0)
            {
                enemy.OnAttack(-2);
            }

            UpdateHealthBars();
            if (player.Health <= 0 || enemy.Health <= 0)
            {
                instance = null;
                Close();
            }
        }

        private void EnemyDamage(int amount)
        {
            enemy.AlterHealth(amount);
        }

        private void PlayerDamage(int amount)
        {
            player.AlterHealth(amount);
        }

        private void tmrFinalBattle_Tick(object sender, EventArgs e)
        {
            picBossBattle.Visible = false;
            tmrFinalBattle.Enabled = false;
        }
    }
}
